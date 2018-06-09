using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Capitol : Town
{
    private Dictionary<TournamentProtocol.Level, Tournament> quarterlies;

    public Capitol(string name, Vector2Int location, int regionDistance)
        : base(name, location, regionDistance)
    {
        quarterlies = new Dictionary<TournamentProtocol.Level, Tournament>();
    }

    public Capitol(JSONObject json)
		: base(json.GetField("name").str, JSONTemplates.ToVector2Int(json.GetField("location")), (TournamentProtocol.Level)Enum.Parse(typeof(TournamentProtocol.Level), json.GetField("level").str))
	{
		quarterlies = new Dictionary<TournamentProtocol.Level, Tournament>();

        foreach (JSONObject record in json.GetField("quarterlies").list)
		{
			quarterlies.Add(
				(TournamentProtocol.Level)Enum.Parse(typeof(TournamentProtocol.Level), record.GetField("key").str),
				new Tournament(record.GetField("value")));
		}
	}

    public void setupQualifier(Dictionary<TournamentProtocol.Level, bool> qualifierMap)
	{
        foreach (TournamentProtocol.Level level in qualifierMap.Keys)
        {
            if (qualifierMap[level])
            {
                quarterlies.Add(level, WorldBuilderProtocol.createQuarterlyTournament(level, new CalendarDate(1, 1, 1602)));
            }
        }
    }


	public new JSONObject jsonify()
	{
		JSONObject json = new JSONObject(JSONObject.Type.OBJECT);

        json.AddField("name", Name);
        json.AddField("location", JSONTemplates.FromVector2Int(Location));
        json.AddField("level", RegionLevel.ToString());

		JSONObject qrts = new JSONObject(JSONObject.Type.ARRAY);
		foreach (TournamentProtocol.Level level in quarterlies.Keys)
		{
			JSONObject record = new JSONObject(JSONObject.Type.OBJECT);
			record.AddField("key", level.ToString());
			record.AddField("value", quarterlies[level].jsonify());
		}
		json.AddField("quarterlies", qrts);

        return json;
	}

    //Getters
    public Dictionary<TournamentProtocol.Level, Tournament> Quarterlies
	{
        get { return quarterlies; }
    }
}
