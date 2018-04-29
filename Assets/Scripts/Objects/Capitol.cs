using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Capitol : Town
{
    private Dictionary<TournamentProtocol.Level, TournamentProtocol> quarterlies;

	public Capitol(string name, Vector2Int location, int capitolType)
        : base(name, location)
    {
        quarterlies = new Dictionary<TournamentProtocol.Level, TournamentProtocol>();

        createCapitolBasedOnLevel(capitolType);
    }

    private void createCapitolBasedOnLevel(int capitolType){
        if (capitolType == 0){
            quarterlies.Add(TournamentProtocol.Level.E, WorldBuilderProtocol.createQuarterlyTournament(TournamentProtocol.Level.E, new CalendarDate(1, 1, 1602)));
        } else if (capitolType == 1){
            quarterlies.Add(TournamentProtocol.Level.D, WorldBuilderProtocol.createQuarterlyTournament(TournamentProtocol.Level.D, new CalendarDate(1, 1, 1602)));
        } else if (capitolType == 2){
            quarterlies.Add(TournamentProtocol.Level.D, WorldBuilderProtocol.createQuarterlyTournament(TournamentProtocol.Level.D, new CalendarDate(1, 1, 1602)));
            quarterlies.Add(TournamentProtocol.Level.C, WorldBuilderProtocol.createQuarterlyTournament(TournamentProtocol.Level.C, new CalendarDate(1, 1, 1602)));
        } else if (capitolType == 3){
            quarterlies.Add(TournamentProtocol.Level.B, WorldBuilderProtocol.createQuarterlyTournament(TournamentProtocol.Level.B, new CalendarDate(1, 1, 1602)));
        } else if (capitolType == 4){
            quarterlies.Add(TournamentProtocol.Level.E, WorldBuilderProtocol.createQuarterlyTournament(TournamentProtocol.Level.E, new CalendarDate(1, 1, 1602)));
            quarterlies.Add(TournamentProtocol.Level.A, WorldBuilderProtocol.createQuarterlyTournament(TournamentProtocol.Level.A, new CalendarDate(1, 1, 1602)));
        } else if (capitolType == 5){
            quarterlies.Add(TournamentProtocol.Level.B, WorldBuilderProtocol.createQuarterlyTournament(TournamentProtocol.Level.B, new CalendarDate(1, 1, 1602)));
            quarterlies.Add(TournamentProtocol.Level.A, WorldBuilderProtocol.createQuarterlyTournament(TournamentProtocol.Level.A, new CalendarDate(1, 1, 1602)));
        } else if (capitolType == 6)
        {
            quarterlies.Add(TournamentProtocol.Level.D, WorldBuilderProtocol.createQuarterlyTournament(TournamentProtocol.Level.D, new CalendarDate(1, 1, 1602)));
            quarterlies.Add(TournamentProtocol.Level.C, WorldBuilderProtocol.createQuarterlyTournament(TournamentProtocol.Level.C, new CalendarDate(1, 1, 1602)));
            quarterlies.Add(TournamentProtocol.Level.S, WorldBuilderProtocol.createQuarterlyTournament(TournamentProtocol.Level.S, new CalendarDate(1, 1, 1602)));
        }
    }

    //Getters
    public Dictionary<TournamentProtocol.Level, TournamentProtocol> Quarterlies {
        get { return quarterlies; }
    }
}
