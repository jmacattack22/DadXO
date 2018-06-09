using UnityEngine;
using System.Collections.Generic;

public class Item
{
	private string name;

	private float baseValue;
   
	public Item(string name, float baseValue)
	{
		this.name = name;

		this.baseValue = baseValue;
	}


    //Getters
    public string Name
	{
		get { return Name; }
	}

    public float BaseValue
	{
		get { return baseValue; }
	}
}
