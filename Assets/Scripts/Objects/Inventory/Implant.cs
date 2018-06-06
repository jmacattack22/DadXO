using UnityEngine;
using System.Collections;

public class Implant : UpgradableItem
{
	public enum ImplantType
	{
		Standard
	}

	private ImplantType type;

	public Implant(string name, float baseValue, int level, int levelCap, ImplantType type)
		: base(name, baseValue, level, levelCap)
	{
		this.type = type;
	}

	public Implant(JSONObject json)
        : base(json.GetField("name").str, json.GetField("basevalue").f, (int)json.GetField("level").f, (int)json.GetField("levelcap").f)
    {
        type = parseImplantType(json.GetField("type").str);
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

        json.AddField("type", type.ToString());

        return json;
    }

	//Getters
	public ImplantType Type
	{
		get { return type; }
	}
}
