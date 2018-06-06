using UnityEngine;
using System.Collections.Generic;

public class UpgradableItem : Item
{
	private int level;
    private int levelCap;

	private List<float> baseFactors;
    private List<float> growthFactors;

	public UpgradableItem(string name, float baseValue, int level, int levelCap)
		: base(name, baseValue)
	{
		this.level = level;
		this.levelCap = levelCap;

		baseFactors = new List<float>();
		growthFactors = new List<float>();
	}

    //Getters
    public int Level
	{
		get { return level; }
	}

    public int LevelCap 
	{
		get { return levelCap; }
	}
}
