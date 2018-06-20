using UnityEngine;
using System.Threading;
using System.Collections;
using System;
using System.Collections.Generic;

public class WorldHandlerTest : MonoBehaviour
{
	public enum ControllerState
    {
        None, Map, WorldCreation
    }
    
	public InfoLayerBehaviour infoLayer;
	public WorldBuilderMenuHandler worldBuilderMenu;
	public UIHandlerBehaviour uIHandler;
	public MapMenuHandler mapMenu;

    private DataPool worldData;

    private ControllerState controllerState;

    void Start()
    {
		controllerState = ControllerState.WorldCreation;
		worldBuilderMenu.createWorld();
    }
    
    void Update()
	{
		if (controllerState.Equals(ControllerState.WorldCreation) && !worldBuilderMenu.creationInProgress)
		{
			worldData = worldBuilderMenu.extractWorldData();
			//infoLayer.updateWorldData(worldData);
			mapMenu.loadRegionHub(ref worldData);
			controllerState = ControllerState.Map;
		}
		else if (controllerState.Equals(ControllerState.Map))
		{
			mapMenu.manualUpdate(ref worldData);

            if (mapMenu.getMapState().Equals(MapMenuHandler.MapState.World))
			{
				constructAndSendJob(mapMenu.getCurrentTile());
			}
			else if (mapMenu.getMapState().Equals(MapMenuHandler.MapState.Region))
            {
				checkHoverTile(InfoLayerJob.InfoJob.TownPreview);
            }
		}
		else
		{
			
		}

        if (Input.GetKeyDown(KeyCode.T))
		{
			uIHandler.showUI(UIHandlerBehaviour.Type.Map);
			mapMenu.focusList();
			controllerState = ControllerState.Map;
		}

        if (Input.GetKeyDown(KeyCode.Y))
		{
			uIHandler.hideAllUI();
			controllerState = ControllerState.None;
		}
	}

	private void checkHoverTile(InfoLayerJob.InfoJob job)
    {
		RowInfoInitializer rowInfo = mapMenu.getCurrentTile();

		if (rowInfo != null)
		{
			if (rowInfo.ID >= 0)
			{
				//infoLayer.sendJob(new InfoLayerJob(job, rowInfo.ID));
			}
		}
		else
		{
			//infoLayer.sendJob(new InfoLayerJob(InfoLayerJob.InfoJob.Clear, 0));
		}
    }

	private void constructAndSendJob(RowInfoInitializer rowInfo)
	{
		if (rowInfo.Type.Equals(RowInfo.Type.Region))
		{
			//infoLayer.sendJob(new InfoLayerJob(InfoLayerJob.InfoJob.RegionPreview, rowInfo.ID));
			mapMenu.setCursorTarget(rowInfo);
		}
		else if (rowInfo.Type.Equals(RowInfo.Type.Town))
		{
			infoLayer.sendJob(new InfoLayerJob(InfoLayerJob.InfoJob.TownPreview, rowInfo.ID));
			mapMenu.setCursorTarget(rowInfo);
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

	//Getters
	public DataPool Data
    {
        get { return worldData; }
    }
}
