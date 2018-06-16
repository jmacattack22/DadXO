﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class InfoLayerBehaviour : MonoBehaviour {
   
    public enum Labels 
	{
		Title, RightInfoPanelTitle, RightInfoPanelDetail
	}

	private Dictionary<Labels, TextMeshProUGUI> textLabels = new Dictionary<Labels, TextMeshProUGUI>();

	private Transform rightInfoPanel;

	private List<InfoLayerJob> jobs = new List<InfoLayerJob>();

	private DataPool worldData;

	void Start () {
		loadUI();
	}

	private void loadUI()
	{
		textLabels.Add(Labels.Title, transform.GetChild(0).GetComponent<TextMeshProUGUI>());
		textLabels.Add(Labels.RightInfoPanelTitle, transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>());
		textLabels.Add(Labels.RightInfoPanelDetail, transform.GetChild(1).GetChild(2).GetComponent<TextMeshProUGUI>());

		rightInfoPanel = transform.GetChild(1).GetComponent<Transform>();
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

    private void clearInfoLayer(InfoLayerJob job)
	{
		foreach (Labels l in job.Labels)
		{
			textLabels[l].SetText("");
		}
	}

    private void executeJob()
	{
		clearInfoLayer(jobs[0]);

		populateLabels(jobs[0]);
              
		jobs.RemoveAt(0);
	}

	protected void populateLabels(InfoLayerJob job)
	{
		if (job.JobIndex >= 0)
		{
			Dictionary<Labels, string> tuple = new Dictionary<Labels, string>();
            if (job.Job.Equals(InfoLayerJob.InfoJob.Region) || job.Job.Equals(InfoLayerJob.InfoJob.RegionPreview))
            {
                tuple = job.JobIndex >= 0 ? InfoLayerTuple.getRegionTuple(worldData, job.JobIndex) : tuple;
            }
			else if (job.Job.Equals(InfoLayerJob.InfoJob.Town) || job.Job.Equals(InfoLayerJob.InfoJob.TownPreview))
            {
                tuple = job.JobIndex >= 0 ? InfoLayerTuple.getTownTuple(worldData, job.JobIndex) : tuple;
            }

            foreach (Labels l in job.Labels)
            {
                populateLabel(l, tuple[l]);
            }	
		}

		if (job.Job.Equals(InfoLayerJob.InfoJob.Clear))
        {
            clearInfoLayer();
        }
     
	}

	private void populateLabel(Labels l, string content)
    {
		textLabels[l].SetText(content);
    }

	public void sendJob(InfoLayerJob job)
	{
		jobs.Add(job);
	}

    public void toggleRightPanel()
	{
		rightInfoPanel.GetComponent<EasyTween>().OpenCloseObjectAnimation();
	}

	public void updateWorldData(DataPool worldData)
	{
		this.worldData = worldData;
	}
}
