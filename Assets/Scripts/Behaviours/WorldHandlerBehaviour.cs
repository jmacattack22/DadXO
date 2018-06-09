using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

public class WorldHandlerBehaviour : MonoBehaviour
{
    public enum ControllerState
	{
		None, Map
	}

    public enum MapState
	{
		None, Region, World
	}

    public WorldMapDrawer mapDrawer;
	public TopLayerDrawer topLayer;
	public RegionDrawer regionDrawer;
	public MapPositionCaster cursor;
	public InfoLayerBehaviour infoLayer;

	private bool creatingNewWorld = false;
	public WorldBuilderBehaviour worldBuilder;

	private DataPool worldData;

	private ControllerState controllerState;
	private MapState mapState;

	void Start()
	{
		worldBuilder.createNewWorld();
		creatingNewWorld = true;
		controllerState = ControllerState.Map;
		mapState = MapState.None;
	}

	void Update()
	{
        if (creatingNewWorld && worldBuilder.State.Equals(WorldBuilderBehaviour.BuilderState.Complete))
		{         
			worldData = worldBuilder.getWorldData();

			worldData.setSaveDirectories("AutoSave1", "AutoSave1");
			Thread saveMapThread = new Thread(new ThreadStart(worldData.saveWorldMapThread));
			saveMapThread.Start();

			mapState = MapState.World;
			regionDrawer.drawRegions(ref worldData);
			creatingNewWorld = false;

			infoLayer.updateWorldData(worldData);
			controllerState = ControllerState.Map;
		}

		handleInput();
	}

    private void handleInput()
	{
		if (controllerState.Equals(ControllerState.Map))
		{
			handleMapInput();
            
		}
	}

	private void handleMapInput()
	{
		if (mapState.Equals(MapState.World))
		{
			handleWorldMapInput();
		}
		else if (mapState.Equals(MapState.Region))
        {
			handleRegionMapInput();
        }
	}

	private void handleRegionMapInput()
	{
		if (Input.GetKeyDown(KeyCode.B))
        {
            mapDrawer.setActive(false);
            regionDrawer.setActive(true);
            regionDrawer.drawRegions(ref worldData);
            mapState = MapState.World;
			cursor.setMovement(0.5f);
			topLayer.cleanTileMap();
        }

		mapDrawer.handleInput();

		MapPositionCaster.CursorPosition cursorPosition = cursor.getCursorPosition();
        if (!cursorPosition.Equals(MapPositionCaster.CursorPosition.Central))
		{
			mapDrawer.panMap(cursorPosition);
			topLayer.panMap(cursorPosition);
		}
	}

	private void handleWorldMapInput()
	{
		if (Input.GetKeyDown(KeyCode.Space))
        {
            TileInfo tile = cursor.CurrentTile;
            regionDrawer.setActive(false);
            mapDrawer.setActive(true);
            mapDrawer.drawRegion(ref worldData, tile.ID);
            mapState = MapState.Region;
			cursor.setMovement(0.04f);
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
