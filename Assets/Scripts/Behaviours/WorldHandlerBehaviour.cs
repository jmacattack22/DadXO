using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

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

	public void logBoxerResults()
    {
		List<ManagerProtocol> managers = worldData.ManagerProtocols.OrderByDescending(m => EvaluationProtocol.evaluateBoxer(worldData.Boxers[m.BoxerIndex])).ToList();

        foreach (ManagerProtocol mp in managers)
		{
			worldData.Boxers[mp.BoxerIndex].logBoxerStats(mp.Rank);
		}
    }

    public void logManagerResults()
    {
        foreach (ManagerProtocol mp in worldData.ManagerProtocols)
        {
            mp.logManagerStats(ref worldData);
        }
    }

	public void distribution()
    {
        foreach (TournamentProtocol.Level rank in worldData.Distribution.Keys)
        {
            Debug.Log(rank.ToString() + " - " + worldData.Distribution[rank]);
        }
    }

    //Getters
    public DataPool Data {
        get { return worldData; }
    }
}
