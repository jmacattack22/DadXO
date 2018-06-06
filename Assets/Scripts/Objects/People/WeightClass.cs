using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightClass {

	public enum WClass
	{
		FlyWeight, LightWeight, WelterWeight, MiddleWeight, CruiserWeight, HeavyWeight
	}

	private WClass weightClass;

	public WeightClass(float weight){
		weightClass = determineWeightClass (weight);
	}

	private WClass determineWeightClass(float weight){
		if (weight > 200.0f)
			return WClass.HeavyWeight;
		else if (weight > 175.0f)
			return WClass.CruiserWeight;
		else if (weight > 160.0f)
			return WClass.MiddleWeight;
		else if (weight > 140.0f)
			return WClass.WelterWeight;
		else if (weight > 120.0f)
			return WClass.LightWeight;

		return WClass.FlyWeight;
	}

	public void updateWeight(float weight){
		weightClass = determineWeightClass (weight);
	}
   
	public WClass Class {
		get { return weightClass; }
	}
}
