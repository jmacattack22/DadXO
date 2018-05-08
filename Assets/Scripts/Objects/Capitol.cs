using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Capitol : Town
{
    private Dictionary<TournamentProtocol.Level, TournamentProtocol> quarterlies;

    public Capitol(string name, Vector2Int location, int regionDistance)
        : base(name, location, regionDistance)
    {
        quarterlies = new Dictionary<TournamentProtocol.Level, TournamentProtocol>();
    }

    public void setupQualifier(Dictionary<TournamentProtocol.Level, bool> qualifierMap){
        foreach (TournamentProtocol.Level level in qualifierMap.Keys)
        {
            if (qualifierMap[level])
            {
                quarterlies.Add(level, WorldBuilderProtocol.createQuarterlyTournament(level, new CalendarDate(1, 1, 1602)));
            }
        }
    }

    //Getters
    public Dictionary<TournamentProtocol.Level, TournamentProtocol> Quarterlies {
        get { return quarterlies; }
    }
}
