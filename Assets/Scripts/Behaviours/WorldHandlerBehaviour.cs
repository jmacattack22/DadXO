using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System;

public class WorldHandlerBehaviour : MonoBehaviour
{
    public enum ControllerState
	{
		None, Map, Stats
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
	public ListController listController;

	public UIHandlerBehaviour uIHandler;

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

			infoLayer.updateWorldData(worldData);

			loadRegionHub();
         
			creatingNewWorld = false;
		}

		handleInput();
		handleListControllers();
	}

	private void checkForMapPanning()
    {
        MapPositionCaster.CursorPosition cursorPosition = cursor.getCursorPosition();
        if (!cursorPosition.Equals(MapPositionCaster.CursorPosition.Central))
        {
            mapDrawer.panMap(cursorPosition);
            topLayer.panMap(cursorPosition);
        }
    }

	private RowInfoInitializer convertTileInfoToRowInfo(TileInfo currentTile)
    {
        if (currentTile != null)
        {
            if (currentTile.ID >= 0)
            {
                return new RowInfoInitializer(
                currentTile.IsRegion ? RowInfo.Type.Region : RowInfo.Type.Town,
                currentTile.ID,
                currentTile.Position,
                "");
            }
        }

		return new RowInfoInitializer(
                RowInfo.Type.Town,
                -1,
			    new Vector2(0f,0f),
                "");
    }

    private RowInfoInitializer getCurrentTile()
	{
		if (listController.State.Equals(ListController.ListState.Focused))
		{
			return listController.getSelectedRow();
		}

		return convertTileInfoToRowInfo(cursor.CurrentTile);
	}

	private void checkHoverTile(InfoLayerJob.InfoJob job)
    {
		RowInfoInitializer tile = getCurrentTile();

		if (tile.ID >= 0)
		{
			infoLayer.sendJob(new InfoLayerJob(job, tile.ID));
			highlightSelection(tile);
		}
		else
		{
			infoLayer.sendJob(new InfoLayerJob(InfoLayerJob.InfoJob.Clear, 0));
		}
    }

	private void highlightSelection(RowInfoInitializer tile)
	{
		if (tile.Type.Equals(RowInfo.Type.Region))
		{
			regionDrawer.highlightRegion(tile);
		}
		else if (tile.Type.Equals(RowInfo.Type.Town))
        {
			topLayer.highlightTown(tile);
        }
	}

	private void handleInput()
	{
		handleUIListeners();

		if (controllerState.Equals(ControllerState.Map))
		{
			handleMapInput();
		}
	}

	private void handleUIListeners()
	{
		if (Input.GetKeyDown(KeyCode.M))
		{
			if (uIHandler.isDisplaying())
			{
				uIHandler.hideAllUI();
				controllerState = ControllerState.None;
			}
			else
			{
				uIHandler.showUI(UIHandlerBehaviour.Type.Map);
				infoLayer.sendJob(new InfoLayerJob(InfoLayerJob.InfoJob.Parameters, 5));
				controllerState = ControllerState.Map;
			}
		}

        if (!controllerState.Equals(ControllerState.None))
		{
			if (Input.GetKeyDown(KeyCode.R))
			{
				uIHandler.showUI(uIHandler.getPreviousType());
			}
			else if (Input.GetKeyDown(KeyCode.T))
			{
				uIHandler.showUI(uIHandler.getNextType());	
			}
		}
	}

	private void handleListControllers()
    {
        if (listController.State.Equals(ListController.ListState.Clicked))
        {
            RowInfoInitializer rowInfo = listController.getSelectedRow();
            if (rowInfo.ID >= 0)
            {
                rowClick(ref worldData, rowInfo);
            }
            listController.acknowledgeClick();
        }

        listController.resolveSelection();
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
			loadRegionHub();
		}

		mapDrawer.handleInput();

		checkHoverTile(InfoLayerJob.InfoJob.Town);
	}   

	private void handleWorldMapInput()
	{
		//if (Input.GetKeyDown(KeyCode.Space))
		//{
		//	loadRegion();
		//}



		checkHoverTile(InfoLayerJob.InfoJob.Region);
	}
       
	private void loadRegion()
	{
		if (cursor.CurrentTile != null)
		{
			if (cursor.CurrentTile.ID >= 0)
			{
				regionDrawer.setActive(false);
				regionDrawer.toggleLegend(false);
				mapDrawer.setActive(true);
				mapDrawer.drawRegion(ref worldData, cursor.CurrentTile.ID);
				mapState = MapState.Region;
				listController.addRows(MapMenuUtils.generateTownRowInfos(cursor.CurrentTile.ID, ref worldData));
                listController.focusOnList();
				cursor.setMovement(0.04f);            
			}
		}
	}

    private void loadRegion(int regionIndex)
	{
        regionDrawer.setActive(false);
		regionDrawer.toggleLegend(false);
        mapDrawer.setActive(true);
        mapDrawer.drawRegion(ref worldData, regionIndex);
        mapState = MapState.Region;
		listController.addRows(MapMenuUtils.generateTownRowInfos(regionIndex, ref worldData));
        listController.focusOnList();
        cursor.setMovement(0.04f);  
	}

	private void loadRegionHub()
    {
        mapDrawer.setActive(false);
        regionDrawer.setActive(true);
		regionDrawer.toggleLegend(true);
        regionDrawer.drawRegions(ref worldData);
        mapState = MapState.World;
		cursor.setMovement(0.5f);
		listController.addRows(MapMenuUtils.generateRegionRowInfos(ref worldData));
        listController.focusOnList();
        topLayer.cleanTileMap();
    }

	private void rowClick(ref DataPool worldData, RowInfoInitializer rowInfo)
    {
        if (rowInfo.Type.Equals(RowInfo.Type.Region))
        {
			loadRegion(rowInfo.ID);
            listController.addRows(MapMenuUtils.generateTownRowInfos(rowInfo.ID, ref worldData));
            listController.focusOnList();
        }
    }

	public void advanceWeek(){
		TournamentHandlerProtocol.simTournamentsAndTraining(ref worldData);
		worldData.updateBoxerDistribution();

        worldData.Calendar.progessWeek();
        Debug.Log(worldData.Calendar.getDate(Calendar.DateType.fullLong));

		infoLayer.updateWorldData(worldData);
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
