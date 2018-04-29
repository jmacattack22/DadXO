using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TournamentProtocol {

	public enum Level
	{
		E, D, C, B, A, S, X
	}

	private CalendarDate date;
	private List<float> prizes;
	private int size;

	private List<int> managerIndexes;
	private Dictionary<int, TournamentResult> tournamentResults;
	//TODO Dictionary Pointing to tournamentResult

	private Level level;

	private int currentRound;
	private Dictionary<int, List<Vector2Int>> schedule;

    private bool priority;
    private bool quarterly;

	public TournamentProtocol(CalendarDate dt, float topPrize, int size, Level lvl, bool qtr){
		managerIndexes = new List<int> ();
		prizes = new List<float> ();
		tournamentResults = new Dictionary<int, TournamentResult> ();
        schedule = new Dictionary<int, List<Vector2Int>>();

		date = dt;

		prizes.Add (topPrize);
		prizes.Add (topPrize / 2.0f);
		prizes.Add (topPrize / 4.0f);
		prizes.Add (0.0f);

		this.size = size;

		level = lvl;

		currentRound = 0;
        priority = false;
        quarterly = qtr;
	}

	public void addContestant(int index){
		if (managerIndexes.Count < size) {
            if (!managerIndexes.Contains(index))
            {
                managerIndexes.Add(index);
                tournamentResults.Add(index, new TournamentResult());
            }
		}
	}

    public void cancelTournament(ref DataPool worldData){
        foreach (int index in tournamentResults.Keys)
        {
            worldData.ManagerProtocols[index].backOutOfTournament();
        }
    }

	public void logResults(ref DataPool worldData){
        managerIndexes = managerIndexes.OrderByDescending(x => tournamentResults[x].Record.getWinPercentage()).ToList();
		foreach (int index in managerIndexes) {
            Debug.Log(worldData.ManagerProtocols[index].getManagerStats(ref worldData) + " - " + tournamentResults[index].Record.getWinPercentage());
		}
	}

    public string getDetails(){
        return level.ToString() + " - " + prizes[0] + " - " + date + " - " + managerIndexes.Count + "/" + size;
    }

    public void refreshTournament(bool quarterly){
        managerIndexes.Clear();
        tournamentResults.Clear();
        priority = false;
        currentRound = 0;
        schedule.Clear();

        if (quarterly)
            date.advanceQuarter();
        else
            date.advanceYear();
    }

    public void setDate(CalendarDate newDate){
        date = newDate;
    }

	public void scheduleTournament(){
		schedule = TournamentScheduler.generateTournamentSchedule (managerIndexes.Count);
	}

	public void simNextRound(ref DataPool worldData){
		foreach (Vector2Int match in schedule[currentRound]) {
			float ovr1 = worldData.Boxers [worldData.ManagerProtocols [managerIndexes[match.x]].BoxerIndex].getOverall ();
			float ovr2 = worldData.Boxers [worldData.ManagerProtocols [managerIndexes[match.y]].BoxerIndex].getOverall ();

			float ovr1Chance = (ovr1 / (ovr1 + ovr2)) * 100.0f;

			if (Random.Range (0.0f, 100.0f) < ovr1Chance) {
				tournamentResults [managerIndexes [match.x]].Record.addWin (
					worldData.ManagerProtocols [managerIndexes [match.y]].CurrentBoxerELO);
				tournamentResults [managerIndexes [match.y]].Record.addLoss (
					worldData.ManagerProtocols [managerIndexes [match.x]].CurrentBoxerELO);

				Debug.Log (match.x + " vs. " + match.y + " - " + match.x + " Wins");
			} else {
				tournamentResults [managerIndexes [match.x]].Record.addLoss (
					worldData.ManagerProtocols [managerIndexes [match.y]].CurrentBoxerELO);
				tournamentResults [managerIndexes [match.y]].Record.addWin (
					worldData.ManagerProtocols [managerIndexes [match.x]].CurrentBoxerELO);
				Debug.Log (match.x + " vs. " + match.y + " - " + match.y + " Wins");
			}
		}

		currentRound++;
	}

    public void SimWholeTournament(ref DataPool worldData){
        foreach (int round in schedule.Keys){
            currentRound = round;
            foreach (Vector2Int match in schedule[round])
            {
                if (match.x != -1 && match.y != -1)
                {
                    float ovr1 = worldData.Boxers[worldData.ManagerProtocols[managerIndexes[match.x]].BoxerIndex].getOverall();
                    float ovr2 = worldData.Boxers[worldData.ManagerProtocols[managerIndexes[match.y]].BoxerIndex].getOverall();

                    float ovr1Chance = (ovr1 / (ovr1 + ovr2)) * 100.0f;

                    if (Random.Range(0.0f, 100.0f) < ovr1Chance)
                    {
                        tournamentResults[managerIndexes[match.x]].Record.addWin(
                            worldData.ManagerProtocols[managerIndexes[match.y]].CurrentBoxerELO);
                        tournamentResults[managerIndexes[match.y]].Record.addLoss(
                            worldData.ManagerProtocols[managerIndexes[match.x]].CurrentBoxerELO);

                        //Debug.Log(match.x + " vs. " + match.y + " - " + match.x + " Wins");
                    }
                    else
                    {
                        tournamentResults[managerIndexes[match.x]].Record.addLoss(
                            worldData.ManagerProtocols[managerIndexes[match.y]].CurrentBoxerELO);
                        tournamentResults[managerIndexes[match.y]].Record.addWin(
                            worldData.ManagerProtocols[managerIndexes[match.x]].CurrentBoxerELO);
                        //Debug.Log(match.x + " vs. " + match.y + " - " + match.y + " Wins");
                    }
                }
            }  
        }

        rankResults();
        if (quarterly)
        {
            tournamentResults[managerIndexes[0]].wonQuarterly();

            float boxerPercentage = (float) worldData.Distribution[level] / worldData.ManagerProtocols.Count;

            if (boxerPercentage > 0.1f)
            {
                tournamentResults[managerIndexes[1]].wonQuarterly();

                if (boxerPercentage > 0.2f && managerIndexes.Count > 2)
                {
                    tournamentResults[managerIndexes[2]].wonQuarterly();

                    if (boxerPercentage > 0.3f && managerIndexes.Count > 3)
                    {
                        tournamentResults[managerIndexes[3]].wonQuarterly();

                        if (boxerPercentage > 0.4f && managerIndexes.Count > 4)
                        {
                            tournamentResults[managerIndexes[4]].wonQuarterly();

                            if (boxerPercentage > 0.5f && managerIndexes.Count > 5)
                            {
                                tournamentResults[managerIndexes[5]].wonQuarterly();
                            }
                        }
                    }
                }
            }
        }

        foreach (int index in tournamentResults.Keys){
            worldData.ManagerProtocols[index].completeTournament(ref worldData, tournamentResults[index]);
        }
    }

    private void rankResults(){
        managerIndexes = managerIndexes.OrderByDescending(i => countPoints(tournamentResults[i].Record)).ToList();
    }

    private int countPoints(Record record){
        return (record.Wins * 2) + record.Ties;
    }

	public bool spaceLeft(){
		return managerIndexes.Count < size ? true : false;
	}

    public void togglePriority(){
        priority = !priority;
    }

	//Getters
	public Level TournamentLevel {
		get { return level; }
	}

	public CalendarDate TournamentDate {
		get { return date; }
	}

	public float TopPrize {
		get { return prizes [0]; }
	}

	public int Round {
		get { return currentRound; }
	}

	public int Size {
		get { return size; }
	}

    public bool Priority {
        get { return priority; }
    }

    public int Attendees {
        get { return managerIndexes.Count; }
    }
}
