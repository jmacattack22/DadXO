using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class TournamentProtocol {

	public enum Level
	{
		E, D, C, B, A, S, X
	}

    public static void cancelTournament(ref DataPool worldData, int townIndex){
		foreach (int index in worldData.getTournamentFromTownIndex(townIndex).getTournamentResults().Keys)
		{
			worldData.Managers[index].backOutOfTournament();
		}
    }

	private static int generateRandomInt(int min, int max)
    {
        return new System.Random((int)System.DateTime.Now.Ticks).Next(min, max);
    }

	public static void simNextRound(ref DataPool worldData, int townIndex){
		int currentRound = worldData.getTournamentFromTownIndex(townIndex).Round;
		List<int> managerIndexes = worldData.getTournamentFromTownIndex(townIndex).getManagerIndexes();


		foreach (Vector2Int match in worldData.getTournamentFromTownIndex(townIndex).getSchedule()[currentRound]) {
			float ovr1 = worldData.Boxers [worldData.Managers [managerIndexes[match.x]].BoxerIndex].getOverall ();
			float ovr2 = worldData.Boxers [worldData.Managers [managerIndexes[match.y]].BoxerIndex].getOverall ();

			float ovr1Chance = (ovr1 / (ovr1 + ovr2)) * 100.0f;

			float chance = generateRandomInt(0, 100);
			if (chance < ovr1Chance) {
				worldData.getTournamentFromTownIndex(townIndex).getTournamentResults() [managerIndexes [match.x]].Record.addWin (
					worldData.Managers [managerIndexes [match.y]].BoxerELO);
				worldData.getTournamentFromTownIndex(townIndex).getTournamentResults() [managerIndexes [match.y]].Record.addLoss (
					worldData.Managers [managerIndexes [match.x]].BoxerELO);

				Debug.Log (match.x + " vs. " + match.y + " - " + match.x + " Wins");
			} else {
				worldData.getTournamentFromTownIndex(townIndex).getTournamentResults() [managerIndexes [match.x]].Record.addLoss (
					worldData.Managers [managerIndexes [match.y]].BoxerELO);
				worldData.getTournamentFromTownIndex(townIndex).getTournamentResults() [managerIndexes [match.y]].Record.addWin (
					worldData.Managers [managerIndexes [match.x]].BoxerELO);
				Debug.Log (match.x + " vs. " + match.y + " - " + match.y + " Wins");
			}
		}

		currentRound++;
	}

    public static void simWholeQualifier(ref DataPool worldData, int capitolIndex, Level rank)
	{
		Dictionary<int, List<Vector2Int>> schedule = worldData.getTournamentFromCapitolIndex(capitolIndex, rank).getSchedule();
		List<int> managerIndexes = worldData.getTournamentFromCapitolIndex(capitolIndex, rank).getManagerIndexes();
        int currentRound = 0;

        foreach (int round in schedule.Keys)
        {
            currentRound = round;
            foreach (Vector2Int match in schedule[round])
            {
                if (match.x != -1 && match.y != -1)
                {
                    float ovr1 = worldData.Boxers[worldData.Managers[managerIndexes[match.x]].BoxerIndex].getOverall();
                    float ovr2 = worldData.Boxers[worldData.Managers[managerIndexes[match.y]].BoxerIndex].getOverall();

                    float ovr1Chance = (ovr1 / (ovr1 + ovr2)) * 100.0f;

                    float chance = generateRandomInt(0, 100);
                    if (chance < ovr1Chance)
                    {
						worldData.getTournamentFromCapitolIndex(capitolIndex, rank).getTournamentResults()[managerIndexes[match.x]].Record.addWin(
                            worldData.Managers[managerIndexes[match.y]].BoxerELO);
						worldData.getTournamentFromCapitolIndex(capitolIndex, rank).getTournamentResults()[managerIndexes[match.y]].Record.addLoss(
                            worldData.Managers[managerIndexes[match.x]].BoxerELO);
                    }
                    else
                    {
						worldData.getTournamentFromCapitolIndex(capitolIndex, rank).getTournamentResults()[managerIndexes[match.x]].Record.addLoss(
                            worldData.Managers[managerIndexes[match.y]].BoxerELO);
						worldData.getTournamentFromCapitolIndex(capitolIndex, rank).getTournamentResults()[managerIndexes[match.y]].Record.addWin(
                            worldData.Managers[managerIndexes[match.x]].BoxerELO);
                    }
                }
            }
        }

		worldData.getTournamentFromCapitolIndex(capitolIndex, rank).rankResults();
		managerIndexes = worldData.getTournamentFromCapitolIndex(capitolIndex, rank).getManagerIndexes();

		worldData.getTournamentFromCapitolIndex(capitolIndex, rank).getTournamentResults()[managerIndexes[0]].wonQuarterly();
		Level level = worldData.getTournamentFromCapitolIndex(capitolIndex, rank).Level;

        float boxerPercentage = (float)worldData.Distribution[rank] / worldData.Managers.Count;

        if (boxerPercentage > 0.1f)
        {
			worldData.getTournamentFromCapitolIndex(capitolIndex, rank).getTournamentResults()[managerIndexes[1]].wonQuarterly();

            if (boxerPercentage > 0.2f && managerIndexes.Count > 2)
            {
				worldData.getTournamentFromCapitolIndex(capitolIndex, rank).getTournamentResults()[managerIndexes[2]].wonQuarterly();

                if (boxerPercentage > 0.3f && managerIndexes.Count > 3)
                {
					worldData.getTournamentFromCapitolIndex(capitolIndex, rank).getTournamentResults()[managerIndexes[3]].wonQuarterly();

                    if (boxerPercentage > 0.4f && managerIndexes.Count > 4)
                    {
						worldData.getTournamentFromCapitolIndex(capitolIndex, rank).getTournamentResults()[managerIndexes[4]].wonQuarterly();

                        if (boxerPercentage > 0.5f && managerIndexes.Count > 5)
                        {
							worldData.getTournamentFromCapitolIndex(capitolIndex, rank).getTournamentResults()[managerIndexes[5]].wonQuarterly();
                        }
                    }
                }
            }
        }

		foreach (int index in worldData.getTournamentFromCapitolIndex(capitolIndex, rank).getTournamentResults().Keys)
        {
			ManagerProtocol.completeTournament(ref worldData, index, worldData.getTournamentFromCapitolIndex(capitolIndex, rank).getTournamentResults()[index]);
        }
	}

	public static void SimWholeTournament(ref DataPool worldData, int townIndex){
		Dictionary<int, List<Vector2Int>> schedule = worldData.getTournamentFromTownIndex(townIndex).getSchedule();
		List<int> managerIndexes = worldData.getTournamentFromTownIndex(townIndex).getManagerIndexes();
		int currentRound = 0;

        foreach (int round in schedule.Keys){
            currentRound = round;
            foreach (Vector2Int match in schedule[round])
            {
                if (match.x != -1 && match.y != -1)
                {
                    float ovr1 = worldData.Boxers[worldData.Managers[managerIndexes[match.x]].BoxerIndex].getOverall();
                    float ovr2 = worldData.Boxers[worldData.Managers[managerIndexes[match.y]].BoxerIndex].getOverall();

                    float ovr1Chance = (ovr1 / (ovr1 + ovr2)) * 100.0f;

					float chance = generateRandomInt(0, 100);
                    if (chance < ovr1Chance)
                    {
						worldData.getTournamentFromTownIndex(townIndex).getTournamentResults()[managerIndexes[match.x]].Record.addWin(
                            worldData.Managers[managerIndexes[match.y]].BoxerELO);
						worldData.getTournamentFromTownIndex(townIndex).getTournamentResults()[managerIndexes[match.y]].Record.addLoss(
                            worldData.Managers[managerIndexes[match.x]].BoxerELO);
                    }
                    else
                    {
						worldData.getTournamentFromTownIndex(townIndex).getTournamentResults()[managerIndexes[match.x]].Record.addLoss(
                            worldData.Managers[managerIndexes[match.y]].BoxerELO);
						worldData.getTournamentFromTownIndex(townIndex).getTournamentResults()[managerIndexes[match.y]].Record.addWin(
                            worldData.Managers[managerIndexes[match.x]].BoxerELO);
                    }
                }
            }  
        }

		worldData.getTournamentFromTownIndex(townIndex).rankResults();
		managerIndexes = worldData.getTournamentFromTownIndex(townIndex).getManagerIndexes();

		foreach (int index in worldData.getTournamentFromTownIndex(townIndex).getTournamentResults().Keys){
			ManagerProtocol.completeTournament(ref worldData, index, worldData.getTournamentFromTownIndex(townIndex).getTournamentResults()[index]);
        }
    }
}
