using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildJob : Job
{
    public BuildJob(Vector3Int position)
    {
        this.position = position;
        this.workSeconds = 5;
        this.jobName = "Build";
    }

    public BuildJob(Vector3Int position, int workSeconds)
    {
        this.position = position;
        this.workSeconds = workSeconds;
    }
}
