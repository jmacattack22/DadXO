using UnityEngine;
using System.Collections.Generic;

public static class InfoLayerTuple
{
	public static Dictionary<InfoLayerBehaviour.Labels, string> getRegionTuple(DataPool worldData, int regionIndex)
	{
		Dictionary<InfoLayerBehaviour.Labels, string> tuple = new Dictionary<InfoLayerBehaviour.Labels, string>();

		Region region = worldData.Regions[regionIndex];
        
		tuple.Add(InfoLayerBehaviour.Labels.MapRegionTitle, region.Name);
		tuple.Add(InfoLayerBehaviour.Labels.MapTownTitle, "");
		tuple.Add(InfoLayerBehaviour.Labels.RightInfoPanelTitle, region.Name);
		tuple.Add(InfoLayerBehaviour.Labels.RightInfoPanelSubTitle, (region.getRegionsTownIndexes().Count + 1).ToString() + " Towns");

		return tuple;
	}

	public static Dictionary<InfoLayerBehaviour.Labels, string> getTownTuple(DataPool worldData, int townIndex)
    {
        Dictionary<InfoLayerBehaviour.Labels, string> tuple = new Dictionary<InfoLayerBehaviour.Labels, string>();

		Town town = worldData.Towns[townIndex];

		tuple.Add(InfoLayerBehaviour.Labels.MapRegionTitle, 
		          worldData.Regions[WorldDetailProtocol.getRegionIndexFromTownIndex(ref worldData, townIndex)].Name + " : ");
		tuple.Add(InfoLayerBehaviour.Labels.MapTownTitle, town.Name);
        tuple.Add(InfoLayerBehaviour.Labels.RightInfoPanelTitle, town.Name);
        tuple.Add(InfoLayerBehaviour.Labels.RightInfoPanelSubTitle, town.Tournament.Name);

        return tuple;
    }

    public static Dictionary<InfoLayerBehaviour.Labels, string> getBoxerTuple(DataPool worldData, int boxerIndex)
	{
		Dictionary<InfoLayerBehaviour.Labels, string> tuple = new Dictionary<InfoLayerBehaviour.Labels, string>();

		Boxer boxer = worldData.Boxers[boxerIndex];

		tuple.Add(InfoLayerBehaviour.Labels.Accuracy, boxer.Accuracy.ToString());
		tuple.Add(InfoLayerBehaviour.Labels.Endurance, boxer.Endurance.ToString());
		tuple.Add(InfoLayerBehaviour.Labels.Health, boxer.Health.ToString());
		tuple.Add(InfoLayerBehaviour.Labels.Speed, boxer.Speed.ToString());
		tuple.Add(InfoLayerBehaviour.Labels.Strength, boxer.Strength.ToString());

		tuple.Add(InfoLayerBehaviour.Labels.AccuracyLevel, "Lv. " + calculateLevel(boxer.Accuracy));
		tuple.Add(InfoLayerBehaviour.Labels.EnduranceLevel, "Lv. " + calculateLevel(boxer.Endurance));
		tuple.Add(InfoLayerBehaviour.Labels.HealthLevel, "Lv. " + calculateLevel(boxer.Health));
		tuple.Add(InfoLayerBehaviour.Labels.SpeedLevel, "Lv. " + calculateLevel(boxer.Speed));
		tuple.Add(InfoLayerBehaviour.Labels.StrengthLevel, "Lv. " + calculateLevel(boxer.Strength));

		return tuple;
	}

    private static int calculateLevel(int value)
	{
		return Mathf.RoundToInt((value - (value % 50.0f)) / 50.0f);
	}
}
