using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exercise {

	private string description;
	private string exerciseName;

	private int accuracy;
	private int endurance;
	private int health;
	private int speed;
	private int strength;

	private int coachIndex;

	private List<int> ageCurve;
	private List<int> coachCurve;
	private List<int> competenceCurve;
	private List<int> growthCurve;

	private int fatigue;

	private bool aiExercise;

	public Exercise(
		string name, string desc, bool ai)
	{
		description = desc;
		exerciseName = name;

		accuracy = 0;
		endurance = 0;
		health = 0;
		speed = 0;
		strength = 0;

		coachIndex = -1;

		aiExercise = ai;

		ageCurve = new List<int> (new int[] {
			0, 0, 2, 5, 2, 0, -3, -5, -7, -7, -8, -9, -10, -10, -14
		});

		growthCurve = new List<int> (new int[]{
			0,0,0,1,1,1,2,2,2,3,3,4,5,5,6,7,8,8,9,10,12,14
		});

		coachCurve = new List<int> (new int[]{
			0,0,0,0,0,0,1,1,1,2,2,2,3,3,4,4,5,5,6,6,6,6
		});

		competenceCurve = new List<int> (new int[]{
			3,3,4,5,5,6,7,7,7,8,8,8,8,8,9,10,11,11,1,12,12,13
		});
	}

	public Exercise(JSONObject json)
	{
		description = json.GetField("description").str;
		exerciseName = json.GetField("name").str;

		accuracy = (int)json.GetField("accuracy").i;
		endurance = (int)json.GetField("endurance").i;
		health = (int)json.GetField("health").i;
		speed = (int)json.GetField("speed").i;
		strength = (int)json.GetField("strength").i;

		coachIndex = (int)json.GetField("coachindex").i;

		aiExercise = json.GetField("ai").b;

		ageCurve = new List<int>(new int[] {
            0, 0, 2, 5, 2, 0, -3, -5, -7, -7, -8, -9, -10, -10, -14
        });

        growthCurve = new List<int>(new int[]{
            0,0,0,1,1,1,2,2,2,3,3,4,5,5,6,7,8,8,9,10,12,14
        });

        coachCurve = new List<int>(new int[]{
            0,0,0,0,0,0,1,1,1,2,2,2,3,3,4,4,5,5,6,6,6,6
        });

        competenceCurve = new List<int>(new int[]{
            3,3,4,5,5,6,7,7,7,8,8,8,8,8,9,10,11,11,1,12,12,13
        });      
	}

	public void addCoach(int newCoachIndex){
		coachIndex = newCoachIndex;
	}

	private static int generateRandomInt(int min, int max)
    {
        return new System.Random((int)System.DateTime.Now.Ticks).Next(min, max);
    }

	private int getAgeBonus(Vector2Int age){
		float lower = ageCurve [age.x];
		float higher = ageCurve [age.x + 1];
		float fraction = (float)age.y / 48;

		return Mathf.RoundToInt(lower + ((higher - lower) * fraction));
	}

    private int getAIBonus()
	{
		if (aiExercise)
		    return generateRandomInt(0, 2);

		return 0;
	}

	private int getGrowthBonus(int growth){
		int place = Mathf.RoundToInt ((growth / 10.0f) * 100.0f);
		place = (place - (place % 5)) / 5;

		return growthCurve[place];
	}

	private int getCoachInput(float stat){
		int place = Mathf.RoundToInt ((stat / 999.0f) * 100.0f);
		place = (place - (place % 5)) / 5;

		return coachCurve[place];
	}

	private int getPlayerCompetence(float stat){
		int place = Mathf.RoundToInt ((stat / 999.0f) * 100.0f);
		place = (place - (place % 5)) / 5;

		return competenceCurve[place];
	}

	private int getLowerBound(int value, TrainingResult.Outcome outcome){
		int remainder = value % 5;
		int multiples = (value - remainder) / 5;
		int mValue = 1;

		if (outcome.Equals (TrainingResult.Outcome.Excellent))
			mValue = 0;
		else if (outcome.Equals (TrainingResult.Outcome.Success))
			mValue = 3;
		else
			mValue = 4;

		int lower = value - (multiples * mValue);

		return lower <= 0 ? 2 : lower;
	}

	private int getUpperBound(int value, TrainingResult.Outcome outcome){
		int remainder = value % 5;
		int multiples = (value - remainder) / 5;
		int mValue = 1;

		if (outcome.Equals (TrainingResult.Outcome.Excellent))
			mValue = 3;
		else if (outcome.Equals (TrainingResult.Outcome.Success))
			mValue = 2;
		else
			mValue = 0;

		return value + (multiples * mValue);
	}

	private TrainingResult.Outcome getOutcome(ref DataPool worldData, int boxerIndex){
		int failFactor = Mathf.RoundToInt ((worldData.Boxers [boxerIndex].Maturity / 100.0f) * 25.0f);

		int outcome = generateRandomInt(0, 100);

		if (outcome <= (25 - failFactor))
			return TrainingResult.Outcome.Failure;
		else if (outcome <= 85)
			return TrainingResult.Outcome.Success;

		return TrainingResult.Outcome.Excellent;
	}

	private TrainingResult getResult(TrainingResult.Outcome outcome, int acc, int end, int hlt, int spd, int str){
		int newAcc = 0; int newEnd = 0; int newHlt = 0; int newSpd = 0; int newStr = 0;

		newAcc = acc > 0 ? generateRandomInt(getLowerBound(acc, outcome), getUpperBound(acc, outcome)) + getAIBonus() : 0;
		newEnd = end > 0 ? generateRandomInt(getLowerBound(end, outcome), getUpperBound(end, outcome)) + getAIBonus() : 0;
		newHlt = hlt > 0 ? generateRandomInt(getLowerBound(hlt, outcome), getUpperBound(hlt, outcome)) + getAIBonus() : 0;
		newSpd = spd > 0 ? generateRandomInt(getLowerBound(spd, outcome), getUpperBound(spd, outcome)) + getAIBonus() : 0;
		newStr = str > 0 ? generateRandomInt(getLowerBound(str, outcome), getUpperBound(str, outcome)) + getAIBonus() : 0;

		return new TrainingResult (outcome, newAcc, newEnd, newHlt, newSpd, newStr, fatigue);
	}

	public void setFactors(int acc, int end, int hlt, int spd, int str, int ftg){
		accuracy = acc;
		endurance = end;
		health = hlt;
		speed = spd;
		strength = str;
		fatigue = ftg;
	}

	public TrainingResult train(ref DataPool worldData, int boxerIndex){
		int accuracyFactor = accuracy > 0 ? accuracy : 0;
		int enduranceFactor = endurance > 0 ? endurance : 0;
		int healthFactor = health > 0 ? health : 0;
		int speedFactor = speed > 0 ? speed : 0;
		int strengthFactor = strength > 0 ? strength : 0;

		accuracyFactor += accuracyFactor > 0 ? getAgeBonus(worldData.Boxers[boxerIndex].Age) : 0;
		enduranceFactor += enduranceFactor > 0 ? getAgeBonus(worldData.Boxers[boxerIndex].Age) : 0;
		healthFactor += healthFactor > 0 ? getAgeBonus(worldData.Boxers[boxerIndex].Age) : 0;
		speedFactor += speedFactor > 0 ? getAgeBonus(worldData.Boxers[boxerIndex].Age) : 0;
		strengthFactor += strengthFactor > 0 ? getAgeBonus(worldData.Boxers[boxerIndex].Age) : 0;

		if (coachIndex >= 0) {
			accuracyFactor += accuracyFactor > 0 ? getCoachInput ((float)worldData.Boxers[coachIndex].Accuracy) : 0;
			enduranceFactor += enduranceFactor > 0 ? getCoachInput ((float)worldData.Boxers[coachIndex].Endurance) : 0;
			healthFactor += healthFactor > 0 ? getCoachInput ((float)worldData.Boxers[coachIndex].Health) : 0;
			speedFactor += speedFactor > 0 ? getCoachInput ((float)worldData.Boxers[coachIndex].Speed) : 0;
			strengthFactor += strengthFactor > 0 ? getCoachInput ((float)worldData.Boxers[coachIndex].Strength) : 0;
		}

		accuracyFactor += accuracyFactor > 0 ? getPlayerCompetence((float)worldData.Boxers[boxerIndex].Accuracy) : 0;
		enduranceFactor += enduranceFactor > 0 ? getPlayerCompetence((float)worldData.Boxers[boxerIndex].Endurance) : 0;
		healthFactor += healthFactor > 0 ? getPlayerCompetence((float)worldData.Boxers[boxerIndex].Health) : 0;
		speedFactor += speedFactor > 0 ? getPlayerCompetence((float)worldData.Boxers[boxerIndex].Speed) : 0;
		strengthFactor += strengthFactor > 0 ? getPlayerCompetence((float)worldData.Boxers[boxerIndex].Strength) : 0;

		accuracyFactor += accuracyFactor > 0 ? getGrowthBonus(worldData.Boxers[boxerIndex].AccuracyGrowth) : 0;
		enduranceFactor += enduranceFactor > 0 ? getGrowthBonus(worldData.Boxers[boxerIndex].EnduranceGrowth) : 0;
		healthFactor += healthFactor > 0 ? getGrowthBonus(worldData.Boxers[boxerIndex].HealthGrowth) : 0;
		speedFactor += speedFactor > 0 ? getGrowthBonus(worldData.Boxers[boxerIndex].SpeedGrowth) : 0;
		strengthFactor += strengthFactor > 0 ? getGrowthBonus(worldData.Boxers[boxerIndex].StrengthGrowth) : 0;

		return getResult (getOutcome (ref worldData, boxerIndex), accuracyFactor, enduranceFactor, healthFactor, speedFactor, strengthFactor);
	}

	public void upgradeExercise(List<int> factors){
		accuracy += factors [0];
		endurance += factors [1];
		health += factors [2];
		speed += factors [3];
		strength += factors [4];
	}
   
    public JSONObject jsonify()
	{
		JSONObject json = new JSONObject();

		json.AddField("description", description);
		json.AddField("name", exerciseName);
		json.AddField("accuracy", accuracy);
		json.AddField("endurance", endurance);
		json.AddField("health", health);
		json.AddField("speed", speed);
		json.AddField("strength", strength);
		json.AddField("coachindex", coachIndex);
		json.AddField("fatigue", fatigue);
		json.AddField("ai", aiExercise);

		JSONObject age = new JSONObject(JSONObject.Type.ARRAY);       
		foreach (int i in ageCurve)
			age.Add(i);

		json.AddField("agecurve", age);

		JSONObject coach = new JSONObject(JSONObject.Type.ARRAY);
		foreach (int i in coachCurve)
			coach.Add(i);

		json.AddField("coachcurve", coach);

		JSONObject competence = new JSONObject(JSONObject.Type.ARRAY);
		foreach (int i in competenceCurve)
			competence.Add(i);

		json.AddField("comptetencecurve", competence);

		JSONObject growth = new JSONObject(JSONObject.Type.ARRAY);
		foreach (int i in growthCurve)
			growth.Add(i);

		json.AddField("growthcurve", growth);

		return json;
	}

	//Getters
	public string Name {
		get { return exerciseName; }
	}

	public string Description {
		get { return description; }
	}
}
