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

	public WorldMapDrawer mapDrawer;
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
		if (creatingNewWorld && worldBuilder.State.Equals(WorldBuilderBehaviour.BuilderState.Complete))
        {
            worldData = worldBuilder.getWorldData();

            worldData.setSaveDirectories("AutoSave1", "AutoSave1");
            Thread saveMapThread = new Thread(new ThreadStart(worldData.saveWorldMapThread));
            saveMapThread.Start();

            mapState = MapState.World;
            regionDrawer.drawRegions(ref worldData);
			infoLayer.updateWorldData(worldData);
			listController.addRows(generateRegionRowInfos());
			listController.focusOnList();
            creatingNewWorld = false;
            
            controllerState = ControllerState.Map;
        }

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
            cursor.setTarget(rowInfo.Position / 2.0f);
		}
		else if (rowInfo.Type.Equals(RowInfo.Type.Town))
		{
			infoLayer.sendJob(new InfoLayerJob(InfoLayerJob.InfoJob.TownPreview, rowInfo.ID));
			cursor.setTarget(rowInfo.Position / 30.0f);
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
    
    public void loadRegion(int id)
	{
		regionDrawer.setActive(false);
        mapDrawer.setActive(true);
        mapDrawer.drawRegion(ref worldData, id);
        mapState = MapState.Region;
        cursor.setMovement(0.04f); 
	}

	//Getters
	public DataPool Data
    {
        get { return worldData; }
    }
}
