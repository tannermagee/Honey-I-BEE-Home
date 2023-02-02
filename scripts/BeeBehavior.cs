using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class BeeBehavior : MonoBehaviour
{
    [Header("Bee Stats")]
    [SerializeField]
    private float speed;
    [SerializeField]
    private float rotationSpeed;
    [SerializeField]
    private int pollenCapacity;
    [SerializeField]
    private int nectarCapacity;

    [Header("Images")]
    [SerializeField]
    private Image workSpinner;
    [SerializeField]
    private GameObject buffedEffect;

    private ResourceManager resourceManager;
    private int currentPollen = 0;
    private int currentNectar = 0;

    private WorkManager workManager;
    private MapManager mapManager;
    private Job currentJob;
    private Vector3 jobWorldPosition;


    // potential animation bools
    private bool flying = false;
    private bool takingOff = false;
    private bool landing = false;
    private bool isWorking = false;
    private bool depositing = false;

    private float workTime = 0f;
    private float timer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        workManager = GameObject.Find("WorkManager").GetComponent<WorkManager>();
        resourceManager = GameObject.Find("ResourceManager").GetComponent<ResourceManager>();
        mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentJob == null && CanCarry() && !depositing)
        {
            GetJob();
        }
        else
        {
            if (!CanCarry() || depositing)
            {
                DepositResources();
            }
            else
            {
                PerformJob();
            }
        }
    }

    private bool CanCarry()
    {
        return currentPollen <= pollenCapacity && currentNectar <= nectarCapacity;
    }

    private void GetJob()
    {
        if (workManager.HasJob())
        {
            currentJob = workManager.GetFirstJob();
            currentJob.SetBeeBehavior(this);
            jobWorldPosition = mapManager.GetInteractableTileMap().CellToWorld(currentJob.GetPosition());
            Debug.Log($"{this.transform.name} picked up new {currentJob.GetJobName()} Job at {currentJob.GetPosition()}.");
        }
    }

    private void PerformJob()
    {
        if (mapManager.GetInteractableTileMap().WorldToCell(transform.position).Equals(currentJob.GetPosition()))
        {
            Debug.Log($"{this.name} done flying to {currentJob.GetJobName()} job.");
            flying = false;
            landing = true;
            //might be able to do a land animation!
            if (!isWorking)
            {
                Debug.Log("!isWorking");
                if (workManager.CurrentJobStillValid(currentJob))
                {
                    Debug.Log($"{this.name} is starting {currentJob.GetJobName()} job.");
                    StartCoroutine(StartJob(true, currentJob.GetWorkSeconds()));
                }
                else
                {
                    currentJob = null;
                }
            }
            else
            {
                UpdateWorkSpinner();
            }
        }
        else
        {
            if (!flying)
            {
                takingOff = true;
                Vector3 target = new Vector3(jobWorldPosition.x - transform.position.x, jobWorldPosition.y - transform.position.y, 0);
                float angle = Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg;
                Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                if (targetRotation == transform.rotation)
                {
                    Debug.Log("Done rotating");
                    takingOff = false;
                }
            }

            //Fly to job
            if (!takingOff)
            {
                //Debug.Log("Flying.");
                transform.position = Vector3.MoveTowards(transform.position, jobWorldPosition, speed * Time.deltaTime);
                takingOff = false;
                flying = true;
            }
        }
    }

    private void DepositResources() {
        if(currentJob == null)
        {
            float pollenMag = (transform.position - mapManager.GetPollenBankWorldLocation()).magnitude;
            float nectarMag = (transform.position - mapManager.GetNectarBankWorldLocation()).magnitude;

            if(currentPollen > 0)
            {
                depositing = true;
                Debug.Log($"Creating new Pollen Deposit Job.");
                currentJob = new DepositJob(mapManager.GetPollenBankCellLocation());
                currentJob.SetBeeBehavior(this);
                jobWorldPosition = mapManager.GetInteractableTileMap().CellToWorld(currentJob.GetPosition());
                workManager.AddCurrentWorkingJob(currentJob);
            }
            else
            {
                depositing = true;
                Debug.Log($"Creating new Nectar Deposit Job.");
                currentJob = new DepositJob(mapManager.GetNectarBankCellLocation());
                currentJob.SetBeeBehavior(this);
                jobWorldPosition = mapManager.GetInteractableTileMap().CellToWorld(currentJob.GetPosition());
                workManager.AddCurrentWorkingJob(currentJob);
            }
        }
        else
        {
            Debug.Log($"Current Job: {currentJob.GetJobName()}.");
            PerformJob();
        }
    }

    IEnumerator StartJob(bool isWorking, float workTime)
    {
        this.isWorking = isWorking;
        this.workTime = workTime;
        timer = 0;
        workSpinner.fillAmount = timer;
        workSpinner.gameObject.SetActive(isWorking);
        Debug.Log($"{this.name} working for {workTime} second.");
        yield return new WaitForSeconds(workTime);

        Debug.Log($"{this.name} DONE working for {workTime} second.");
        this.isWorking = false;
        workSpinner.gameObject.SetActive(false);
        FinishJob();
        workManager.FinishCurrentJob(currentJob.GetPosition());
        currentJob = null;
    }

    private void FinishJob()
    {
        switch(currentJob.GetJobName())
        {
            case "Flower":
                Tuple<int,int> resourcesGathered = resourceManager.FinishFlowerJob(currentJob.GetPosition());
                currentNectar += resourcesGathered.Item1;
                currentPollen += resourcesGathered.Item2;
                mapManager.FlowerSpawn();
                break;
            case "Pollen":
                resourceManager.FinishPollenJob(currentJob.GetPosition());
                break;
            case "Honey": 
                resourceManager.FinishHoneyJob(currentJob.GetPosition());
                break;
            case "Wax":
                resourceManager.FinishWaxJob(currentJob.GetPosition());
                break;
            case "Build":
                resourceManager.FinishHiveJob(currentJob.GetPosition());
                break;
            case "Deposit":
                if (mapManager.GetNectarBankCellLocation().Equals(currentJob.GetPosition()))
                {
                    resourceManager.DepositNectar(currentNectar);
                    currentNectar = 0;
                }
                else
                {
                    resourceManager.DepositPollen(currentPollen);
                    currentPollen = 0;
                }

                if (currentPollen == 0 && currentNectar == 0)
                {
                    depositing = false;
                }
                break;
            default: break;
        }
    }

    private void UpdateWorkSpinner()
    {
        if (isWorking)
        {
            timer += Time.deltaTime;
            if (timer > workTime)
            {
                timer = workTime;
            }
            workSpinner.fillAmount = timer / workTime;
        }
    }

    public void ChangeSpeed(int value)
    {
        speed += value;
    }

    public void ChangeRotationSpeed(int value)
    {
        rotationSpeed += value;
    }

    public void ShowBuffedBee(bool isBuffed)
    {
        buffedEffect.SetActive(isBuffed);
    }
}
