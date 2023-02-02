using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerJob : Job
{
    public FlowerJob(Vector3Int position)
    {
        this.position = position;
        this.workSeconds = 2;
        this.jobName = "Flower";
    }

    public FlowerJob(Vector3Int position, int workSeconds)
    {
        this.position = position;
        this.workSeconds = workSeconds;
    }
}
