using UnityEngine;
using System.Collections.Generic;

public class Arm : UpgradableItem
{
    public enum ArmType
	{
		Sword, Gun, Whip, Spear
	}

	private ArmType type;

    public Arm(string name, float baseValue, int level, int levelCap, int tier, ArmType type)
		: base(name, baseValue, level, levelCap, tier)
	{
		this.type = type;
	}

	public Arm(JSONObject json)
		: base(json.GetField("name").str, json.GetField("basevalue").f, 
		       (int)json.GetField("level").f, (int)json.GetField("levelcap").f, (int)json.GetField("tier").f)
	{
		type = parseArmType(json.GetField("type").str);

		setGrowth(json.GetField("growth").f);

		List<JSONObject> stats = json.GetField("stats").list;
		setBase(stats[0].f, stats[1].f, stats[2].f, stats[3].f, stats[4].f);

		List<JSONObject> dist = json.GetField("distribution").list;
		setDistribution(dist);
	}

    public JSONObject jsonify()
	{
		JSONObject json = new JSONObject(JSONObject.Type.OBJECT);

		json.AddField("name", Name);
		json.AddField("basevalue", BaseValue);
		json.AddField("level", Level);
		json.AddField("levelcap", LevelCap);
		json.AddField("tier", Tier);

		json.AddField("growth", GrowthFactor);

		JSONObject stats = new JSONObject(JSONObject.Type.ARRAY);
        foreach (float stat in getStatsInList())
		{
			stats.Add(stat);
		}
		json.AddField("stats", stats);

		JSONObject dist = new JSONObject(JSONObject.Type.ARRAY);
        foreach (EvaluationProtocol.Stats stat in getDistribution())
		{
			dist.Add(stat.ToString());
		}
		json.AddField("distribution", dist);
      
		json.AddField("type", type.ToString());

		return json;
	}

    public ArmType parseArmType(string aType)
	{
		if (aType.Equals("Sword"))
		{
			return ArmType.Sword;
		}

        if (aType.Equals("Gun"))
		{
			return ArmType.Gun;
		}

        if (aType.Equals("Whip"))
		{
			return ArmType.Whip;
		}

        if (aType.Equals("Spear"))
		{
			return ArmType.Spear;
		}

		return ArmType.Sword;
	}

	//Getters
	public ArmType Type
	{
		get { return type; }
	}
}
