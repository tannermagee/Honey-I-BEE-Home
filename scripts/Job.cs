using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Job
{
    protected Vector3Int position;
    protected int workSeconds;
    protected string jobName;
    protected BeeBehavior bee;

    public virtual Vector3Int GetPosition() { return position; }

    public virtual int GetWorkSeconds() { return workSeconds; }

    public virtual string GetJobName() { return jobName; }

    public virtual BeeBehavior GetBeeBehavior() { return bee; }

    public virtual void SetBeeBehavior(BeeBehavior beeBehavior) { this.bee = beeBehavior; }
}
