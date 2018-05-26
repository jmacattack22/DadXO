using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Manager : Person {

	private BoxerClass.Type preference;
      
    private int boxerIndex;

    private bool atSea;
    private bool atTournament;

    private FinanceProtocol finance;

    private Homebase homebase;
    private Ship ship;

    private List<ManagerProtocol.FacilityShortcut> trainingRegime;

    private float currentBoxerELO;
    private float currentManagerELO;
    private List<float> archivedBoxerELO;
    private List<float> archivedManagerELO;

	private Record record; 
    
    public List<int> previousOpponents;

    private int tournamentCount;
    private int tournamentPriority;

    private TournamentProtocol.Level currentRanking;
    
	public Manager(
		string fName, string lName, int townId, float wt, BoxerClass.Type pref
	) : base(new Vector2Int(1,1), fName, lName, townId, wt)
	{
		preference = pref;

		record = new Record (40.0f);

		finance = new FinanceProtocol(5000);

		atSea = false;
        atTournament = false;

        archivedBoxerELO = new List<float>();
        archivedManagerELO = new List<float>();

        previousOpponents = new List<int>();

        tournamentCount = 0;
        tournamentPriority = 0;
	}

    public Manager(JSONObject json)
		: base(JSONTemplates.ToVector2Int(json.GetField("age")), json.GetField("firstname").str, json.GetField("lastname").str, 
		       (int)json.GetField("townindex").i, (int)json.GetField("weight").i)
	{
		preference = (BoxerClass.Type)Enum.Parse(typeof(BoxerClass.Type), json.GetField("preference").str);

		record = new Record(json.GetField("record"));

		atSea = json.GetField("atsea").b;
		atTournament = json.GetField("attournament").b;

		homebase = new Homebase(json.GetField("homebase"));
		ship = new Ship(json.GetField("ship"));
		finance = new FinanceProtocol(json.GetField("finance"));
        
		currentBoxerELO = json.GetField("boxerelo").f;
		currentManagerELO = json.GetField("managerelo").f;

		tournamentCount = (int)json.GetField("tournamentsattended").i;
		tournamentPriority = (int)json.GetField("priority").i;
		currentRanking = (TournamentProtocol.Level)Enum.Parse(typeof(TournamentProtocol.Level), json.GetField("rank").str);

		trainingRegime = new List<ManagerProtocol.FacilityShortcut>();
        foreach (JSONObject r in json.GetField("trainingregime").list)
		{
			trainingRegime.Add((ManagerProtocol.FacilityShortcut)Enum.Parse(typeof(ManagerProtocol.FacilityShortcut), r.str));
		}

		archivedBoxerELO = new List<float>();
        foreach (JSONObject r in json.GetField("archivedboxerelo").list)
		{
			archivedBoxerELO.Add(r.f);
		}

		archivedManagerELO = new List<float>();
        foreach (JSONObject r in json.GetField("archivedmanagerelo").list)
		{
			archivedManagerELO.Add(r.f);
		}

		previousOpponents = new List<int>();
        foreach (JSONObject r in json.GetField("previousopponents").list)
		{
			previousOpponents.Add((int)r.i);
		}
	}   

    public void archiveBoxerELO()
	{
		archivedBoxerELO.Add(currentBoxerELO);
	}

    public void archiveManagerELO()
	{
		archivedManagerELO.Add(currentManagerELO);
	}

	public void attendTournament()
    {
        atTournament = true;
		tournamentCount++;
        tournamentPriority = tournamentPriority - 3;// < 0 ? 0 : tournamentPriority - 2;
    }

    public void backOutOfTournament()
    {
		tournamentCount--;
        tournamentPriority += 3;
        atTournament = false;
    }

    public void bumpTournamentPriority()
    {
        tournamentPriority += 1;
    }

	public ManagerProtocol.FacilityShortcut chooseTraining()
    {
        return trainingRegime[generateRandomInt(0, trainingRegime.Count - 1)];
    }

	private static int generateRandomInt(int min, int max)
    {
        return new System.Random((int)System.DateTime.Now.Ticks).Next(min, max);
    }

    public string getDetails(){
        return FirstName + " " + LastName + ", W " + record.Wins + " L " + record.Losses + ", Preference " + preference.ToString();
    }

	public float getBoxerELOHistory()
    {
        return archivedBoxerELO[0];
    }

    public float getManagerELOHistory()
    {
        return archivedManagerELO[0];
    }

    public string getManagerStats()
    {
        return currentManagerELO + " - " + currentRanking + " - " + tournamentPriority + " - " + getDetails();
    }

    public void graduateRank()
	{
		currentRanking = (TournamentProtocol.Level)(((int)currentRanking) + 1);
	}

    public bool isBusy()
	{
		return atTournament;
	}

    public void leaveTournament()
	{
		atTournament = false;
	}

	public void logManagerStats(){
        Debug.Log(currentManagerELO + " - " + currentRanking + " - " + tournamentPriority + " - " + getDetails() + " - " + tournamentCount);
    }

	public void recruitBoxer(int bIndex)
    {
        boxerIndex = bIndex;
    }

    public void setBoxerELO(float elo)
	{
		currentBoxerELO = elo;
	}

    public void setManagerELO(float elo)
	{
		currentManagerELO = elo;
	}

    public void setRank(TournamentProtocol.Level rank)
    {
        currentRanking = rank;
    }   

	public void setupHomebase(ref DataPool worldData, bool ai)
	{
		homebase = new Homebase(ref worldData, ai);
        ship = new Ship(ref worldData);
        trainingRegime = new List<ManagerProtocol.FacilityShortcut>();
		setupTrainingRegime();
	}

	private void setupTrainingRegime()
    {
        trainingRegime.Add(ManagerProtocol.FacilityShortcut.DoubleEndBag);
		trainingRegime.Add(ManagerProtocol.FacilityShortcut.Laps);
		trainingRegime.Add(ManagerProtocol.FacilityShortcut.PunchGlove);
		trainingRegime.Add(ManagerProtocol.FacilityShortcut.PunchingBag);
		trainingRegime.Add(ManagerProtocol.FacilityShortcut.Sprints);

        List<EvaluationProtocol.Stats> bestStats = BoxerClass.getBuild(preference);

        foreach (EvaluationProtocol.Stats stat in bestStats)
        {
			ManagerProtocol.FacilityShortcut shortcut = ManagerProtocol.FacilityShortcut.DoubleEndBag;

            if (stat.Equals(EvaluationProtocol.Stats.AccuracyGrowth))
				shortcut = ManagerProtocol.FacilityShortcut.DoubleEndBag;
            else if (stat.Equals(EvaluationProtocol.Stats.EnduranceGrowth))
				shortcut = ManagerProtocol.FacilityShortcut.PunchGlove;
            else if (stat.Equals(EvaluationProtocol.Stats.HealthGrowth))
				shortcut = ManagerProtocol.FacilityShortcut.Laps;
            else if (stat.Equals(EvaluationProtocol.Stats.SpeedGrowth))
				shortcut = ManagerProtocol.FacilityShortcut.Sprints;
            else if (stat.Equals(EvaluationProtocol.Stats.StrengthGrowth))
				shortcut = ManagerProtocol.FacilityShortcut.PunchingBag;

            for (int i = 0; i < 2; i++)
            {
                trainingRegime.Add(shortcut);
            }
        }
    }

	public void train(ref DataPool worldData)
    {
		ManagerProtocol.FacilityShortcut training = chooseTraining();

        if (atSea)
            ship.train(ref worldData, boxerIndex, training);
        else
            homebase.train(ref worldData, boxerIndex, training);
    }

	public void upgradeFacilities(ref DataPool worldData)
    {
        homebase.upgradeFacilities(ref worldData, currentManagerELO, preference);
    }

	public JSONObject jsonify()
    {
        JSONObject json = new JSONObject(JSONObject.Type.OBJECT);

        json.AddField("weightclass", Class.ToString());

        json.AddField("age", JSONTemplates.FromVector2Int(Age));
        json.AddField("firstname", FirstName);
        json.AddField("lastname", LastName);
        json.AddField("townindex", TownIndex);
        json.AddField("weight", Weight);
        json.AddField("weeksremaining", WeeksRemaining);

		json.AddField("preference", preference.ToString());
		json.AddField("record", record.jsonify());

		json.AddField("atsea", atSea);
		json.AddField("attournament", atTournament);
		json.AddField("homebase", homebase.jsonify());
		json.AddField("ship", ship.jsonify());
		json.AddField("finance", finance.jsonify());

		json.AddField("boxerelo", currentBoxerELO);
		json.AddField("managerelo", currentManagerELO);

		json.AddField("record", record.jsonify());
		json.AddField("tournamentsattended", tournamentCount);
		json.AddField("priority", tournamentPriority);
		json.AddField("rank", currentRanking.ToString());

		JSONObject regime = new JSONObject(JSONObject.Type.ARRAY);
        foreach (ManagerProtocol.FacilityShortcut facility in trainingRegime)
		{
			regime.Add(facility.ToString());
		}
		json.AddField("trainingregime", regime);

		JSONObject archivedBoxer = new JSONObject(JSONObject.Type.ARRAY);
        foreach (float elo in archivedBoxerELO)
		{
			archivedBoxer.Add(elo);
		}
		json.AddField("archivedboxerelo", archivedBoxer);

		JSONObject archivedManager = new JSONObject(JSONObject.Type.ARRAY);
        foreach (float elo in archivedManagerELO)
        {
            archivedManager.Add(elo);
        }
        json.AddField("archivedmanagerelo", archivedManager);

		JSONObject prevOpp = new JSONObject(JSONObject.Type.ARRAY);
        foreach (int index in previousOpponents)
		{
			prevOpp.Add(index);
		}
		json.AddField("previousopponents", prevOpp);

        return json;
    }
          
	//Getters
	public BoxerClass.Type Preference {
		get { return preference; }
	}

	public Record Record {
		get { return record; }
	}

    public bool AtSea 
	{
		get { return atSea; }
	}

    public bool AtTournament 
	{
		get { return atTournament; }
	}

    public FinanceProtocol Finance
	{
		get { return finance; }
	}

    public int BoxerIndex
	{
		get { return boxerIndex; }
	}

    public float BoxerELO 
	{
		get { return currentBoxerELO; }
	}

    public float ManagerELO
	{
		get { return currentManagerELO; }
	}

    public TournamentProtocol.Level Rank
	{
		get { return currentRanking; }
	}

    public int TournamentsAttended
	{
		get { return tournamentCount; }
	}

    public int Priority
	{
		get { return tournamentPriority; }
	}
}
