using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaxJob : Job
{
    public WaxJob(Vector3Int position)
    {
        this.position = position;
        this.workSeconds = 5;
        this.jobName = "Wax";
    }

    public WaxJob(Vector3Int position, int workSeconds)
    {
        this.position = position;
        this.workSeconds = workSeconds;
    }
}
