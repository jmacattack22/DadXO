using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class InfoLayerBehaviour : MonoBehaviour {
   
    public enum Labels 
	{
		RightInfoPanelTitle, RightInfoPanelSubTitle, MapRegionTitle, MapTownTitle, 
		Accuracy, Endurance, Health, Speed, Strength, AccuracyLevel, EnduranceLevel, HealthLevel, SpeedLevel, StrengthLevel
	}

	private Dictionary<Labels, Transform> progressBars = new Dictionary<Labels, Transform>();

	private Dictionary<Labels, TextMeshProUGUI> textLabels = new Dictionary<Labels, TextMeshProUGUI>();

	private Transform rightInfoPanel;
	private TournamentCalendarBehaviour rightInfoPanelCalendar;

	private List<InfoLayerJob> jobs = new List<InfoLayerJob>();
    
	private DataPool worldData;

	void Awake () 
	{
		loadUI();
	}

	private void loadUI()
	{
		textLabels.Add(Labels.RightInfoPanelTitle, GameObject.FindWithTag("RightInfoTitle").GetComponent<TextMeshProUGUI>());
		textLabels.Add(Labels.RightInfoPanelSubTitle, GameObject.FindWithTag("RightInfoSubTitle").GetComponent<TextMeshProUGUI>());
		textLabels.Add(Labels.MapRegionTitle, GameObject.FindWithTag("MapRegionTitle").GetComponent<TextMeshProUGUI>());
		textLabels.Add(Labels.MapTownTitle, GameObject.FindWithTag("MapTownTitle").GetComponent<TextMeshProUGUI>());      

		rightInfoPanelCalendar = GameObject.FindWithTag("RightInfoCalendar").GetComponent<TournamentCalendarBehaviour>();

		Transform acc = GameObject.FindWithTag("StatsUI").transform.GetChild(0).GetChild(3).transform;
		Transform end = GameObject.FindWithTag("StatsUI").transform.GetChild(0).GetChild(4).transform;
		Transform lif = GameObject.FindWithTag("StatsUI").transform.GetChild(0).GetChild(5).transform;
		Transform spd = GameObject.FindWithTag("StatsUI").transform.GetChild(0).GetChild(6).transform;
		Transform str = GameObject.FindWithTag("StatsUI").transform.GetChild(0).GetChild(7).transform;

        progressBars.Add(Labels.Accuracy, acc.GetChild(2).GetChild(0).GetComponent<Transform>());
		progressBars.Add(Labels.Endurance, end.GetChild(2).GetChild(0).GetComponent<Transform>());
		progressBars.Add(Labels.Health, lif.GetChild(2).GetChild(0).GetComponent<Transform>());
		progressBars.Add(Labels.Speed, spd.GetChild(2).GetChild(0).GetComponent<Transform>());
		progressBars.Add(Labels.Strength, str.GetChild(2).GetChild(0).GetComponent<Transform>());

		textLabels.Add(Labels.Accuracy, acc.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>());
		textLabels.Add(Labels.Endurance, end.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>());
		textLabels.Add(Labels.Health, lif.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>());
		textLabels.Add(Labels.Speed, spd.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>());
		textLabels.Add(Labels.Strength, str.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>());

		textLabels.Add(Labels.AccuracyLevel, acc.GetChild(1).GetComponent<TextMeshProUGUI>());
		textLabels.Add(Labels.EnduranceLevel, end.GetChild(1).GetComponent<TextMeshProUGUI>());
		textLabels.Add(Labels.HealthLevel, lif.GetChild(1).GetComponent<TextMeshProUGUI>());
		textLabels.Add(Labels.SpeedLevel, spd.GetChild(1).GetComponent<TextMeshProUGUI>());
		textLabels.Add(Labels.StrengthLevel, str.GetChild(1).GetComponent<TextMeshProUGUI>());
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

		if (jobs[0].Job.Equals(InfoLayerJob.InfoJob.Clear))
        {
            clearInfoLayer();
        }
	}

    private void executeJob()
	{
		clearInfoLayer(jobs[0]);

		Dictionary<Labels, string> tuple = fetchTuple(jobs[0]);
        if (tuple.Keys.Count > 0)
		{
			populateLabels(tuple);
			populateCalendar(jobs[0]);

            if (jobs[0].Job.Equals(InfoLayerJob.InfoJob.Parameters))
			{
				stretchParameterBars(tuple);
			}
		}

		jobs.RemoveAt(0);
	}

	private void stretchParameterBars(Dictionary<Labels, string> tuple)
	{
		float accWidth = (float.Parse(tuple[Labels.Accuracy]) / 999.0f) * 500.0f;
		float endWidth = (float.Parse(tuple[Labels.Endurance]) / 999.0f) * 500.0f;
		float lifWidth = (float.Parse(tuple[Labels.Health]) / 999.0f) * 500.0f;
		float spdWidth = (float.Parse(tuple[Labels.Speed]) / 999.0f) * 500.0f;
		float strWidth = (float.Parse(tuple[Labels.Strength]) / 999.0f) * 500.0f;

		progressBars[Labels.Accuracy].GetComponent<RectTransform>().sizeDelta = new Vector2(accWidth, 30.0f);
		progressBars[Labels.Endurance].GetComponent<RectTransform>().sizeDelta = new Vector2(endWidth, 30.0f);
		progressBars[Labels.Health].GetComponent<RectTransform>().sizeDelta = new Vector2(lifWidth, 30.0f);
		progressBars[Labels.Speed].GetComponent<RectTransform>().sizeDelta = new Vector2(spdWidth, 30.0f);
		progressBars[Labels.Strength].GetComponent<RectTransform>().sizeDelta = new Vector2(strWidth, 30.0f);
	}

	private Dictionary<Labels, string> fetchTuple(InfoLayerJob job)
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
        else if (job.Job.Equals(InfoLayerJob.InfoJob.Parameters))
        {
            tuple = job.JobIndex >= 0 ? InfoLayerTuple.getBoxerTuple(worldData, job.JobIndex) : tuple;
        }

		return tuple;
	}

	private void populateLabels(Dictionary<Labels, string> tuple)
	{
		foreach (Labels l in tuple.Keys)
        {
            populateLabel(l, tuple[l]);
        }
	}

	//protected void populateLabels(InfoLayerJob job)
	//{
	//	if (job.JobIndex >= 0)
	//	{
	//		Dictionary<Labels, string> tuple = new Dictionary<Labels, string>();
 //           if (job.Job.Equals(InfoLayerJob.InfoJob.Region) || job.Job.Equals(InfoLayerJob.InfoJob.RegionPreview))
 //           {
 //               tuple = job.JobIndex >= 0 ? InfoLayerTuple.getRegionTuple(worldData, job.JobIndex) : tuple;
 //           }
	//		else if (job.Job.Equals(InfoLayerJob.InfoJob.Town) || job.Job.Equals(InfoLayerJob.InfoJob.TownPreview))
 //           {
 //               tuple = job.JobIndex >= 0 ? InfoLayerTuple.getTownTuple(worldData, job.JobIndex) : tuple;
 //           }
	//		else if (job.Job.Equals(InfoLayerJob.InfoJob.Parameters))
	//		{
	//			tuple = job.JobIndex >= 0 ? InfoLayerTuple.getBoxerTuple(worldData, job.JobIndex) : tuple;
	//		}

 //           foreach (Labels l in job.Labels)
 //           {
	//			populateLabel(l, tuple[l]);
 //           }

 //           if (job.CalendarVisible)
	//		{
	//			rightInfoPanelCalendar.gameObject.SetActive(true);
	//			populateCalendar(job);
	//		}
	//		else
	//		{
	//			rightInfoPanelCalendar.gameObject.SetActive(false);
	//		}
	//	}

	//	if (job.Job.Equals(InfoLayerJob.InfoJob.Clear))
 //       {
 //           clearInfoLayer();
 //       }
     
	//}

	private void populateCalendar(InfoLayerJob job)
	{
		if (job.CalendarVisible)
		{
			rightInfoPanelCalendar.gameObject.SetActive(true);
			rightInfoPanelCalendar.drawCalendar(WorldDetailProtocol.getRegionTournamentsForMonth(ref worldData, job.JobIndex, 0));
		}
		else
		{
			rightInfoPanelCalendar.gameObject.SetActive(false);
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
