using UnityEngine;
using System.Collections;
using System.Threading;

public class WorldHandlerBehaviour : MonoBehaviour
{
    public WorldMapDrawer mapDrawer;

	private bool creatingNewWorld = false;
	public WorldBuilderBehaviour worldBuilder;
	private Thread worldBuilderThread;

	private bool tournamentsInProgress;
	public TournamentHandlerBehaviour tournamentHandler;
	private Thread tournamentHandlerThread;

    private DataPool worldData;

	void Start()
	{
		worldBuilder.createNewWorld();
		creatingNewWorld = true;
		tournamentsInProgress = false;
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
		if (!tournamentsInProgress)
		{
			tournamentsInProgress = true;
			tournamentHandler.updateWorldData(worldData);
			tournamentHandlerThread = new Thread(new ThreadStart(tournamentHandler.simTournamentsAndTraining));
			tournamentHandlerThread.Start();
		}
    }

	public void logBoxerResults()
    {
        foreach (ManagerProtocol mp in worldData.ManagerProtocols)
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
