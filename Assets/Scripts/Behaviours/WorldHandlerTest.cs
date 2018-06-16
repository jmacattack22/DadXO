using UnityEngine;
using System.Threading;
using System.Collections;
using System;
using System.Collections.Generic;

public class WorldHandlerTest : MonoBehaviour
{
	public enum ControllerState
    {
        None, Map
    }

    public enum MapState
    {
        None, Region, World
    }

	public WorldMapDrawer terrainDrawer;
    public TopLayerDrawer topLayer;
    public RegionDrawer regionDrawer;

	public ListController listController;
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
		newWorldCreationCheck();

		handleListControllers();

		handleInput();
	}

	private void handleInput()
	{
		if (mapState.Equals(MapState.World))
		{
			handleWorldInput();
		}
		else if (mapState.Equals(MapState.Region))
		{
			handleRegionInput();
		}
	}

	private void handleWorldInput()
	{
		if (Input.GetKeyDown(KeyCode.Tab))
		{
			toggleMapControls();
		}

		handleFreeMovement(InfoLayerJob.InfoJob.RegionPreview);
	}

	private void toggleMapControls()
	{
		if (listController.State.Equals(ListController.ListState.Focused))
		{
			listController.unfocusList();

			if (mapState.Equals(MapState.World))
			{
				cursor.setMovement(0.5f);
			}
			else if (mapState.Equals(MapState.Region))
			{
				cursor.setMovement(0.16f);
			}
		}
		else if (listController.State.Equals(ListController.ListState.None))
        {
			listController.focusOnList();

			if (mapState.Equals(MapState.World))
            {
                cursor.setMovement(10.0f);
            }
            else if (mapState.Equals(MapState.Region))
            {
                cursor.setMovement(0.08f);
            }
        }
	}

	private void handleRegionInput()
	{
		if (Input.GetKeyDown(KeyCode.B))
		{
			loadRegionHub();
		}

		if (Input.GetKeyDown(KeyCode.Tab))
		{
			toggleMapControls();
		}

		handleFreeMovement(InfoLayerJob.InfoJob.TownPreview);
	}

	private void handleFreeMovement(InfoLayerJob.InfoJob infoJob)
	{
		if (listController.State.Equals(ListController.ListState.None))
		{
			checkHoverTile(infoJob);

            if (Input.GetKeyDown(KeyCode.Space))
			{
				if (mapState.Equals(MapState.World))
				{
					loadRegion();
				}            
			}
		}
	}

	private void checkHoverTile(InfoLayerJob.InfoJob job)
    {
        if (cursor.CurrentTile != null)
        {
            if (cursor.CurrentTile.ID >= 0)
            {
                infoLayer.sendJob(new InfoLayerJob(job, cursor.CurrentTile.ID));
            }
        }
        else
        {
            infoLayer.sendJob(new InfoLayerJob(InfoLayerJob.InfoJob.Clear, 0));
        }
    }

	private void handleListControllers()
	{
		if (listController.State.Equals(ListController.ListState.Focused))
		{
			RowInfoInitializer rowInfo = listController.getSelectedRow();
			if (rowInfo.ID >= 0)
			{
				constructAndSendJob(rowInfo);
			}
		}
		else if (listController.State.Equals(ListController.ListState.Clicked))
		{
			RowInfoInitializer rowInfo = listController.getSelectedRow();
			if (rowInfo.ID >= 0)
			{
				rowClick(rowInfo);
			}
			listController.acknowledgeClick();
		}

		listController.resolveSelection();
	}

	private void newWorldCreationCheck()
	{
		if (creatingNewWorld && worldBuilder.State.Equals(WorldBuilderBehaviour.BuilderState.Complete))
		{
			worldData = worldBuilder.getWorldData();

			worldData.setSaveDirectories("AutoSave1", "AutoSave1");
			Thread saveMapThread = new Thread(new ThreadStart(worldData.saveWorldMapThread));
			saveMapThread.Start();

			loadRegionHub();

			controllerState = ControllerState.Map;
		}
	}

	private void rowClick(RowInfoInitializer rowInfo)
	{
		if (rowInfo.Type.Equals(RowInfo.Type.Region))
		{
			loadRegion(rowInfo.ID);
            listController.addRows(generateTownRowInfos(rowInfo.ID));
            listController.focusOnList();	
		}
	}

	private void constructAndSendJob(RowInfoInitializer rowInfo)
	{
		if (rowInfo.Type.Equals(RowInfo.Type.Region))
		{
			infoLayer.sendJob(new InfoLayerJob(InfoLayerJob.InfoJob.RegionPreview, rowInfo.ID));
			cursor.setTarget(new Vector3(rowInfo.Position.x / 2.0f, rowInfo.Position.y / 2.0f, -6.0f));
		}
		else if (rowInfo.Type.Equals(RowInfo.Type.Town))
		{
			infoLayer.sendJob(new InfoLayerJob(InfoLayerJob.InfoJob.TownPreview, rowInfo.ID));
			Vector3 pos = new Vector3((rowInfo.Position.x + terrainDrawer.Offset) / 30.0f,
									  (rowInfo.Position.y + terrainDrawer.Offset) / 30.0f,
									  -6.0f);
			cursor.setTarget(pos);
		}
	}

	private List<RowInfoInitializer> generateTownRowInfos(int regionId)
	{
		List<RowInfoInitializer> rowInfos = new List<RowInfoInitializer>();

		foreach (int index in worldData.Regions[regionId].getRegionsTownIndexes())
		{
			RowInfoInitializer infoInitializer = new RowInfoInitializer(
				RowInfo.Type.Town, index, worldData.Towns[index].Location, worldData.Towns[index].Name);
			rowInfos.Add(infoInitializer);
		}

        return rowInfos;
	}

	private List<RowInfoInitializer> generateRegionRowInfos()
	{
		List<RowInfoInitializer> rowInfos = new List<RowInfoInitializer>();

		for (int i = 0; i < worldData.Regions.Count; i++)
		{
			RowInfoInitializer info = new RowInfoInitializer(
				RowInfo.Type.Region, i, worldData.Regions[i].Position, worldData.Regions[i].Name);
			rowInfos.Add(info);
		}

		return rowInfos;
	}

	private void loadRegion()
    {
        if (cursor.CurrentTile != null)
        {
            if (cursor.CurrentTile.ID >= 0)
            {
                regionDrawer.setActive(false);
				terrainDrawer.setActive(true);
                topLayer.setActive(true);
                terrainDrawer.drawRegion(ref worldData, cursor.CurrentTile.ID);
                mapState = MapState.Region;
                cursor.setMovement(0.08f);

				listController.addRows(generateTownRowInfos(cursor.CurrentTile.ID));
                listController.focusOnList();
            }
        }
    }
    
    public void loadRegion(int id)
	{
		regionDrawer.setActive(false);
        terrainDrawer.setActive(true);
		topLayer.setActive(true);
        terrainDrawer.drawRegion(ref worldData, id);
        mapState = MapState.Region;
        cursor.setMovement(0.08f);
		mapState = MapState.Region;
	}

    public void loadRegionHub()
	{
		terrainDrawer.setActive(false);
		topLayer.setActive(false);
		regionDrawer.setActive(true);  
		mapState = MapState.World;
        regionDrawer.drawRegions(ref worldData);
        infoLayer.updateWorldData(worldData);
        listController.addRows(generateRegionRowInfos());
        listController.focusOnList();
        creatingNewWorld = false;
	}

	//Getters
	public DataPool Data
    {
        get { return worldData; }
    }
}
