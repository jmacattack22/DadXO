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

        if (listController.Focused)
		{
			RowInfoInitializer rowInfo = listController.getSelectedRow();
			if (rowInfo.ID >= 0)
			{
				infoLayer.sendJob(new InfoLayerJob(InfoLayerJob.InfoJob.RegionPreview, rowInfo.ID));
				cursor.setTarget(rowInfo.Position / 2.0f);
			}
		}
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
