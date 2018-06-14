using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TournamentCalendarBehaviour : MonoBehaviour {

	public GameObject weekNode;
   
	private Dictionary<TournamentProtocol.Level, List<GameObject>> weekNodes;

	void Start () {
		weekNodes = new Dictionary<TournamentProtocol.Level, List<GameObject>>();
		loadContent();
	}

	public void drawCalendar(Dictionary<TournamentProtocol.Level, Dictionary<int, List<int>>> regionTournaments)
	{
		cleanUpCalendar();

        foreach (TournamentProtocol.Level level in regionTournaments.Keys)
		{
			for (int i = 1; i <= 4; i++)
			{
				if (regionTournaments[level][i].Count > 0)
				{
					weekNodes[level][i-1].SetActive(true);
					weekNodes[level][i-1].transform.GetChild(0).GetComponent<WeekNode>().setupNode(level, 0, regionTournaments[level][i].Count);
					//weekNodes[level][i - 1].GetComponent<WeekNode>().setupNode(level, 0, regionTournaments[level][i].Count);
				}
			}
		}
	}

	private void cleanUpCalendar()
	{
		foreach (TournamentProtocol.Level level in TournamentProtocol.getLevels())
		{
			for (int i = 0; i < 4; i++)
			{
				weekNodes[level][i].SetActive(false);
			}
		}
	}

    private void loadContent()
	{
		weekNodes.Add(TournamentProtocol.Level.S, new List<GameObject>());
		weekNodes[TournamentProtocol.Level.S].Add(transform.GetChild(1).GetChild(0).gameObject);
		weekNodes[TournamentProtocol.Level.S].Add(transform.GetChild(1).GetChild(1).gameObject);
		weekNodes[TournamentProtocol.Level.S].Add(transform.GetChild(1).GetChild(2).gameObject);
		weekNodes[TournamentProtocol.Level.S].Add(transform.GetChild(1).GetChild(3).gameObject);
        
		weekNodes.Add(TournamentProtocol.Level.A, new List<GameObject>());
        weekNodes[TournamentProtocol.Level.A].Add(transform.GetChild(2).GetChild(0).gameObject);
        weekNodes[TournamentProtocol.Level.A].Add(transform.GetChild(2).GetChild(1).gameObject);
        weekNodes[TournamentProtocol.Level.A].Add(transform.GetChild(2).GetChild(2).gameObject);
        weekNodes[TournamentProtocol.Level.A].Add(transform.GetChild(2).GetChild(3).gameObject);

		weekNodes.Add(TournamentProtocol.Level.B, new List<GameObject>());
        weekNodes[TournamentProtocol.Level.B].Add(transform.GetChild(3).GetChild(0).gameObject);
        weekNodes[TournamentProtocol.Level.B].Add(transform.GetChild(3).GetChild(1).gameObject);
        weekNodes[TournamentProtocol.Level.B].Add(transform.GetChild(3).GetChild(2).gameObject);
        weekNodes[TournamentProtocol.Level.B].Add(transform.GetChild(3).GetChild(3).gameObject);

		weekNodes.Add(TournamentProtocol.Level.C, new List<GameObject>());
        weekNodes[TournamentProtocol.Level.C].Add(transform.GetChild(4).GetChild(0).gameObject);
        weekNodes[TournamentProtocol.Level.C].Add(transform.GetChild(4).GetChild(1).gameObject);
        weekNodes[TournamentProtocol.Level.C].Add(transform.GetChild(4).GetChild(2).gameObject);
        weekNodes[TournamentProtocol.Level.C].Add(transform.GetChild(4).GetChild(3).gameObject);

		weekNodes.Add(TournamentProtocol.Level.D, new List<GameObject>());
        weekNodes[TournamentProtocol.Level.D].Add(transform.GetChild(5).GetChild(0).gameObject);
        weekNodes[TournamentProtocol.Level.D].Add(transform.GetChild(5).GetChild(1).gameObject);
        weekNodes[TournamentProtocol.Level.D].Add(transform.GetChild(5).GetChild(2).gameObject);
        weekNodes[TournamentProtocol.Level.D].Add(transform.GetChild(5).GetChild(3).gameObject);

		weekNodes.Add(TournamentProtocol.Level.E, new List<GameObject>());
        weekNodes[TournamentProtocol.Level.E].Add(transform.GetChild(6).GetChild(0).gameObject);
        weekNodes[TournamentProtocol.Level.E].Add(transform.GetChild(6).GetChild(1).gameObject);
        weekNodes[TournamentProtocol.Level.E].Add(transform.GetChild(6).GetChild(2).gameObject);
        weekNodes[TournamentProtocol.Level.E].Add(transform.GetChild(6).GetChild(3).gameObject);
	}

}
