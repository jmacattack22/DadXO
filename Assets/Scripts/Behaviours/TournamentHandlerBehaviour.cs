﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TournamentHandlerBehaviour : MonoBehaviour 
{
    public enum TournamentState 
	{
		None, QualifierRecruiting, TournamentRecruiting, Training, QualifierSim, TournamentSim
	}

	private TournamentState state;

	private DataPool worldData;

	void Start () {
		state = TournamentState.None;
	}

	public void simTournamentsAndTraining()
    {
        if (isQualifyingWeek())
        {
			state = TournamentState.QualifierRecruiting;
            recruitForQualifiers();
        }

		state = TournamentState.TournamentRecruiting;
        List<int> townTournaments = recruitForTournaments();

		state = TournamentState.Training;
        simTraining();

        if (isQualifyingWeek())
        {
			state = TournamentState.QualifierSim;
            simQualifiers();
        }

		state = TournamentState.TournamentSim;
        simTournaments(townTournaments);

		worldData.updateBoxerDistribution();

        worldData.Calendar.progessWeek();
        Debug.Log(worldData.Calendar.getDate(Calendar.DateType.fullLong));
    }

	private int getDistanceFromLevel(TournamentProtocol.Level level)
    {
        if (level.Equals(TournamentProtocol.Level.S))
            return 10;
        else if (level.Equals(TournamentProtocol.Level.A))
            return 4;
        else if (level.Equals(TournamentProtocol.Level.B))
            return 3;
        else if (level.Equals(TournamentProtocol.Level.C))
            return 3;
        else if (level.Equals(TournamentProtocol.Level.D))
            return 2;
        else if (level.Equals(TournamentProtocol.Level.E))
            return 2;

        return 10;
    }

	public bool isQualifyingWeek()
    {
        return ((worldData.Calendar.getWeekOfYear() - 1) % 8 == 0);
    }

	private List<int> recruit(TournamentProtocol.Level level, List<int> regionsWithinJurisdiction, bool highestRated)
    {
        List<int> potentialRecruits = new List<int>();

        foreach (int rIndex in regionsWithinJurisdiction)
        {
            foreach (int mIndex in worldData.Regions[rIndex].getRegionsManagerIndexes())
            {
                if (worldData.ManagerProtocols[mIndex].Rank.Equals(level) && !worldData.ManagerProtocols[mIndex].isBusy())
                {
                    potentialRecruits.Add(mIndex);
                }
            }
        }

        if (highestRated)
            potentialRecruits = potentialRecruits.OrderByDescending(t => EvaluationProtocol.evaluateBoxer(worldData.Boxers[t])).ToList();
        else
            potentialRecruits = potentialRecruits.OrderByDescending(t => worldData.ManagerProtocols[t].TournamentPriority).ToList();

        return potentialRecruits;
    }

	private void recruitForQualifiers()
    {
        foreach (Region region in worldData.Regions)
        {
            foreach (TournamentProtocol.Level level in worldData.Capitols[region.CapitolIndex].Quarterlies.Keys)
            {
                List<int> regionsWithinJurisdiction = new List<int>();

                for (int i = 0; i < worldData.Regions.Count; i++)
                {
                    List<Vector2Int> path = worldData.Dijkstras.shortestPath(region.Position, worldData.Regions[i].Position);

                    if (path.Count < getDistanceFromLevel(level))
                    {
                        regionsWithinJurisdiction.Add(i);
                    }
                }

                List<int> recruits = recruit(level, regionsWithinJurisdiction, true);

                for (int i = 0; i < recruits.Count; i++)
                {
                    if (worldData.Capitols[region.CapitolIndex].Quarterlies[level].spaceLeft())
                    {
                        worldData.Capitols[region.CapitolIndex].Quarterlies[level].addContestant(recruits[i]);
                        worldData.ManagerProtocols[recruits[i]].attendTournament();
                    }
                }
            }
        }
    }

	private List<int> recruitForTournaments()
    {
        List<int> townTournaments = new List<int>();

        foreach (Region region in worldData.Regions)
        {
            foreach (int tIndex in region.getRegionsTownIndexes())
            {
                if (worldData.Towns[tIndex].Tournament.TournamentDate.sameWeek(worldData.Calendar.GetCalendarDate()))
                {
                    townTournaments.Add(tIndex);

                    List<int> regionsWithinJurisdiction = new List<int>();

                    for (int i = 0; i < worldData.Regions.Count; i++)
                    {
                        List<Vector2Int> path = worldData.Dijkstras.shortestPath(region.Position, worldData.Regions[i].Position);

                        if (path.Count < getDistanceFromLevel(worldData.Towns[tIndex].Tournament.TournamentLevel))
                        {
                            regionsWithinJurisdiction.Add(i);
                        }
                    }

                    List<int> recruits = recruit(worldData.Towns[tIndex].Tournament.TournamentLevel, regionsWithinJurisdiction, false);

                    for (int i = 0; i < recruits.Count; i++)
                    {
                        if (worldData.Towns[tIndex].Tournament.spaceLeft())
                        {
                            worldData.Towns[tIndex].Tournament.addContestant(recruits[i]);
                            worldData.ManagerProtocols[recruits[i]].attendTournament();
                        }
                        else
                        {
                            worldData.ManagerProtocols[recruits[i]].bumpTournamentPriority();
                        }
                    }
                }
            }
        }

        return townTournaments;
    }

	private void simQualifiers()
    {
        foreach (Capitol c in worldData.Capitols)
        {
            foreach (TournamentProtocol.Level level in c.Quarterlies.Keys)
            {
                if (c.Quarterlies[level].Attendees > 2)
                {
                    //Debug.Log(c.Location.ToString() + " - " + c.Quarterlies[level].getDetails());
                    c.Quarterlies[level].scheduleTournament();
                    c.Quarterlies[level].SimWholeTournament(ref worldData);
                    //c.Quarterlies[level].logResults(ref worldData);
                }
                else
                {
                    c.Quarterlies[level].cancelTournament(ref worldData);
                }

                c.Quarterlies[level].refreshTournament(true);
            }
        }
    }

    private void simTournaments(List<int> townTournaments)
    {
        foreach (int townIndex in townTournaments)
        {
            if (worldData.Towns[townIndex].Tournament.Attendees > 2)
            {
                //Debug.Log(worldData.Towns[tIndex].Location.ToString() + " - " + worldData.Towns[tIndex].Tournament.getDetails());
                worldData.Towns[townIndex].Tournament.scheduleTournament();
                worldData.Towns[townIndex].Tournament.SimWholeTournament(ref worldData);
            }
            else
            {
                worldData.Towns[townIndex].Tournament.cancelTournament(ref worldData);
            }

            worldData.Towns[townIndex].Tournament.refreshTournament(false);
        }
    }

	private void simTraining()
	{
		foreach (ManagerProtocol mp in worldData.ManagerProtocols)
		{
			mp.executeWeek(ref worldData);
		}
	}

	public void updateWorldData(DataPool worldData)
	{
		this.worldData = worldData;	
	}

	void Update () {
		if (!state.Equals(TournamentState.None))
		{
			print(state);	
		}      
	}
}
