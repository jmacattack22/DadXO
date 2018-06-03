using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class InfoLayerBehaviour : MonoBehaviour {
   
    public enum Labels 
	{
		Title
	}

	private Dictionary<Labels, TextMeshProUGUI> textLabels = new Dictionary<Labels, TextMeshProUGUI>();

	private List<InfoLayerJob> jobs = new List<InfoLayerJob>();

	private DataPool worldData;

	void Start () {
		textLabels.Add(Labels.Title, transform.GetChild(0).GetComponent<TextMeshProUGUI>());
	}

	void Update () {
		if (jobs.Count > 0)
		{
			executeJob();
		}
	}

    private void clearInfoLayer()
	{
		foreach (Labels l in textLabels.Keys)
		{
			textLabels[l].SetText("");
		}
	}

    private void executeJob()
	{
		clearInfoLayer();

		populateLabels(jobs[0]);
              
		jobs.RemoveAt(0);
	}

	protected void populateLabels(InfoLayerJob job)
	{
		if (job.Job.Equals(InfoLayerJob.InfoJob.Region))
		{
			populateRegionLabels(job);
		}
		else if (job.Job.Equals(InfoLayerJob.InfoJob.Town))
		{
			populateTownLabels(job);
		}
		else if (job.Job.Equals(InfoLayerJob.InfoJob.Clear))
		{
			clearInfoLayer();
		}
	}

	private void populateLabel(Labels l, string content)
    {
		textLabels[l].SetText(content);
    }

	private void populateRegionLabels(InfoLayerJob job)
    {
        foreach (Labels l in job.Labels)
		{
			string content = "";

            if (l.Equals(Labels.Title))
			{
				print(job.JobIndex);
				content = worldData.Regions[job.JobIndex].Name;
			}
         
			populateLabel(l, content);
		}
    }   

	private void populateTownLabels(InfoLayerJob job)
	{
		if (job.JobIndex >= 0)
		{
			foreach (Labels l in job.Labels)
			{
				string content = "";

				if (l.Equals(Labels.Title))
				{
					content = worldData.Towns[job.JobIndex].Name + " " + job.JobIndex;
				}

				populateLabel(l, content);
			}
		}
	}

	public void sendJob(InfoLayerJob job)
	{
		jobs.Add(job);
	}

	public void updateWorldData(DataPool worldData)
	{
		this.worldData = worldData;
	}
}
