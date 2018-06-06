using UnityEngine;
using System.Collections.Generic;
using System;

public class Legs : UpgradableItem
{
	public enum LegType
    {
        Standard, Jumpers, Sliders
    }

    private LegType type;

    public Legs(string name, float baseValue, int level, int levelCap, int tier, LegType type)
        : base(name, baseValue, level, levelCap, tier)
    {
        this.type = type;
    }

	public Legs(JSONObject json)
        : base(json.GetField("name").str, json.GetField("basevalue").f, 
		       (int)json.GetField("level").f, (int)json.GetField("levelcap").f, (int)json.GetField("tier").f)
    {
        type = parseLegType(json.GetField("type").str);

		setGrowth(json.GetField("growth").f);

        List<JSONObject> stats = json.GetField("stats").list;
        setBase(stats[0].f, stats[1].f, stats[2].f, stats[3].f, stats[4].f);

		List<JSONObject> dist = json.GetField("distribution").list;
        setDistribution(dist);
    }

	private LegType parseLegType(string lType)
	{
		if (lType.Equals("Standard"))
		{
			return LegType.Standard;
		}

        if (lType.Equals("Jumpers"))
		{
			return LegType.Jumpers;
		}

        if (lType.Equals("Sliders"))
		{
			return LegType.Sliders;
		}

		return LegType.Standard;
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

    //Getters
	public LegType Type
	{
		get { return type; }
	}
}
