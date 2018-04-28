using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Capitol : Town
{
    private Dictionary<TournamentProtocol.Level, TournamentProtocol> quarterlies;

	public Capitol(string name, Vector2Int location)
        : base(name, location)
    {
        quarterlies = new Dictionary<TournamentProtocol.Level, TournamentProtocol>();

        quarterlies.Add(TournamentProtocol.Level.E, WorldBuilderProtocol.createQuarterlyTournament(TournamentProtocol.Level.E, new CalendarDate(1, 1, 1602)));
        quarterlies.Add(TournamentProtocol.Level.D, WorldBuilderProtocol.createQuarterlyTournament(TournamentProtocol.Level.D, new CalendarDate(1, 1, 1602)));
        quarterlies.Add(TournamentProtocol.Level.C, WorldBuilderProtocol.createQuarterlyTournament(TournamentProtocol.Level.C, new CalendarDate(1, 1, 1602)));
        quarterlies.Add(TournamentProtocol.Level.B, WorldBuilderProtocol.createQuarterlyTournament(TournamentProtocol.Level.B, new CalendarDate(1, 1, 1602)));
        quarterlies.Add(TournamentProtocol.Level.A, WorldBuilderProtocol.createQuarterlyTournament(TournamentProtocol.Level.A, new CalendarDate(1, 1, 1602)));
        quarterlies.Add(TournamentProtocol.Level.S, WorldBuilderProtocol.createQuarterlyTournament(TournamentProtocol.Level.S, new CalendarDate(1, 1, 1602)));
    }

    //Getters
    public Dictionary<TournamentProtocol.Level, TournamentProtocol> Quarterlies {
        get { return quarterlies; }
    }
}
