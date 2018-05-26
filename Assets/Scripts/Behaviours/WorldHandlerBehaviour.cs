using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class WorldHandlerBehaviour : MonoBehaviour
{
    public WorldMapDrawer mapDrawer;

	private bool creatingNewWorld = false;
	public WorldBuilderBehaviour worldBuilder;

	private DataPool worldData;

	void Start()
	{
		worldBuilder.createNewWorld();
		creatingNewWorld = true;
	}

	void Update()
	{
        if (creatingNewWorld && worldBuilder.State.Equals(WorldBuilderBehaviour.BuilderState.Complete))
		{
			worldData = worldBuilder.getWorldData();
			mapDrawer.drawRegions(ref worldData);
			creatingNewWorld = false;
		}       
	}

    public void advanceWeek(){
		TournamentHandlerProtocol.simTournamentsAndTraining(ref worldData);
		worldData.updateBoxerDistribution();

        worldData.Calendar.progessWeek();
        Debug.Log(worldData.Calendar.getDate(Calendar.DateType.fullLong));
    }

	public void loadGame(string filename)
	{
		worldData = new DataPool();
		worldData.loadWorld(filename);
		WorldBuilderProtocol.initExercises(ref worldData);
	}

	public void logBoxerResults()
    {
		List<Manager> managers = worldData.Managers.OrderByDescending(m => EvaluationProtocol.evaluateBoxer(worldData.Boxers[m.BoxerIndex])).ToList();

        foreach (Manager m in managers)
		{
			worldData.Boxers[m.BoxerIndex].logBoxerStats(m.Rank);
		}
    }

    public void logManagerResults()
    {
        foreach (Manager mp in worldData.Managers)
        {
            mp.logManagerStats();
        }
    }

	public void distribution()
    {
        foreach (TournamentProtocol.Level rank in worldData.Distribution.Keys)
        {
            Debug.Log(rank.ToString() + " - " + worldData.Distribution[rank]);
        }
    }

    public void saveGame()
	{
		worldData.saveWorld("Assets/Resources/Saves/gamesave.txt");
	}

    //Getters
    public DataPool Data {
        get { return worldData; }
    }
}
