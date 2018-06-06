using UnityEngine;
using System.Collections;
using System;

public class Legs : UpgradableItem
{
	public enum LegType
    {
        Standard, Jumpers, Sliders
    }

    private LegType type;

    public Legs(string name, float baseValue, int level, int levelCap, LegType type)
        : base(name, baseValue, level, levelCap)
    {
        this.type = type;
    }

	public Legs(JSONObject json)
        : base(json.GetField("name").str, json.GetField("basevalue").f, (int)json.GetField("level").f, (int)json.GetField("levelcap").f)
    {
        type = parseLegType(json.GetField("type").str);
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

        json.AddField("type", type.ToString());

        return json;
	}

    //Getters
	public LegType Type
	{
		get { return type; }
	}
}
