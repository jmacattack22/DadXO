using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class StandardMenuHandler: MonoBehaviour
{
	public void advanceWeek(ref DataPool worldData)
    {
        TournamentHandlerProtocol.simTournamentsAndTraining(ref worldData);
        worldData.updateBoxerDistribution();

        worldData.Calendar.progessWeek();
        Debug.Log(worldData.Calendar.getDate(Calendar.DateType.fullLong));
    }

	public void advanceFourYears(ref DataPool worldData)
    {
        for (int i = 0; i < 4; i++)
        {
            for (int w = 0; w < 48; w++)
            {
                advanceWeek(ref worldData);
            }
        }
    }

	public void loadGameButton(ref DataPool worldData)
    {
		loadGame(ref worldData, "NewGame", "NewGame");
    }

	public void loadGame(ref DataPool worldData, string saveDirectory, string saveFile)
    {
        worldData = new DataPool();
        worldData.loadWorld(saveDirectory, saveFile);
        WorldBuilderProtocol.initExercises(ref worldData);
    }

	public void logBoxerResults(DataPool worldData)
    {
        List<Manager> managers = worldData.Managers.OrderByDescending(m => EvaluationProtocol.evaluateBoxer(worldData.Boxers[m.BoxerIndex])).ToList();

        foreach (Manager m in managers)
        {
            worldData.Boxers[m.BoxerIndex].logBoxerStats(m.Rank);
        }
    }

	public void logManagerResults(DataPool worldData)
    {
        foreach (Manager mp in worldData.Managers)
        {
            mp.logManagerStats();
        }
    }

	public void distribution(ref DataPool worldData)
    {
        foreach (TournamentProtocol.Level rank in worldData.Distribution.Keys)
        {
            Debug.Log(rank.ToString() + " - " + worldData.Distribution[rank]);
        }
    }

	public void saveGame(ref DataPool worldData)
    {
        worldData.saveWorld("NewGame", "NewGame");
    }
}
