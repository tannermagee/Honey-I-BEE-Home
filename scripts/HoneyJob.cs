using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoneyJob : Job
{
    public HoneyJob(Vector3Int position)
    {
        this.position = position;
        this.workSeconds = 5;
        this.jobName = "Honey";
    }

    public HoneyJob(Vector3Int position, int workSeconds)
    {
        this.position = position;
        this.workSeconds = workSeconds;
    }
}
