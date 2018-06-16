using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TournamentCalendarBehaviour : MonoBehaviour {

	private Dictionary<TournamentProtocol.Level, List<GameObject>> weekNodes;

	void Awake () {
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
					weekNodes[level][i-1].GetComponent<Image>().color = new Color32(220, 100, 100, 100);
					//weekNodes[level][i-1].GetComponent<WeekNode>().setupNode(level, 0, regionTournaments[level][i].Count);
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
				weekNodes[level][i].GetComponent<Image>().color = new Color(100, 100, 100, 100);
			}
		}
	}

    private void loadContent()
	{
		weekNodes.Add(TournamentProtocol.Level.S, new List<GameObject>());
		weekNodes[TournamentProtocol.Level.S].Add(transform.GetChild(4).gameObject);
		weekNodes[TournamentProtocol.Level.S].Add(transform.GetChild(5).gameObject);
		weekNodes[TournamentProtocol.Level.S].Add(transform.GetChild(6).gameObject);
		weekNodes[TournamentProtocol.Level.S].Add(transform.GetChild(7).gameObject);
        
		weekNodes.Add(TournamentProtocol.Level.A, new List<GameObject>());
        weekNodes[TournamentProtocol.Level.A].Add(transform.GetChild(8).gameObject);
        weekNodes[TournamentProtocol.Level.A].Add(transform.GetChild(9).gameObject);
        weekNodes[TournamentProtocol.Level.A].Add(transform.GetChild(10).gameObject);
        weekNodes[TournamentProtocol.Level.A].Add(transform.GetChild(11).gameObject);

		weekNodes.Add(TournamentProtocol.Level.B, new List<GameObject>());
        weekNodes[TournamentProtocol.Level.B].Add(transform.GetChild(12).gameObject);
        weekNodes[TournamentProtocol.Level.B].Add(transform.GetChild(13).gameObject);
        weekNodes[TournamentProtocol.Level.B].Add(transform.GetChild(14).gameObject);
        weekNodes[TournamentProtocol.Level.B].Add(transform.GetChild(15).gameObject);

		weekNodes.Add(TournamentProtocol.Level.C, new List<GameObject>());
        weekNodes[TournamentProtocol.Level.C].Add(transform.GetChild(16).gameObject);
        weekNodes[TournamentProtocol.Level.C].Add(transform.GetChild(17).gameObject);
        weekNodes[TournamentProtocol.Level.C].Add(transform.GetChild(18).gameObject);
        weekNodes[TournamentProtocol.Level.C].Add(transform.GetChild(19).gameObject);

		weekNodes.Add(TournamentProtocol.Level.D, new List<GameObject>());
        weekNodes[TournamentProtocol.Level.D].Add(transform.GetChild(20).gameObject);
        weekNodes[TournamentProtocol.Level.D].Add(transform.GetChild(21).gameObject);
        weekNodes[TournamentProtocol.Level.D].Add(transform.GetChild(22).gameObject);
        weekNodes[TournamentProtocol.Level.D].Add(transform.GetChild(23).gameObject);

		weekNodes.Add(TournamentProtocol.Level.E, new List<GameObject>());
        weekNodes[TournamentProtocol.Level.E].Add(transform.GetChild(24).gameObject);
        weekNodes[TournamentProtocol.Level.E].Add(transform.GetChild(25).gameObject);
        weekNodes[TournamentProtocol.Level.E].Add(transform.GetChild(26).gameObject);
        weekNodes[TournamentProtocol.Level.E].Add(transform.GetChild(27).gameObject);
	}

}
