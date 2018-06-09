using UnityEngine;
using System.Collections;

public class Consumable : Item
{
	public enum ConsumableType
	{
        Recovery, StressReliever
	}

	private ConsumableType type;

	private int potency;

    public Consumable(string name, float baseValue, ConsumableType type, int potency)
		: base(name, baseValue)
	{
		this.type = type;
        
		this.potency = potency;
	}

    public Consumable(JSONObject json)
		: base (json.GetField("name").str, json.GetField("basevalue").f)
	{
		this.type = parseConsumableType(json.GetField("type").str);
		this.potency = (int)json.GetField("potency").f;
	}

    public JSONObject jsonify()
	{
		JSONObject json = new JSONObject(JSONObject.Type.OBJECT);

		json.AddField("name", Name);
		json.AddField("basevalue", BaseValue);

		json.AddField("type", type.ToString());
		json.AddField("potency", potency);

		return json;
	}

    public ConsumableType parseConsumableType(string cType)
	{
		if (cType.Equals("Recovery"))
		{
			return ConsumableType.Recovery;
		}

        if (cType.Equals("StressReliever"))
		{
			return ConsumableType.StressReliever;
		}

		return ConsumableType.Recovery;
	}
   
    //Getters
    public int Potency
	{
		get { return potency; }
	}

    public ConsumableType Type
	{
		get { return type; }
	}
}
