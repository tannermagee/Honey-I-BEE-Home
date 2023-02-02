using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField]
    private MapManager mapManager;
    [SerializeField]
    private UiManager uiManager;
    [SerializeField]
    private PopUpManager popUpManager;

    private float timer = 0f;

    private string openingMessage = "These Bees are endangered. They've decided to raise 100,000 dollars to spread awareness of their endangerment.\n\n<b>Left click</b> on flowers to have the Bees harvest them for Pollen and Nectar! Use <b>WASD or Arrow keys</b> to navigate the map. You can also scroll using the <b>mouse wheel</b>.";

    // Start is called before the first frame update
    void Start()
    {
        mapManager.GenerateMap();
        popUpManager.OpenPopUp(openingMessage);
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        uiManager.UpdateTimer();
    }

    public float GetTimer() { return timer; }
}
