using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class WorkManager : MonoBehaviour
{
    private Queue<Job> jobQueue = new Queue<Job> ();
    private Queue<Job> currentWorkingJobsQueue = new Queue<Job>();


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Job GetFirstJob() {
        Job currentJob = jobQueue.Dequeue();
        currentWorkingJobsQueue.Enqueue(currentJob);
        return currentJob;
    }

    public void AddJob(Job job) { jobQueue.Enqueue(job); }

    public void AddCurrentWorkingJob(Job job) { currentWorkingJobsQueue.Enqueue(job); }

    public void RemoveJob(Vector3Int position) { 
        jobQueue = new Queue<Job>(jobQueue.Where(j => !j.GetPosition().Equals(position)));
        currentWorkingJobsQueue = new Queue<Job>(currentWorkingJobsQueue.Where(j => !j.GetPosition().Equals(position)));
        //TODO tell bee their job has been removed
    }

    public void FinishCurrentJob(Vector3Int position)
    {
        currentWorkingJobsQueue = new Queue<Job>(currentWorkingJobsQueue.Where(j => !j.GetPosition().Equals(position)));
    }

    public bool CurrentJobStillValid(Job job) { return currentWorkingJobsQueue.Contains(job); }

    public bool HasJob() { return jobQueue.Count > 0;}
}
