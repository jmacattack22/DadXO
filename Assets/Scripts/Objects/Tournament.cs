using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class Tournament
{
	private CalendarDate date;
	private string name;
	private List<float> prizes;
	private int size;

	private List<int> managerIndexes;
	private Dictionary<int, TournamentResult> tournamentResults;

	private TournamentProtocol.Level level;

	private int currentRound;
	private Dictionary<int, List<Vector2Int>> schedule;

	private bool priority;
	private bool quarterly;

	public Tournament(string name, CalendarDate dt, float topPrize, int size, TournamentProtocol.Level lvl, bool qtr)
	{
		managerIndexes = new List<int>();
        this.name = name;
        prizes = new List<float>();
        tournamentResults = new Dictionary<int, TournamentResult>();
        schedule = new Dictionary<int, List<Vector2Int>>();

        date = dt;

        prizes.Add(topPrize);
        prizes.Add(topPrize / 2.0f);
        prizes.Add(topPrize / 4.0f);
        prizes.Add(0.0f);

        this.size = size;

        level = lvl;

        currentRound = 0;
        priority = false;
        quarterly = qtr;
	}

    public Tournament(JSONObject json)
	{
		date = new CalendarDate(json.GetField("date"));
		name = json.GetField("name").str;
		size = (int)json.GetField("size").i;
		currentRound = (int)json.GetField("round").i;
		priority = json.GetField("priority");
		quarterly = json.GetField("quarterly");
        
		level = (TournamentProtocol.Level)Enum.Parse(typeof(TournamentProtocol.Level), json.GetField("level").str);

		prizes = new List<float>();
        foreach (JSONObject p in json.GetField("prizes").list)
		{
			prizes.Add(p.f);
		}

		managerIndexes = new List<int>();
        foreach (JSONObject m in json.GetField("managerindexes").list)
		{
			managerIndexes.Add((int)m.i);
		}

		tournamentResults = new Dictionary<int, TournamentResult>();
        foreach (JSONObject record in json.GetField("results").list)
		{
			tournamentResults.Add((int)record.GetField("key").i, new TournamentResult(record.GetField("value")));
		}

		scheduleTournament();
	}

	public void addContestant(int index)
    {
        if (managerIndexes.Count < size)
        {
            if (!managerIndexes.Contains(index))
            {
                managerIndexes.Add(index);
                tournamentResults.Add(index, new TournamentResult());
            }
        }
    }

	private int countPoints(Record record)
    {
        return (record.Wins * 2) + record.Ties;
    }

	public string getDetails()
    {
        return name + " - " + level.ToString() + " - " + prizes[0] + " - " + date + " - " + managerIndexes.Count + "/" + size;
    }

    public List<int> getManagerIndexes()
	{
		return managerIndexes;
	}

	public Dictionary<int, List<Vector2Int>> getSchedule()
    {
        return schedule;
    }

	public Dictionary<int, TournamentResult> getTournamentResults()
	{
		return tournamentResults;
	}

	public void logResults(ref DataPool worldData, int townIndex)
    {
        managerIndexes = managerIndexes.OrderByDescending(x => tournamentResults[x].Record.getWinPercentage()).ToList();
        foreach (int index in managerIndexes)
        {
            Debug.Log(worldData.Managers[index].getManagerStats() + " - " + tournamentResults[index].Record.getWinPercentage());
        }
    }

	public void rankResults()
	{
		managerIndexes = managerIndexes.OrderByDescending(i => countPoints(tournamentResults[i].Record)).ToList();
	}

	public void refreshTournament(bool quarterly)
    {
        managerIndexes.Clear();
        tournamentResults.Clear();
        priority = false;
        currentRound = 0;
        schedule.Clear();

        if (quarterly)
            date.advanceQuarter();
        else
            date.advanceYear();
    }

	public void setDate(CalendarDate newDate)
    {
        date = newDate;
    }

    public void scheduleTournament()
    {
        schedule = TournamentScheduler.generateTournamentSchedule(managerIndexes.Count);
    }

	public bool spaceLeft()
    {
        return managerIndexes.Count < size ? true : false;
    }

    public void togglePriority()
    {
        priority = !priority;
    }

    public JSONObject jsonify()
	{
		JSONObject json = new JSONObject(JSONObject.Type.OBJECT);

		json.AddField("date", date.jsonify());
		json.AddField("name", name);
		json.AddField("size", size);
		json.AddField("level", level.ToString());
		json.AddField("round", currentRound);
		json.AddField("priority", priority);
		json.AddField("quarterly", quarterly);

		JSONObject money = new JSONObject(JSONObject.Type.ARRAY);
        foreach (float p in prizes)
		{
			money.Add(p);
		}
		json.AddField("prizes", money);

		JSONObject mgr = new JSONObject(JSONObject.Type.ARRAY);      
        foreach (int i in managerIndexes)
		{
			mgr.Add(i);
		}      
		json.AddField("managerindexes", mgr);

		JSONObject results = new JSONObject(JSONObject.Type.ARRAY);      
        foreach (int key in tournamentResults.Keys)
		{
			JSONObject record = new JSONObject(JSONObject.Type.OBJECT);

			record.AddField("key", key);
			record.AddField("value", tournamentResults[key].jsonify());
		}
		json.AddField("results", results);

		JSONObject sched = new JSONObject(JSONObject.Type.ARRAY);
        foreach (int key in schedule.Keys)
		{
			JSONObject record = new JSONObject(JSONObject.Type.OBJECT);

			record.AddField("key", key);

			JSONObject round = new JSONObject(JSONObject.Type.ARRAY);
			foreach (Vector2Int match in schedule[key])
			{
				round.Add(JSONTemplates.FromVector2Int(match));
			}
			record.AddField("value", round);
		}
		json.AddField("schedule", sched);      

		return json;
	}

    //Getters
    public CalendarDate TournamentDate 
	{
		get { return date; }
	}

    public string Name 
	{
		get { return name; }
	}

    public int Size 
	{
		get { return size; }
	}

    public TournamentProtocol.Level Level
	{
		get { return level; }
	}

    public int Round
	{
		get { return currentRound; }
	}

    public bool Priority
	{
		get { return priority; }
	}

    public bool Quarterly
	{
		get { return quarterly; }
	}

	public int Attendees
    {
        get { return managerIndexes.Count; }
    }

	public float TopPrize
    {
        get { return prizes[0]; }
    }
}
