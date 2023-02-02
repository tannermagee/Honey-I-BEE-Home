using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraManager : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField]
    private Camera mainCamera;
    [SerializeField]
    private float CameraSpeed;
    [SerializeField]
    private float ZoomMinBound;
    [SerializeField]
    private float ZoomDefault;
    [SerializeField]
    private float ZoomMaxBound;
    [SerializeField]
    private float MouseZoomSpeed;

    [Header("Managers")]
    [SerializeField]
    private MapManager mapManager;

    private float xMin;
    private float xMax;
    private float yMin;
    private float yMax;


    // Start is called before the first frame update
    void Start()
    {
        mainCamera.fieldOfView = ZoomDefault;
        CalculateCameraBounds(mapManager.GetHeight(), mapManager.GetWidth(), mapManager.GetGroundTileMap());
    }

    // Update is called once per frame
    void Update()
    {
        MoveCamera();
        ScrollCamera();
    }

    private void CalculateCameraBounds(int height, int width, Tilemap groundTileMap)
    {
        Vector3 yMinVector = groundTileMap.CellToWorld(new Vector3Int(-width / 2, 0, 0));
        Vector3 yMaxVector = groundTileMap.CellToWorld(new Vector3Int(width / 2, 0, 0));
        Vector3 xMinVector = groundTileMap.CellToWorld(new Vector3Int(0, -height / 2, 0));
        Vector3 xMaxVector = groundTileMap.CellToWorld(new Vector3Int(0, height / 2, 0));
        Debug.Log($"xMinVector: {xMinVector}, xMaxVector: {xMaxVector}, yMinVector: {yMinVector}, yMaxVector: {yMaxVector}");

        xMin = xMinVector.x;
        xMax = xMaxVector.x;
        yMin = yMinVector.y;
        yMax = yMaxVector.y;
    }

    private void MoveCamera()
    {
        float xAxisValue = Input.GetAxis("Horizontal") * CameraSpeed;
        float yAxisValue = Input.GetAxis("Vertical") * CameraSpeed;

        float xClamp = Mathf.Clamp(mainCamera.transform.position.x + xAxisValue, xMin, xMax);
        float yClamp = Mathf.Clamp(mainCamera.transform.position.y + yAxisValue, yMin, yMax);

        mainCamera.transform.position = new Vector3(xClamp, yClamp, mainCamera.transform.position.z);
    }

    private void ScrollCamera()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        Zoom(scroll, MouseZoomSpeed);
    }


    void Zoom(float deltaMagnitudeDiff, float speed)
    {
        mainCamera.fieldOfView += deltaMagnitudeDiff * speed;
        // set min and max value of Clamp function upon your requirement
        mainCamera.fieldOfView = Mathf.Clamp(mainCamera.fieldOfView, ZoomMinBound, ZoomMaxBound);
    }
}
