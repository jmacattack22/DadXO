﻿using UnityEngine;
using System.Collections.Generic;

public static class InfoLayerTuple
{
	public static Dictionary<InfoLayerBehaviour.Labels, string> getRegionTuple(DataPool worldData, int regionIndex)
	{
		Dictionary<InfoLayerBehaviour.Labels, string> tuple = new Dictionary<InfoLayerBehaviour.Labels, string>();

		Region region = worldData.Regions[regionIndex];

		tuple.Add(InfoLayerBehaviour.Labels.Title, region.Name);
		tuple.Add(InfoLayerBehaviour.Labels.RightInfoPanelTitle, region.Name);
		tuple.Add(InfoLayerBehaviour.Labels.RightInfoPanelDetail, (region.getRegionsTownIndexes().Count + 1).ToString());

		return tuple;
	}

	public static Dictionary<InfoLayerBehaviour.Labels, string> getTownTuple(DataPool worldData, int townIndex)
    {
        Dictionary<InfoLayerBehaviour.Labels, string> tuple = new Dictionary<InfoLayerBehaviour.Labels, string>();

		Town town = worldData.Towns[townIndex];

        tuple.Add(InfoLayerBehaviour.Labels.Title, town.Name);
        tuple.Add(InfoLayerBehaviour.Labels.RightInfoPanelTitle, town.Name);
        tuple.Add(InfoLayerBehaviour.Labels.RightInfoPanelDetail, town.Tournament.Name);

        return tuple;
    }
}
