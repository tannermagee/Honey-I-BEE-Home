using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class PlayerMouseManager : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField]
    private Camera playerCamera;

    private Tilemap groundTileMap;
    private Tilemap interactableTileMap;
    private Tilemap buildTemplateTileMap;
    private Tilemap workTileMap;
    private Tilemap temporaryTileMap;

    [Header("Managers")]
    [SerializeField]
    private MapManager mapManager;
    [SerializeField]
    private WorkManager workManager;
    [SerializeField]
    private ResourceManager resourceManager;

    private bool building = false;
    private Vector3Int previousBuildLocation = Vector3Int.zero;

    // Start is called before the first frame update
    void Start()
    {
        groundTileMap = mapManager.GetGroundTileMap();
        interactableTileMap = mapManager.GetInteractableTileMap();
        buildTemplateTileMap = mapManager.GetBuildTemplateTileMap();
        workTileMap = mapManager.GetWorkTileMap();
        temporaryTileMap = mapManager.GetTemporaryTileMap();
    }

    // Update is called once per frame
    void Update()
    {
        MouseInteraction();
    }

    private void MouseInteraction()
    {
        Vector3Int mouseTilePosition = GetMouseWorldPositionToTilePosition();

        if (building)
        {
            bool isSafeBuildPosition = IsSafeBuildPosition(mouseTilePosition);
            temporaryTileMap.SetTile(mouseTilePosition, isSafeBuildPosition ? mapManager.GetHoneycombSafe() : mapManager.GetHoneycombFail());
            if (previousBuildLocation != mouseTilePosition)
            {
                temporaryTileMap.SetTile(previousBuildLocation, null);
            }

            if (Input.GetMouseButtonDown(0) && isSafeBuildPosition)
            {
                BuildInteraction(mouseTilePosition);
            }

            previousBuildLocation = mouseTilePosition;
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                FlowerInteraction(mouseTilePosition);
                PollenInteraction(mouseTilePosition);
                NectarInteraction(mouseTilePosition);
                HoneyInteraction(mouseTilePosition);
            }
        }


    }

    private void FlowerInteraction(Vector3Int position)
    {
        if (interactableTileMap.HasTile(position))
        {
            TileBase clickedTile = interactableTileMap.GetTile(position);
            if (mapManager.GetFlowerTiles().Contains(clickedTile))
            {
                if (!workTileMap.HasTile(position))
                {
                    workTileMap.SetTile(position, mapManager.GetWorkTile());
                    workManager.AddJob(new FlowerJob(position));
                    Debug.Log($"Added Flower Job at {position}.");
                }
                //else
                //{
                //    workTileMap.SetTile(position, null);
                //    workManager.RemoveJob(position);
                //    Debug.Log($"Removed Flower Job at {position}.");
                //}
            }
        }
    }

    private void PollenInteraction(Vector3Int position)
    {
        if (interactableTileMap.HasTile(position))
        {
            TileBase clickedTile = interactableTileMap.GetTile(position);
            if (mapManager.GetHoneycombPollen().Equals(clickedTile))
            {
                if (!workTileMap.HasTile(position))
                {
                    if (resourceManager.CanStartPollenJob() && !resourceManager.GetIsBuffed())
                    {
                        workTileMap.SetTile(position, mapManager.GetWorkTile());
                        workManager.AddJob(new PollenJob(position));
                        Debug.Log($"Added Pollen Job at {position}.");
                        resourceManager.StartPollenJob();
                    }
                }
                //else
                //{
                //    workTileMap.SetTile(position, null);
                //    workManager.RemoveJob(position);
                //    Debug.Log($"Removed Honey Job at {position}.");
                //    resourceManager.CancelHoneyJob();
                //}
            }
        }
    }

    private void NectarInteraction(Vector3Int position)
    {
        if (interactableTileMap.HasTile(position))
        {
            TileBase clickedTile = interactableTileMap.GetTile(position);
            if (mapManager.GetHoneycombNectar().Equals(clickedTile))
            {
                if (!workTileMap.HasTile(position))
                {
                    if (resourceManager.CanStartHoneyJob())
                    {
                        workTileMap.SetTile(position, mapManager.GetWorkTile());
                        workManager.AddJob(new HoneyJob(position));
                        Debug.Log($"Added Honey Job at {position}.");
                        resourceManager.StartHoneyJob();
                    }
                }
                //else
                //{
                //    workTileMap.SetTile(position, null);
                //    workManager.RemoveJob(position);
                //    Debug.Log($"Removed Honey Job at {position}.");
                //    resourceManager.CancelHoneyJob();
                //}
            }
        }
    }

    private void HoneyInteraction(Vector3Int position)
    {
        if (interactableTileMap.HasTile(position))
        {
            TileBase clickedTile = interactableTileMap.GetTile(position);
            if (mapManager.GetHoneycombHoney().Equals(clickedTile))
            {
                if (!workTileMap.HasTile(position))
                {
                    if(resourceManager.CanStartWaxJob())
                    {
                        workTileMap.SetTile(position, mapManager.GetWorkTile());
                        workManager.AddJob(new WaxJob(position));
                        Debug.Log($"Added Wax Job at {position}.");
                        resourceManager.StartWaxJob();
                    }
                }
                //else
                //{
                //    workTileMap.SetTile(position, null);
                //    workManager.RemoveJob(position);
                //    Debug.Log($"Removed Wax Job at {position}.");
                //    resourceManager.CancelWaxJob();
                //}
            }
        }
    }

    private void BuildInteraction(Vector3Int position)
    {
        //if(buildTemplateTileMap.HasTile(position))
        //{
        //    TileBase clickedTile = buildTemplateTileMap.GetTile(position);
        //    if(clickedTile.Equals(mapManager.GetHoneycombTemplate())) {
        //        buildTemplateTileMap.SetTile(position, null);
        //        workManager.RemoveJob(position);
        //        Debug.Log($"Removed Build Job at {position}.");
        //        resourceManager.CancelHiveJob();
        //    }
        //}
        //else
        //{
        buildTemplateTileMap.SetTile(position, mapManager.GetHoneycombTemplate());
        workManager.AddJob(new BuildJob(position));
        Debug.Log($"Submitted Build Job at {position}.");
        resourceManager.StartHiveJob();
        if (!resourceManager.CanStartHiveJob())
        {
            building = false;
            temporaryTileMap.ClearAllTiles();
        }
        //}
    }
    private bool IsSafeBuildPosition(Vector3Int position)
    {
        //check if position is in map bounds
        bool positionInBounds = mapManager.IsPositionInBounds(position);
        Debug.Log($"{position} in bounds: {positionInBounds}");
        if (!positionInBounds)
        {
            return false;
        }

        //check if position is a honeycomb
        bool hasInteractableTile = interactableTileMap.HasTile(position);
        if(hasInteractableTile)
        {
            TileBase currentTile = interactableTileMap.GetTile(position);
            if(mapManager.IsHoneyCombTile(currentTile))
            {
                Debug.Log($"{position} is a Honeycomb.");
                return false;
            }
        }

        //check if surrounding tile is honeycomb
        if (!mapManager.IsSurroundingHoneycombTile(position, interactableTileMap)) {
            return false;
        }

        //check if player has enough resources to build.
        if (!resourceManager.CanStartHiveJob())
        {
            Debug.Log($"Not enough resources to build.");
            if (!mapManager.GetHoneycombTemplate().Equals(buildTemplateTileMap.GetTile(position)))
            {
                Debug.Log($"Can cancel honeycomb build.");
                return false;
            }
        }

        return true;
    }

    private Vector3Int GetMouseWorldPositionToTilePosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Mathf.Abs(playerCamera.transform.position.z);
        Vector3 rawWorldMousePosition = playerCamera.ScreenToWorldPoint(mousePosition);
        Vector3Int tileWorldMousePosition = groundTileMap.WorldToCell(rawWorldMousePosition);
        //Debug.Log(tileWorldMousePosition);
        return tileWorldMousePosition;
    }

    public bool GetBuilding() {  return building; }
    public void SetBuilding(bool setBuilding)
    {
        building = setBuilding;
    }
}
