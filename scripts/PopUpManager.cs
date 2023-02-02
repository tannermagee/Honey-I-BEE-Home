using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PopUpManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField]
    private GameObject popUpParent;
    [SerializeField]
    private TextMeshProUGUI popUpText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void OpenPopUp(string message) { 
        popUpText.text = message;
        popUpParent.SetActive(true);
        Time.timeScale = 0;
    }

    public void ClosePopUp()
    {
        popUpParent.SetActive(false);
        Time.timeScale = 1;
    }
}
