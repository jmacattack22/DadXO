using UnityEngine;
using System.Collections.Generic;
using System;

public class UpgradableItem : Item
{
	private int level;
    private int levelCap;
	private int tier;

	private Dictionary<EvaluationProtocol.Stats, float> baseFactors;
	private float growthFactor;

	private List<EvaluationProtocol.Stats> distribution;

	private float experience;
	private float experienceFactor;
	private float experienceTarget;

	public UpgradableItem(string name, float baseValue, int level, int levelCap, int tier)
		: base(name, baseValue)
	{
		this.level = level;
		this.levelCap = levelCap;
		this.tier = tier;

		baseFactors = new Dictionary<EvaluationProtocol.Stats, float>();
		growthFactor = 0.1f;

		distribution = new List<EvaluationProtocol.Stats>();

		defineExperienceBasedOnTier();
	}

    public void battleWon()
	{
		experience += 1.0f;

        if (experience >= experienceTarget && level < levelCap)
		{
			level++;
			experience = 0.0f;
			experienceTarget = experienceTarget * experienceFactor;

			EvaluationProtocol.Stats statToUpgrade = distribution[Rng.generateRandomInt(0, distribution.Count - 1)];
			baseFactors[statToUpgrade] += growthFactor;

            if (Rng.generateRandomInt(0, 1000) > 970)
			{
				statToUpgrade = distribution[Rng.generateRandomInt(0, distribution.Count - 1)];
                baseFactors[statToUpgrade] += growthFactor;
			}
		}
	}

	public void defineDistribution(List<EvaluationProtocol.Stats> bestStats)
	{
		foreach (EvaluationProtocol.Stats stat in bestStats)
		{
			distribution.Add(stat);
			distribution.Add(stat);
		}

		distribution.Add(EvaluationProtocol.Stats.Accuracy);
		distribution.Add(EvaluationProtocol.Stats.Endurance);
		distribution.Add(EvaluationProtocol.Stats.Health);
		distribution.Add(EvaluationProtocol.Stats.Speed);
		distribution.Add(EvaluationProtocol.Stats.Strength);
	}

	private void defineExperienceBasedOnTier()
	{
		if (tier == 1)
		{
			experience = 0.0f;
			experienceFactor = 1.2f;
			experienceTarget = 8.0f;
		}
		else if (tier == 2)
		{
			experience = 0.0f;
            experienceFactor = 1.2f;
            experienceTarget = 9.0f;
		}
		else if (tier == 3)
        {
            experience = 0.0f;
            experienceFactor = 1.3f;
            experienceTarget = 9.0f;
        }
		else if (tier == 4)
        {
            experience = 0.0f;
            experienceFactor = 1.4f;
            experienceTarget = 10.0f;
        }
		else if (tier == 5)
        {
            experience = 0.0f;
            experienceFactor = 1.5f;
            experienceTarget = 10.0f;
        }
	}

	public float getStat(EvaluationProtocol.Stats stat)
	{
		return baseFactors[stat];
	}

    public List<float> getStatsInList()
	{
		List<float> stats = new List<float>();

		stats.Add(baseFactors[EvaluationProtocol.Stats.Accuracy]);
		stats.Add(baseFactors[EvaluationProtocol.Stats.Endurance]);
		stats.Add(baseFactors[EvaluationProtocol.Stats.Health]);
		stats.Add(baseFactors[EvaluationProtocol.Stats.Speed]);
		stats.Add(baseFactors[EvaluationProtocol.Stats.Strength]);

		return stats;
	}

    public void setBase(float accuracy, float endurance, float health, float speed, float strength)
	{
		baseFactors.Add(EvaluationProtocol.Stats.Accuracy, accuracy);
		baseFactors.Add(EvaluationProtocol.Stats.Endurance, endurance);
		baseFactors.Add(EvaluationProtocol.Stats.Health, health);
		baseFactors.Add(EvaluationProtocol.Stats.Speed, speed);
		baseFactors.Add(EvaluationProtocol.Stats.Strength, strength);
	}

    public void setGrowth(float growth)
	{
		growthFactor = growth;
	}

    //Getters
    public int Level
	{
		get { return level; }
	}

    public int LevelCap 
	{
		get { return levelCap; }
	}

    public float GrowthFactor
	{
		get { return growthFactor; }
	}
   
    public int Tier
	{
		get { return tier; }
	}
}
