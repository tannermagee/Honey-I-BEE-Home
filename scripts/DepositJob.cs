using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepositJob : Job
{
    public DepositJob(Vector3Int position)
    {
        this.position = position;
        this.workSeconds = 1;
        this.jobName = "Deposit";
    }

    public DepositJob(Vector3Int position, int workSeconds)
    {
        this.position = position;
        this.workSeconds = workSeconds;
    }
}
