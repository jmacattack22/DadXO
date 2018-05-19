using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading;

public class TournamentHandlerBehaviour : MonoBehaviour 
{
    public enum TournamentState 
	{
		None, QualifierRecruiting, TournamentRecruiting, Training, QualifierSim, TournamentSim
	}

	private TournamentState state;
	private List<int> thisWeeksTournaments;

	private Dictionary<TournamentState, Thread> threads = new Dictionary<TournamentState, Thread>();

	private bool recruitedForQualifiers = false;
	private bool recruitedForTournaments = false;
	private bool trainingComplete = false;
	private bool qualifiersSimulated = false;
	private bool tournamentsSimulated = false;

	private int debugRegionId = 0;

	private DataPool worldData;

	void Start () {
		state = TournamentState.None;
		thisWeeksTournaments = new List<int>();

		threads = new Dictionary<TournamentState, Thread>();
		threads.Add(TournamentState.QualifierRecruiting, new Thread(new ThreadStart(recruitForQualifiers)));
		threads.Add(TournamentState.TournamentRecruiting, new Thread(new ThreadStart(recruitForTournaments)));
		threads.Add(TournamentState.Training, new Thread(new ThreadStart(simTraining)));
		threads.Add(TournamentState.QualifierSim, new Thread(new ThreadStart(simQualifiers)));
		threads.Add(TournamentState.TournamentSim, new Thread(new ThreadStart(simTournaments)));
	}

	public void simTournamentsAndTraining()
    {
        if (isQualifyingWeek())
        {
			state = TournamentState.QualifierRecruiting;
        }
		else
		{
			state = TournamentState.TournamentRecruiting;
		}

		print(worldData.Calendar.getDate(Calendar.DateType.fullLong));

		threads[state].Start();

		//state = TournamentState.TournamentRecruiting;
  //      recruitForTournaments();

		//state = TournamentState.Training;
  //      simTraining();

  //      if (isQualifyingWeek())
  //      {
		//	state = TournamentState.QualifierSim;
  //          simQualifiers();
  //      }

		//state = TournamentState.TournamentSim;
  //      simTournaments();

		//worldData.updateBoxerDistribution();

        //worldData.Calendar.progessWeek();
        //Debug.Log(worldData.Calendar.getDate(Calendar.DateType.fullLong));
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
		debugRegionId = 0;
        foreach (Region region in worldData.Regions)
        {
            foreach (TournamentProtocol.Level level in worldData.Capitols[region.CapitolIndex].Quarterlies.Keys)
            {
                List<int> regionsWithinJurisdiction = new List<int>();

				for (int i = 0; i < worldData.Regions.Count; i++)
				{
					if (region.CapitolIndex == worldData.Regions[i].CapitolIndex)
					{
						regionsWithinJurisdiction.Add(i);
					}
					else
					{
						int distance = region.getDistanceToRegion(i);

                        if (distance < getDistanceFromLevel(level))
                        {
                            regionsWithinJurisdiction.Add(i);
                        }	
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
			debugRegionId++;
        }

		recruitedForQualifiers = true;
    }

	private void recruitForTournaments()
    {
        thisWeeksTournaments = new List<int>();
		debugRegionId = 0;

        foreach (Region region in worldData.Regions)
        {
            foreach (int tIndex in region.getRegionsTownIndexes())
            {
                if (worldData.Towns[tIndex].Tournament.TournamentDate.sameWeek(worldData.Calendar.GetCalendarDate()))
                {
                    thisWeeksTournaments.Add(tIndex);

                    List<int> regionsWithinJurisdiction = new List<int>();

                    for (int i = 0; i < worldData.Regions.Count; i++)
                    {
                        if (region.CapitolIndex == worldData.Regions[i].CapitolIndex)
						{
							regionsWithinJurisdiction.Add(i);
						}
						else
						{
							int distance = region.getDistanceToRegion(i);

                            if (distance < getDistanceFromLevel(worldData.Towns[tIndex].Tournament.TournamentLevel))
                            {
                                regionsWithinJurisdiction.Add(i);
                            }	
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
			debugRegionId++;
        }

		recruitedForTournaments = true;
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

		qualifiersSimulated = true;
    }

    private void simTournaments()
    {
        foreach (int townIndex in thisWeeksTournaments)
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

		tournamentsSimulated = true;
    }

	private void simTraining()
	{
		foreach (ManagerProtocol mp in worldData.ManagerProtocols)
		{
			mp.executeWeek(ref worldData);
		}

		trainingComplete = true;
	}

	public void updateWorldData(DataPool worldData)
	{
		this.worldData = worldData;	
	}

	void Update()
	{
		if (state.Equals(TournamentState.QualifierRecruiting) && recruitedForQualifiers)
		{
			recruitedForQualifiers = false;
			state = TournamentState.TournamentRecruiting;

			threads[state].Start();
		} 
		else if (state.Equals(TournamentState.TournamentRecruiting) && recruitedForTournaments)
		{
			recruitedForTournaments = false;
			state = TournamentState.Training;

			threads[state].Start();
		}
		else if (state.Equals(TournamentState.Training) && trainingComplete)
		{
			trainingComplete = false;
            
            if (isQualifyingWeek())
			{
				state = TournamentState.QualifierSim;
			}
			else
			{
				state = TournamentState.TournamentSim;
			}

			threads[state].Start();
		}
		else if (state.Equals(TournamentState.QualifierSim) && qualifiersSimulated)
		{
			qualifiersSimulated = false;
			state = TournamentState.TournamentSim;

			threads[state].Start();
		}
		else if (state.Equals(TournamentState.TournamentSim) && tournamentsSimulated)
		{
			tournamentsSimulated = false;
			state = TournamentState.None;
		}      

		if (!state.Equals(TournamentState.None))
		{
			print(state + " - " + debugRegionId);	
		}      
	}
}
