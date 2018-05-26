using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ManagerProtocol
{
    public enum FacilityShortcut
    {
        DoubleEndBag, PunchGlove, Laps, Sprints, PunchingBag
    }  

    public static void completeTournament(ref DataPool worldData, int managerIndex, TournamentResult result)
    {
		int boxerIndex = worldData.Managers[managerIndex].BoxerIndex;
		worldData.Managers[managerIndex].leaveTournament();

        worldData.Managers[managerIndex].Record.addWinBulk(result.Record.Wins);
        worldData.Managers[managerIndex].Record.addLossBulk(result.Record.Losses);
        worldData.Managers[managerIndex].Record.addTieBulk(result.Record.Ties);

        worldData.Boxers[boxerIndex].Record.addWinBulk(result.Record.Wins);
        worldData.Boxers[boxerIndex].Record.addLossBulk(result.Record.Losses);
        worldData.Boxers[boxerIndex].Record.addTieBulk(result.Record.Ties);

        if (result.QuarterlyWin)
		{
			worldData.Managers[managerIndex].graduateRank();
        }
    }

    private static void disposeAndRenewBoxer(ref DataPool worldData, int managerIndex)
    {
		float currentManagerELO = worldData.Managers[managerIndex].ManagerELO;
        List<Boxer> boxers = WorldBuilderProtocol.generateBoxerRecruits(ref worldData, worldData.Managers[managerIndex].TownIndex, currentManagerELO);

        int bIndex = 0;
        float max = 0.0f;

        for (int i = 0; i < boxers.Count; i++)
        {
            float boxerEval = EvaluationProtocol.evaluateBoxer(boxers[i], worldData.Managers[managerIndex].Preference);

            if (boxerEval > max)
            {
                max = boxerEval;
                bIndex = i;
            }
        }

        worldData.Boxers.Add(boxers[bIndex]);

		worldData.Managers[managerIndex].recruitBoxer(worldData.Boxers.Count - 1);
        updateELO(ref worldData, managerIndex);
    }

    public static void executeWeek(ref DataPool worldData, int managerIndex)
    {
		int boxerIndex = worldData.Managers[managerIndex].BoxerIndex;

        if (!worldData.Managers[managerIndex].isBusy()){
            if (worldData.Boxers[boxerIndex].isBoxerFatigued())
            {
                worldData.Boxers[boxerIndex].rest();
            }
            else
            {
				worldData.Managers[managerIndex].train(ref worldData);
            }
        }

        worldData.Boxers[boxerIndex].ageWeek();

        if (worldData.Boxers[boxerIndex].WeeksRemaining == 0)
        {
            disposeAndRenewBoxer(ref worldData, managerIndex);
			worldData.Managers[managerIndex].setRank(TournamentProtocol.Level.E);
        }
    }

	private static int generateRandomInt(int min, int max)
	{
		return new System.Random((int)System.DateTime.Now.Ticks).Next(min, max);
	}

    public static void updateELO(ref DataPool worldData, int managerIndex)
    {
		int boxerIndex = worldData.Managers[managerIndex].BoxerIndex;

		if (worldData.Managers[managerIndex].BoxerELO > 0)
			worldData.Managers[managerIndex].archiveBoxerELO();

		if (worldData.Managers[managerIndex].ManagerELO > 0)
			worldData.Managers[managerIndex].archiveManagerELO();

		worldData.Managers[managerIndex].setBoxerELO(worldData.Boxers[boxerIndex].Record.ELO);
		worldData.Managers[managerIndex].setManagerELO(worldData.Managers[managerIndex].Record.ELO);
    }
}
