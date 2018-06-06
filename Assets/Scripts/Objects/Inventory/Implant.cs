using UnityEngine;
using System.Collections.Generic;

public class Implant : UpgradableItem
{
	public enum ImplantType
	{
		Standard
	}

	private ImplantType type;

	public Implant(string name, float baseValue, int level, int levelCap, int tier, ImplantType type)
		: base(name, baseValue, level, levelCap, tier)
	{
		this.type = type;
	}

	public Implant(JSONObject json)
        : base(json.GetField("name").str, json.GetField("basevalue").f, 
		       (int)json.GetField("level").f, (int)json.GetField("levelcap").f, (int)json.GetField("tier").f)
    {
        type = parseImplantType(json.GetField("type").str);

		setGrowth(json.GetField("growth").f);

        List<JSONObject> stats = json.GetField("stats").list;
        setBase(stats[0].f, stats[1].f, stats[2].f, stats[3].f, stats[4].f);
    }

    private ImplantType parseImplantType(string iType)
    {
        if (iType.Equals("Standard"))
        {
            return ImplantType.Standard;
        }

        return ImplantType.Standard;
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

        json.AddField("type", type.ToString());

        return json;
    }

	//Getters
	public ImplantType Type
	{
		get { return type; }
	}
}
