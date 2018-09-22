using UnityEngine;
using System.Collections.Generic;

public static class MapMenuUtils
{
	public static List<RowInfoInitializer> generateRegionRowInfos(ref DataPool worldData)
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

	public static List<RowInfoInitializer> generateTownRowInfos(int regionId, ref DataPool worldData)
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
}
