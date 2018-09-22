using UnityEngine;
using System.Collections.Generic;
using System;

public class MapMenuHandler : MonoBehaviour
{
	public enum MapState
    {
        None, Region, World
    }

	public WorldMapDrawer terrainDrawer;
    public TopLayerDrawer topLayer;
    public RegionDrawer regionDrawer;

	public ListController listController;
    public MapPositionCaster cursor;

	private MapState mapState;
    
    void Start()
    {
		mapState = MapState.None;
    }

	public void manualUpdate(ref DataPool worldData)
	{
		if (mapState.Equals(MapState.World))
        {
            handleWorldInput(ref worldData);
        }
        else if (mapState.Equals(MapState.Region))
        {
            handleRegionInput(ref worldData);
        }
	}

    public void focusList()
	{
		listController.focusOnList();
	}

    public RowInfoInitializer getCurrentTile()
	{
        if (listController.State.Equals(ListController.ListState.Focused))
		{
			return listController.getSelectedRow();
		}

		return convertTileInfoToRowInfo(cursor.CurrentTile);
	}

    public MapState getMapState()
	{
		return mapState;
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

		return null;
	}

	private List<RowInfoInitializer> generateRegionRowInfos(DataPool worldData)
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

	public void setCursorTarget(RowInfoInitializer rowInfo)
	{
		if (rowInfo.Type.Equals(RowInfo.Type.Region))
        {
			cursor.setTarget(new Vector3(rowInfo.Position.x / 2.0f, rowInfo.Position.y / 2.0f, -6.0f));
        }
        else if (rowInfo.Type.Equals(RowInfo.Type.Town))
        {
            Vector3 pos = new Vector3((rowInfo.Position.x + terrainDrawer.XOffset) / 30.0f,
                                      (rowInfo.Position.y + terrainDrawer.YOffset) / 30.0f,
                                      -6.0f);
            cursor.setTarget(pos);
        }
	}

	private List<RowInfoInitializer> generateTownRowInfos(int regionId, DataPool worldData)
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

	private void handleFreeMovement(ref DataPool worldData, InfoLayerJob.InfoJob infoJob)
    {
        if (listController.State.Equals(ListController.ListState.None))
        {         
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (mapState.Equals(MapState.World))
                {
                    loadRegion(ref worldData);
                }
            }
        }
    }

	private void handleListControllers(ref DataPool worldData)
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

	private void handleRegionInput(ref DataPool worldData)
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            loadRegionHub(ref worldData);
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            toggleMapControls();
        }

        handleFreeMovement(ref worldData, InfoLayerJob.InfoJob.TownPreview);
    }

	private void handleWorldInput(ref DataPool worldData)
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            toggleMapControls();
        }

        if (!listController.State.Equals(ListController.ListState.None))
		{
			handleListControllers(ref worldData);
		}

		handleFreeMovement(ref worldData, InfoLayerJob.InfoJob.RegionPreview);
    }
    
	private void loadRegion(ref DataPool worldData)
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

                listController.addRows(generateTownRowInfos(cursor.CurrentTile.ID, worldData));
                listController.focusOnList();
            }
        }
    }

    public void loadRegion(ref DataPool worldData, int id)
    {
        regionDrawer.setActive(false);
        terrainDrawer.setActive(true);
        topLayer.setActive(true);
        terrainDrawer.drawRegion(ref worldData, id);
        mapState = MapState.Region;
        cursor.setMovement(0.08f);
        mapState = MapState.Region;
    }

    public void loadRegionHub(ref DataPool worldData)
    {
        terrainDrawer.setActive(false);
        topLayer.setActive(false);
        regionDrawer.setActive(true);
        mapState = MapState.World;
        regionDrawer.drawRegions(ref worldData);
        listController.addRows(generateRegionRowInfos(worldData));
        listController.focusOnList();
    }

	private void rowClick(ref DataPool worldData, RowInfoInitializer rowInfo)
    {
        if (rowInfo.Type.Equals(RowInfo.Type.Region))
        {
            loadRegion(ref worldData, rowInfo.ID);
            listController.addRows(generateTownRowInfos(rowInfo.ID, worldData));
            listController.focusOnList();
        }
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
}
