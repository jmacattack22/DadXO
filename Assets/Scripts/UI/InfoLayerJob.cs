using UnityEngine;
using System.Collections.Generic;

public class InfoLayerJob
{
    public enum InfoJob
	{
		Clear, Region, Town, RegionPreview, TownPreview
	}

	private InfoJob job;
	private int jobIndex;

	private List<InfoLayerBehaviour.Labels> labels = new List<InfoLayerBehaviour.Labels>();
	private bool calendarVisible = false;

    public InfoLayerJob(InfoJob job, int jobIndex)
	{
		this.job = job;
		this.jobIndex = jobIndex;

		populateLables();
	}

    private void populateLables()
	{
		if (job.Equals(InfoJob.Region))
		{
			labels.Add(InfoLayerBehaviour.Labels.Title);
		} 
		else if (job.Equals(InfoJob.Town))
		{
			labels.Add(InfoLayerBehaviour.Labels.Title);
		}
		else if (job.Equals(InfoJob.RegionPreview))
		{
			labels.Add(InfoLayerBehaviour.Labels.RightInfoPanelTitle);
			labels.Add(InfoLayerBehaviour.Labels.RightInfoPanelSubTitle);

			calendarVisible = true;
		}
		else if (job.Equals(InfoJob.TownPreview))
		{
			labels.Add(InfoLayerBehaviour.Labels.RightInfoPanelTitle);
            labels.Add(InfoLayerBehaviour.Labels.RightInfoPanelSubTitle);
		}
	}

    //Getters
    public InfoJob Job
	{
		get { return job; }
	}

    public int JobIndex 
	{
		get { return jobIndex; }
	}

	public List<InfoLayerBehaviour.Labels> Labels
	{
		get { return labels; }
	}

    public bool CalendarVisible
	{
		get { return calendarVisible; }
	}
}
