using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Boxer : Person {

	private BoxerClass.Type boxerClass;

	private int accuracy;
	private int endurance;
	private int health;
	private int speed;
	private int strength;

	private int accuracyGrowth;
	private int enduranceGrowth;
	private int healthGrowth;
	private int speedGrowth;
	private int strengthGrowth;

	private Record record;

	private bool retired;

	private int concussions;
	private int fatigue;
	private int maturity;
	private int stress;

	public Boxer(
		Vector2Int age, string fName, string lName, int townId, float wt, BoxerClass.Type bClass,
		int acc, int accG, int end, int endG, int hlt, int hltG, int spd, int spdG, int str, int strG
	) : base(age, fName, lName, townId, wt){

		boxerClass = bClass;

		accuracy = acc;
		endurance = end;
		health = hlt;
		speed = spd;
		strength = str;

		accuracyGrowth = accG;
		enduranceGrowth = endG;
		healthGrowth = hltG;
		speedGrowth = spdG;
		strengthGrowth = strG;

		retired = false;

		record = new Record (120.0f);
		record.setELO (800.0f);

		concussions = 0;
		fatigue = 0;
		maturity = 0;
		stress = 0;
	}

	public Boxer(JSONObject json) : 
	base(
		JSONTemplates.ToVector2Int(json.GetField("age")), json.GetField("firstname").str, json.GetField("lastname").str,
		townId: (int)json.GetField("townindex").i, wt: json.GetField("weight").f
	)
	{
		boxerClass = global::BoxerClass.getTypeFromJson(json.GetField("weightclass"));

		accuracy = (int)json.GetField("accuracy").i;
		endurance = (int)json.GetField("endurance").i;
		health = (int)json.GetField("health").i;
		speed = (int)json.GetField("speed").i;
		strength = (int)json.GetField("strength").i;

		accuracyGrowth = (int)json.GetField("accuracygrowth").i;
		enduranceGrowth = (int)json.GetField("endurancegrowth").i;
		healthGrowth = (int)json.GetField("healthgrowth").i;
		speedGrowth = (int)json.GetField("speedgrowth").i;
		strengthGrowth = (int)json.GetField("strengthgrowth").i;

		record = new Record(json.GetField("record"));

		retired = json.GetField("retired").b;

		concussions = (int)json.GetField("concussions").i;
		fatigue = (int)json.GetField("fatigue").i;
		maturity = (int)json.GetField("maturity").i;
		stress = (int)json.GetField("stress").i;
	}

    public void applyTrainingResults(TrainingResult results){
		accuracy += accuracy + results.Accuracy > 999 ? 0 : results.Accuracy;
		endurance += endurance + results.Endurance > 999 ? 0 : results.Endurance;
		health += health + results.Health > 999 ? 0 : results.Health;
		speed += speed + results.Speed > 999 ? 0 : results.Speed;
		strength += strength + results.Strength > 999 ? 0 : results.Strength;

		fatigue += results.Fatigue;

		if (results.Result.Equals (TrainingResult.Outcome.Failure))
			maturity++;
	}

	public float getOverall(){
		return (float)((accuracy + endurance + health + speed + strength) / 5.0f);
	}

	public bool isBoxerFatigued(){
		if (fatigue >= 80)
			return true;

		return false;
	}

	public void logBoxerStats(){
		Debug.Log ("Acc " + accuracy + ", End " + endurance + ", Hlt " + health + ", spd " + speed + ", str " + strength + ", ftg " + fatigue + 
			"AccG " + accuracyGrowth + ", EndG " + enduranceGrowth + ", HltG " + healthGrowth + ", SpdG " + speedGrowth + ", StrG " + strengthGrowth + 
			", Class " + boxerClass.ToString() + ", WR " + WeeksRemaining
		);
	}

    public void logBoxerStats(TournamentProtocol.Level boxerLevel){
        Debug.Log("Acc " + accuracy + ", End " + endurance + ", Hlt " + health + ", spd " + speed + ", str " + strength + ", ftg " + fatigue +
            "AccG " + accuracyGrowth + ", EndG " + enduranceGrowth + ", HltG " + healthGrowth + ", SpdG " + speedGrowth + ", StrG " + strengthGrowth +
                  ", Class " + boxerClass.ToString() + ", WR " + WeeksRemaining + ", " + boxerLevel.ToString() + ", W " + record.Wins + " L " + record.Losses
        );
    }

    public void modifyStat(EvaluationProtocol.Stats stat, int value){
        if (stat.Equals(EvaluationProtocol.Stats.Accuracy))
            accuracy = value;
        else if (stat.Equals(EvaluationProtocol.Stats.Endurance))
            endurance = value;
        else if (stat.Equals(EvaluationProtocol.Stats.Health))
            health = value;
        else if (stat.Equals(EvaluationProtocol.Stats.Speed))
            speed = value;
        else if (stat.Equals(EvaluationProtocol.Stats.Strength))
            strength = value;
    }

	public void rest(){
		fatigue -= fatigue - 80 < 0 ? 0 : 80;
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

		json.AddField("boxerclass", boxerClass.ToString());
		json.AddField("accuracy", accuracy);
		json.AddField("endurance", endurance);
		json.AddField("health", health);
		json.AddField("speed", speed);
		json.AddField("strength", strength);
		json.AddField("accuracygrowth", accuracyGrowth);
		json.AddField("endurancegrowth", enduranceGrowth);
		json.AddField("healthgrowth", healthGrowth);
		json.AddField("speedgrowth", speedGrowth);
		json.AddField("strengthgrowth", strengthGrowth);
		json.AddField("record", record.jsonify());
		json.AddField("retired", retired);
		json.AddField("concussions", concussions);
		json.AddField("fatigue", fatigue);
		json.AddField("maturity", maturity);
		json.AddField("stress", stress);      

		return json;
	}
       
	//Getters
	public BoxerClass.Type BoxerClass {
		get { return boxerClass; }
	}

	public int Accuracy {
		get { return accuracy; }
	}

	public int Endurance { 
		get { return endurance; }
	}

	public int Health {
		get { return health; }
	}

	public int Speed {
		get { return speed; }
	}

	public int Strength {
		get { return strength; }
	}

	public int AccuracyGrowth {
		get { return accuracyGrowth; }
	}

	public int EnduranceGrowth {
		get { return enduranceGrowth; }
	}

	public int HealthGrowth {
		get { return healthGrowth; }
	}

	public int SpeedGrowth {
		get { return speedGrowth; }
	}

	public int StrengthGrowth {
		get { return strengthGrowth; }
	}

	public int Concussions {
		get { return concussions; }
	}

	public int Fatigue {
		get { return fatigue; }
	}

	public int Maturity {
		get { return maturity; }
	}

	public int Stress {
		get { return stress; }
	}

	public Record Record {
		get { return record; }
	}

	public bool Retired {
		get { return retired; }
	}
}
