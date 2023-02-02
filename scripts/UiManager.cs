using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField]
    private TextMeshProUGUI pollenText;
    [SerializeField]
    private TextMeshProUGUI nectarText;
    [SerializeField]
    private TextMeshProUGUI honeyText;
    [SerializeField]
    private TextMeshProUGUI waxText;
    [SerializeField]
    private TextMeshProUGUI moneyText;
    [SerializeField]
    private Slider moneySlider;
    [SerializeField]
    private TextMeshProUGUI timerText;

    [Header("Managers")]
    [SerializeField]
    private ResourceManager resourceManager;
    [SerializeField]
    private PlayerMouseManager playerMouseManager;
    [SerializeField]
    private GameManager gameManager;

    [Header("Buttons")]
    [SerializeField]
    private Button buildButton;
    [SerializeField]
    private Button sellHoneyButton;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateUI() {
        UpdatePollen();
        UpdateNectar();
        UpdateHoney();
        UpdateWax();
        UpdateButtons();
        UpdateMoney();
    }

    public void UpdateTimer()
    {
        float time = gameManager.GetTimer();
        int minutes = (int)time / 60;
        int seconds = (int)time % 60;
        timerText.text = minutes + ":" + seconds.ToString("D2");
    }

    private void UpdatePollen()
    {
        pollenText.text = "Pollen: " + resourceManager.GetPollen().ToString() + "/" + resourceManager.GetMaxCapacity().ToString();
        if(resourceManager.GetPollen() / resourceManager.GetMaxCapacity() == 1)
        {
            pollenText.color = Color.red;
        }
        else if((float)resourceManager.GetPollen() / resourceManager.GetMaxCapacity() > .8)
        {
            pollenText.color = Color.yellow;
        }
        else
        {
            pollenText.color = Color.white;
        }
    }

    private void UpdateNectar()
    {
        nectarText.text = "Nectar: " + resourceManager.GetNectar().ToString() + "/" + resourceManager.GetMaxCapacity().ToString();
        if (resourceManager.GetNectar() / resourceManager.GetMaxCapacity() == 1)
        {
            nectarText.color = Color.red;
        }
        else if ((float)resourceManager.GetNectar() / resourceManager.GetMaxCapacity() > .8)
        {
            nectarText.color = Color.yellow;
        }
        else
        {
            nectarText.color = Color.white;
        }
    }

    private void UpdateHoney()
    {
        honeyText.text = "Honey: " + resourceManager.GetHoney().ToString() + "/" + resourceManager.GetMaxCapacity().ToString();
        if (resourceManager.GetHoney() / resourceManager.GetMaxCapacity() == 1)
        {
            honeyText.color = Color.red;
        }
        else if ((float)resourceManager.GetHoney() / resourceManager.GetMaxCapacity() > .8)
        {
            honeyText.color = Color.yellow;
        }
        else
        {
            honeyText.color = Color.white;
        }
    }

    private void UpdateWax()
    {
        waxText.text = "Wax: " + resourceManager.GetWax().ToString() + "/" + resourceManager.GetMaxCapacity().ToString();
        if (resourceManager.GetWax() / resourceManager.GetMaxCapacity() == 1)
        {
            waxText.color = Color.red;
        }
        else if ((float)resourceManager.GetWax() / resourceManager.GetMaxCapacity() > .8)
        {
            waxText.color = Color.yellow;
        }
        else
        {
            waxText.color = Color.white;
        }
    }

    private void UpdateButtons()
    {
        buildButton.interactable = resourceManager.CanStartHiveJob();
        sellHoneyButton.interactable = resourceManager.CanSellHoney();
    }

    private void UpdateMoney()
    {
        int currentMoney = resourceManager.GetCurrentMoney();
        string moneyString = currentMoney.ToString() + "/100k";
        if (currentMoney > 999)
        {
            int moneyInThousands = currentMoney / 1000;
            moneyString = moneyInThousands.ToString() + "k/100k";
        }
        Debug.Log($"New Money: {moneyString}");
        moneyText.text = moneyString;

        float moneyProgress = (float)currentMoney / resourceManager.GetEndMoney();
        Debug.Log($"New Money Progress: {moneyProgress}");
        moneySlider.value = moneyProgress;
    }

    public void Building()
    {
        if (playerMouseManager.GetBuilding())
        {
            playerMouseManager.SetBuilding(false);
        }
        else
        {
            playerMouseManager.SetBuilding(true);
        }
    }
}
