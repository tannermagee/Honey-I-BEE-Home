using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PollenJob : Job
{
    public PollenJob(Vector3Int position)
    {
        this.position = position;
        this.workSeconds = 3;
        this.jobName = "Pollen";
    }

    public PollenJob(Vector3Int position, int workSeconds)
    {
        this.position = position;
        this.workSeconds = workSeconds;
    }
}
