using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class WorldHandlerBehaviour : MonoBehaviour
{
    public WorldMapDrawer mapDrawer;

	public InfoLayerBehaviour infoLayer;

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

			worldData.setSaveDirectories("AutoSave1", "AutoSave1");
			Thread saveMapThread = new Thread(new ThreadStart(worldData.saveWorldMapThread));
			saveMapThread.Start();

			mapDrawer.drawRegions(ref worldData);
			creatingNewWorld = false;

			infoLayer.updateWorldData(worldData);
		}    

		if (!creatingNewWorld)
		{
			if (!mapDrawer.ViewingRegions)
            {
                if (mapDrawer.RegionToView != -1)
                {
                    mapDrawer.drawRegion(ref worldData, mapDrawer.RegionToView);
                    mapDrawer.setRegionToView(-1);
                }
            }
            else
            {
                if (!mapDrawer.RegionsDrawn)
                {
                    mapDrawer.drawRegions(ref worldData);
                }
            }
		}
      
	}

    public void advanceWeek(){
		TournamentHandlerProtocol.simTournamentsAndTraining(ref worldData);
		worldData.updateBoxerDistribution();

        worldData.Calendar.progessWeek();
        Debug.Log(worldData.Calendar.getDate(Calendar.DateType.fullLong));
    }

    public void advanceFourYears()
	{
		for (int i = 0; i < 4; i++)
		{
			for (int w = 0; w < 48; w++)
			{
				advanceWeek();
			}
		}
	}

    public void loadGameButton()
	{
		loadGame("NewGame", "NewGame");
	}

	public void loadGame(string saveDirectory, string saveFile)
	{
		worldData = new DataPool();
		worldData.loadWorld(saveDirectory, saveFile);
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
		worldData.saveWorld("NewGame", "NewGame");
	}

    //Getters
    public DataPool Data {
        get { return worldData; }
    }
}
