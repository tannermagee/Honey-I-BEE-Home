using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    private int nectar;
    private int pollen;
    private int honey;
    private int wax = 0;
    private int currentHoneycombs = 7;
    private int maxCapacity;
    private int currentMoney = 0;
    private int currentBees = 2;
    private bool isBuffed = false;
    private bool firstPollen = true;
    private bool firstNectar = true;
    private bool firstHoney = true;
    private bool firstWax = true;
    private bool firstHive = true;
    private string firstPollenMessage = "The Bees have made their first Pollen deposit.\n\nOnce the Bees have collected 5 Pollen, you can <b>Left Click</b> on the Pollen Honeycomb to give the Bees a temporary Buff!";
    private string firstNectarMessage = "The Bees have made their first Nectar deposit.\n\nOnce the Bees have collected 5 Nectar, you can <b>Left Click</b> on the Nectar Honeycomb to turn the Nectar into Honey!";
    private string firstHoneyMessage = "The Bees have made their first Honey deposit.\n\nOnce the Bees have collected 5 Honey, you can <b>Left Click</b> on the Honey Honeycomb to turn the Honey into Wax! Alternatively, you can sell the Honey for a profit.";
    private string firstWaxMessage = "The Bees have made their first Wax deposit.\n\nOnce the Bees have collected 10 Wax, you can <b>Left Click</b> on the Build Icon to build more Honeycombs!";
    private string firstHiveMessage = "The Bees have built their first Honeycomb.\n\nBuilding Honeycombs increase the resource capacity of the hive and the sell price of honey. Every 3rd Honeycomb will spawn another Bee!\n\n<b>See how fast you can raise 100k. Good Luck!</b>";

    [Header("Managers")]
    [SerializeField]
    private MapManager mapManager;
    [SerializeField]
    private UiManager uiManager;
    [SerializeField]
    private PlayerMouseManager playerMouseManager;
    [SerializeField]
    private PopUpManager popUpManager;
    [SerializeField]
    private GameManager gameManager;

    [Header("Bees")]
    [SerializeField]
    private GameObject beePrefab;
    [SerializeField]
    private GameObject beeParent;

    [Header("Buffs")]
    [SerializeField]
    private int speedBuff;
    [SerializeField]
    private int rotationBuff;
    [SerializeField]
    private int buffTime;

    [Header("Drop Rates")]
    [SerializeField]
    private int maxDropAmount;

    [Header("Costs")]
    [SerializeField]
    private int pollenCost;
    [SerializeField]
    private int honeyCost;
    [SerializeField]
    private int waxCost;
    [SerializeField]
    private int hiveCost;
    [SerializeField]
    private int honeycombCapacity;

    [Header("Honey Selling")]
    [SerializeField]
    private int honeyCombModifier;
    [SerializeField]
    private int baseHoneyCost;
    [SerializeField]
    private int endMoney;


    // Start is called before the first frame update
    void Start()
    {
        maxCapacity = currentHoneycombs * honeycombCapacity;
        uiManager.UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Tuple<int,int> FinishFlowerJob(Vector3Int position)
    {
        mapManager.FinishFlowerJob(position);
        return new Tuple<int, int>(UnityEngine.Random.Range(1, maxDropAmount+1), UnityEngine.Random.Range(1, maxDropAmount+1));
    }

    public void StartPollenJob()
    {
        pollen -= pollenCost;
        uiManager.UpdateUI();
    }

    public void CancelPollenJob()
    {
    }

    public void FinishPollenJob(Vector3Int position)
    {
        StartCoroutine(BuffBees());
        mapManager.FinishPollenJob(position);
        uiManager.UpdateUI();
    }

    public bool CanStartPollenJob()
    {
        return pollenCost <= pollen;
    }

    public void StartHoneyJob()
    {
        nectar -= honeyCost;
        uiManager.UpdateUI();
    }

    public void CancelHoneyJob()
    {
        nectar += honeyCost;
        nectar = Mathf.Clamp(nectar, 0, maxCapacity);
        uiManager.UpdateUI();
    }

    public void FinishHoneyJob(Vector3Int position)
    {
        honey += honeyCost;
        honey = Mathf.Clamp(honey, 0, maxCapacity);
        mapManager.FinishHoneyJob(position);
        uiManager.UpdateUI();
        if (firstHoney)
        {
            popUpManager.OpenPopUp(firstHoneyMessage);
            firstHoney = false;
        }
    }

    public bool CanStartHoneyJob()
    {
        return honeyCost <= nectar;
    }

    public void StartWaxJob()
    {
        honey -= waxCost;
        uiManager.UpdateUI();
    }

    public void CancelWaxJob()
    {
        honey += waxCost;
        honey = Mathf.Clamp(honey, 0, maxCapacity);
        uiManager.UpdateUI();
    }

    public void FinishWaxJob(Vector3Int position)
    {
        wax += waxCost;
        wax = Mathf.Clamp(wax, 0, maxCapacity);
        mapManager.FinishWaxJob(position);
        uiManager.UpdateUI();
        if (firstWax)
        {
            popUpManager.OpenPopUp(firstWaxMessage);
            firstWax = false;
        }
    }

    public bool CanStartWaxJob()
    {
        return waxCost <= honey;
    }

    public void StartHiveJob()
    {
        wax -= hiveCost;
        uiManager.UpdateUI();
    }

    public void CancelHiveJob()
    {
        wax += hiveCost;
        wax = Mathf.Clamp(wax, 0, maxCapacity);
        uiManager.UpdateUI();
    }

    public void FinishHiveJob(Vector3Int position)
    {
        mapManager.FinishHiveJob(position);
        IncreaseHoneycombs();
        if(currentHoneycombs % 3 == 0)
        {
            GameObject bee = GameObject.Instantiate(beePrefab, Vector3.zero, Quaternion.identity, beeParent.transform);
            bee.name = "Bee" + currentBees.ToString();
            currentBees++;
        }
        if (firstHive)
        {
            popUpManager.OpenPopUp(firstHiveMessage);
            firstHive = false;
        }
    }

    public bool CanStartHiveJob()
    {
        return hiveCost <= wax;
    }

    public int GetNectar() { return nectar; }

    public int GetPollen() { return pollen; }

    public int GetHoney() { return honey; }

    public int GetWax() { return wax; }

    public int GetCurrentMoney() { return currentMoney; }

    public int GetEndMoney() { return endMoney; }

    public int GetMaxCapacity()
    {
        return maxCapacity;
    }

    public void IncreaseHoneycombs()
    {
        currentHoneycombs++;
        maxCapacity = currentHoneycombs * honeycombCapacity;
        uiManager.UpdateUI();
    }

    public void DepositPollen(int beePollen)
    {
        this.pollen += beePollen;
        this.pollen = Mathf.Clamp(pollen, 0, maxCapacity);
        Debug.Log($"Depositing {beePollen}. Total pollen is now {this.pollen}.");
        uiManager.UpdateUI();
        if (firstPollen)
        {
            popUpManager.OpenPopUp(firstPollenMessage);
            firstPollen= false;
        }
    }

    public void DepositNectar(int beeNectar)
    {
        this.nectar += beeNectar;
        this.nectar = Mathf.Clamp(nectar, 0, maxCapacity);
        Debug.Log($"Depositing {beeNectar}. Total nectar is now {this.nectar}.");
        uiManager.UpdateUI();
        if (firstNectar)
        {
            popUpManager.OpenPopUp(firstNectarMessage);
            firstNectar = false;
        }
    }

    public bool CanSellHoney()
    {
        return honey > 0;
    }

    public void SellHoney()
    {
        honey--;
        currentMoney += baseHoneyCost * (currentHoneycombs * honeyCombModifier);
        uiManager.UpdateUI();
        if(currentMoney >= endMoney)
        {
            popUpManager.OpenPopUp(GenerateWinMessage());
        }
    }

    private string GenerateWinMessage()
    {
        float time = gameManager.GetTimer();
        int minutes = (int)time / 60;
        int seconds = (int)time % 60;
        string timeText = minutes + ":" + seconds.ToString("D2");
        return $"YOU DID IT!\n\nYou helped the Bees raise 100k in {timeText}. This accomplishment has attracted the eyes of <b>JOE ROGAN</b>.\n\nCheck out the podcast that inspired this game: The Joe Rogan Experience #1908 Erika Thompson";
    }

    public bool GetIsBuffed() { return isBuffed; }

    IEnumerator BuffBees()
    {
        isBuffed = true;
        Debug.Log($"Buffing Bees.");
        BuffBees(speedBuff, rotationBuff, true);
        yield return new WaitForSeconds(buffTime);

        isBuffed = false;
        BuffBees(-speedBuff, -rotationBuff, false);
        Debug.Log($"Done buffing Bees.");
    }

    private void BuffBees(int speedValue, int rotationValue, bool buffed)
    {
        foreach(BeeBehavior bee in beeParent.GetComponentsInChildren<BeeBehavior>())
        {
            bee.ChangeSpeed(speedValue);
            bee.ChangeRotationSpeed(rotationValue);
            bee.ShowBuffedBee(buffed);
            Debug.Log($"{bee.name} speed change: {speedValue}, rotation speed change: {rotationValue}.");
        }
    }
}
