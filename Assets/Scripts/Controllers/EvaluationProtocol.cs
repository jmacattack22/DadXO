using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class EvaluationProtocol
{
	public enum Stats
	{
		Accuracy, Endurance, Health, Speed, Strength, AccuracyGrowth, EnduranceGrowth, HealthGrowth, SpeedGrowth, StrengthGrowth
	}

	private static List<float> statValueCurve = new List<float> (new float[] {
		0.1f, 0.1f, 0.2f, 0.2f, 0.3f, 0.3f, 0.3f, 0.35f, 0.35f, 0.45f, 0.45f, 0.55f, 0.55f, 0.6f, 0.65f, 0.67f, 0.78f, 0.86f, 0.96f, 1.0f, 1.1f, 1.1f, 1.1f, 1.1f, 1.1f
	});

	private static List<float> growthRateCurve = new List<float> (new float[] {
		8, 8, 9, 9, 9, 10, 10, 10, 11, 11, 11, 12, 12, 12, 14, 16, 18, 19, 21, 23, 23, 23, 25, 26, 27, 29, 30, 30, 30, 30
	});

	private static List<int> statValues = new List<int> (new int[] {
		5, 25, 35, 40, 45, 55, 64, 74, 90, 110, 135
	});

	private static List<float> facilityPointCurve = new List<float> (new float[] {
		2, 2, 3, 3, 3, 3, 4, 4, 4, 5, 5, 5, 6, 6, 7, 8, 9, 10, 11, 12, 13, 13, 14, 14, 15, 15, 16, 16, 16, 16, 16
	});

    public static float evaluateBoxer(Boxer boxer)
    {
        Dictionary<Stats, int> bestStats = identifyBestStats(boxer);

        float average = (float)(bestStats.Values.Sum() / 3.0f);

        int lower = Mathf.RoundToInt((average - (average % 5.0f)) / 5.0f);
        int higher = Mathf.RoundToInt((average - (average % 5.0f)) / 5.0f) + 1;

        lower = lower < 0 ? 0 : lower;
        higher = higher < 0 ? 0 : higher;

        float statValueLower = statValueCurve[lower];
        float statValueHigher = statValueCurve[higher];

        float distance = (average % 5.0f) / 5.0f;

        float overallBooster = boxer.getOverall() / 990.0f;
        float recordBooster = boxer.Record.getWinPercentage() * 0.2f;

        return ((statValueLower * distance) + (statValueHigher * (1.0f - distance))) + (overallBooster + recordBooster);
    }

	public static float evaluateBoxer(Boxer boxer, BoxerClass.Type type){
		Dictionary<Stats, int> bestStats = identifyBestStats (boxer);

		float average = (float)(bestStats.Values.Sum () / 3.0f);

		int lower = Mathf.RoundToInt ((average - (average % 5.0f)) / 5.0f);
		int higher = Mathf.RoundToInt ((average - (average % 5.0f)) / 5.0f) + 1;

		lower = lower < 0 ? 0 : lower;
		higher = higher < 0 ? 0 : higher;

		float statValueLower = statValueCurve [lower];
		float statValueHigher = statValueCurve [higher];

		float distance = (average % 5.0f) / 5.0f;

		float preferenceBooster = (boxer.BoxerClass == type) ? 0.1f : 0.0f;
		float overallBooster = boxer.getOverall () / 990.0f;

		return ((statValueLower * distance) + (statValueHigher * (1.0f - distance))) + (preferenceBooster + overallBooster);
	}

	public static int getStatValueFromGrowthRate(int rate)
	{
		return 40 + generateRandomInt(statValues[rate - 1] - 5, statValues[rate - 1] + 5);
	}

    public static EvaluationProtocol.Stats getStatFromJson(JSONObject json)
	{
		string stat = json.str;

        if (stat.Equals("Accuracy"))
		{
			return Stats.Accuracy;
		}
		else if (stat.Equals("Endurance"))
		{
			return Stats.Endurance;
		}
		else if (stat.Equals("Health"))
		{
			return Stats.Health;
		}
		else if (stat.Equals("Speed"))
		{
			return Stats.Speed;
		}
		else if (stat.Equals("Strength"))
		{
			return Stats.Strength;
		}

		return Stats.Accuracy;

	}

	private static int generateRandomInt(int min, int max)
    {
        return new System.Random((int)System.DateTime.Now.Ticks).Next(min, max);
    }

	public static int getBoxerPointsFromFame(float elo){
		float fame = (elo / 2500.0f) * 1000.0f;

		float statValueLower = growthRateCurve [Mathf.RoundToInt ((fame - (fame % 50.0f)) / 50.0f)];
		float statValueHigher = growthRateCurve [Mathf.RoundToInt ((fame - (fame % 50.0f)) / 50.0f) + 1];

		float distance = (fame % 50.0f) / 50.0f;

		return Mathf.RoundToInt((statValueLower * distance) + (statValueHigher * (1.0f - distance)));
	}

	public static int getFacilityPointsFromElo(float elo){
		float fame = (elo / 2500.0f) * 1000.0f;

		float statValueLower = facilityPointCurve [Mathf.RoundToInt ((fame - (fame % 50.0f)) / 50.0f)];
		float statValueHigher = facilityPointCurve [Mathf.RoundToInt ((fame - (fame % 50.0f)) / 50.0f) + 1];

		float distance = (fame % 50.0f) / 50.0f;

		return Mathf.RoundToInt((statValueLower * distance) + (statValueHigher * (1.0f - distance)));
	}

	private static int getStat(Stats stat, Boxer boxer){
		if (stat.Equals (Stats.AccuracyGrowth))
			return boxer.AccuracyGrowth;
		else if (stat.Equals (Stats.EnduranceGrowth))
			return boxer.EnduranceGrowth;
		else if (stat.Equals (Stats.HealthGrowth))
			return boxer.HealthGrowth;
		else if (stat.Equals (Stats.SpeedGrowth))
			return boxer.SpeedGrowth;
		else if (stat.Equals (Stats.StrengthGrowth))
			return boxer.StrengthGrowth;
		else if (stat.Equals (Stats.Accuracy))
			return boxer.Accuracy;
		else if (stat.Equals (Stats.Endurance))
			return boxer.Endurance;
		else if (stat.Equals (Stats.Health))
			return boxer.Health;
		else if (stat.Equals (Stats.Speed))
			return boxer.Speed;
		else if (stat.Equals (Stats.Strength))
			return boxer.Strength;

		return 0;
	}

	private static Dictionary<Stats, int> identifyBestStats(Boxer boxer){
		List<Stats> growthStats = new List<Stats> (new Stats[]
			{ Stats.AccuracyGrowth, Stats.EnduranceGrowth, Stats.HealthGrowth, Stats.SpeedGrowth, Stats.StrengthGrowth });

		Dictionary<Stats, int> bestStats = new Dictionary<Stats, int> ();

		for (int i = 0; i < 3; i++) {
			Stats bestStat = Stats.AccuracyGrowth;
			int max = 0;

			foreach (Stats stat in growthStats) {
				int statValue = getStat (stat, boxer);

				if (statValue > max) {
					bestStat = stat;
					max = statValue;
				}
			}

			bestStats.Add (bestStat, getStat (bestStat, boxer));
			growthStats.Remove (bestStat);
		}

		return bestStats;
	}
}
