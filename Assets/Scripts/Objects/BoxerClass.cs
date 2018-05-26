using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BoxerClass
{
	public enum Type
	{
		Legion, Ranger, Gladiator, Monk, BushWacker, Bullseye, GentleGiant, Slugger, Juggernaut, DoubleEdge
	}

	public static Type getTypeFromJson(JSONObject json)
	{
		string type = json.str;

		if (type.Equals("Legion"))
			return Type.Legion;
		else if (type.Equals("Ranger"))
			return Type.Ranger;
		else if (type.Equals("Gladiator"))
			return Type.Gladiator;
		else if (type.Equals("Monk"))
			return Type.Monk;
		else if (type.Equals("BushWacker"))
			return Type.BushWacker;
		else if (type.Equals("Bullseye"))
			return Type.Bullseye;
		else if (type.Equals("GentleGiant"))
			return Type.GentleGiant;
		else if (type.Equals("Slugger"))
			return Type.Slugger;
		else if (type.Equals("Juggernaut"))
			return Type.Juggernaut;
		else if (type.Equals("DoubleEdge"))
			return Type.DoubleEdge;

		return Type.Legion;
	}
		
	public static List<EvaluationProtocol.Stats> getBuild(Type type)
	{
		if (type.Equals (Type.Legion))
			return new List<EvaluationProtocol.Stats> (new EvaluationProtocol.Stats[] {
				EvaluationProtocol.Stats.AccuracyGrowth,
				EvaluationProtocol.Stats.EnduranceGrowth,
				EvaluationProtocol.Stats.HealthGrowth
			});
		else if (type.Equals (Type.Ranger))
			return new List<EvaluationProtocol.Stats> (new EvaluationProtocol.Stats[] {
				EvaluationProtocol.Stats.AccuracyGrowth,
				EvaluationProtocol.Stats.EnduranceGrowth,
				EvaluationProtocol.Stats.SpeedGrowth
			});
		else if (type.Equals (Type.Gladiator))
			return new List<EvaluationProtocol.Stats> (new EvaluationProtocol.Stats[] {
				EvaluationProtocol.Stats.AccuracyGrowth,
				EvaluationProtocol.Stats.EnduranceGrowth,
				EvaluationProtocol.Stats.StrengthGrowth
			});
		else if (type.Equals (Type.Monk))
			return new List<EvaluationProtocol.Stats> (new EvaluationProtocol.Stats[] {
				EvaluationProtocol.Stats.AccuracyGrowth,
				EvaluationProtocol.Stats.HealthGrowth,
				EvaluationProtocol.Stats.SpeedGrowth
			});
		else if (type.Equals (Type.BushWacker))
			return new List<EvaluationProtocol.Stats> (new EvaluationProtocol.Stats[] {
				EvaluationProtocol.Stats.AccuracyGrowth,
				EvaluationProtocol.Stats.HealthGrowth,
				EvaluationProtocol.Stats.StrengthGrowth
			});
		else if (type.Equals (Type.Bullseye))
			return new List<EvaluationProtocol.Stats> (new EvaluationProtocol.Stats[] {
				EvaluationProtocol.Stats.AccuracyGrowth,
				EvaluationProtocol.Stats.SpeedGrowth,
				EvaluationProtocol.Stats.StrengthGrowth
			});
		else if (type.Equals (Type.GentleGiant))
			return new List<EvaluationProtocol.Stats> (new EvaluationProtocol.Stats[] {
				EvaluationProtocol.Stats.EnduranceGrowth,
				EvaluationProtocol.Stats.HealthGrowth,
				EvaluationProtocol.Stats.SpeedGrowth
			});
		else if (type.Equals (Type.Slugger))
			return new List<EvaluationProtocol.Stats> (new EvaluationProtocol.Stats[] {
				EvaluationProtocol.Stats.EnduranceGrowth,
				EvaluationProtocol.Stats.HealthGrowth,
				EvaluationProtocol.Stats.StrengthGrowth
			});
		else if (type.Equals (Type.Juggernaut))
			return new List<EvaluationProtocol.Stats> (new EvaluationProtocol.Stats[] {
				EvaluationProtocol.Stats.EnduranceGrowth,
				EvaluationProtocol.Stats.SpeedGrowth,
				EvaluationProtocol.Stats.StrengthGrowth
			});
		else if (type.Equals (Type.DoubleEdge))
			return new List<EvaluationProtocol.Stats> (new EvaluationProtocol.Stats[] {
				EvaluationProtocol.Stats.HealthGrowth,
				EvaluationProtocol.Stats.SpeedGrowth,
				EvaluationProtocol.Stats.StrengthGrowth
			});

		return new List<EvaluationProtocol.Stats> (new EvaluationProtocol.Stats[] {
			EvaluationProtocol.Stats.HealthGrowth,
			EvaluationProtocol.Stats.SpeedGrowth,
			EvaluationProtocol.Stats.StrengthGrowth
		});
	}

	public static List<Type> getClassesBasedOnWeight(WeightClass.WClass wClass)
	{
		if (wClass.Equals (WeightClass.WClass.FlyWeight))
			return new List<Type> (new Type[] {
				Type.Ranger, Type.Monk, Type.Bullseye, Type.DoubleEdge
			});
		else if (wClass.Equals (WeightClass.WClass.LightWeight))
			return new List<Type> (new Type[] {
				Type.Ranger, Type.Monk, Type.Bullseye, Type.DoubleEdge
			});
		else if (wClass.Equals (WeightClass.WClass.WelterWeight))
			return new List<Type> (new Type[] {
				Type.Ranger, Type.Monk, Type.Gladiator, Type.BushWacker, Type.DoubleEdge
			});
		else if (wClass.Equals (WeightClass.WClass.MiddleWeight))
			return new List<Type> (new Type[] {
				Type.Legion, Type.Gladiator, Type.BushWacker, Type.Juggernaut
			});
		else if (wClass.Equals (WeightClass.WClass.CruiserWeight))
			return new List<Type> (new Type[] {
				Type.Legion, Type.GentleGiant, Type.Slugger, Type.Juggernaut
			});
		else if (wClass.Equals (WeightClass.WClass.HeavyWeight))
			return new List<Type> (new Type[] {
				Type.Legion, Type.GentleGiant, Type.Slugger, Type.Juggernaut
			});

		return new List<Type> (new Type[] {
			Type.Ranger, Type.Monk, Type.Bullseye, Type.DoubleEdge
		});
	}

	public static List<Type> getTypeList(){
		return new List<Type> (new Type[] {
			Type.Legion, Type.Ranger, Type.Gladiator, Type.Monk, Type.BushWacker, Type.Bullseye, Type.GentleGiant, 
			Type.Slugger, Type.Juggernaut, Type.DoubleEdge
		});
	}

	public static bool isBoxerEligibleForType(WeightClass.WClass wClass, Type type)
	{
		if (type.Equals (Type.Legion))
			return (wClass.Equals (WeightClass.WClass.CruiserWeight) || wClass.Equals (WeightClass.WClass.HeavyWeight) ||
			wClass.Equals (WeightClass.WClass.MiddleWeight)) ? true : false;
		else if (type.Equals (Type.Ranger))
			return (wClass.Equals (WeightClass.WClass.FlyWeight) || wClass.Equals (WeightClass.WClass.LightWeight) ||
			wClass.Equals (WeightClass.WClass.WelterWeight)) ? true : false;
		else if (type.Equals (Type.Gladiator))
			return (wClass.Equals (WeightClass.WClass.WelterWeight) || wClass.Equals (WeightClass.WClass.MiddleWeight)) ? true : false;
		else if (type.Equals (Type.Monk))
			return (wClass.Equals (WeightClass.WClass.FlyWeight) || wClass.Equals (WeightClass.WClass.LightWeight) ||
				wClass.Equals (WeightClass.WClass.WelterWeight)) ? true : false;
		else if (type.Equals (Type.BushWacker))
			return (wClass.Equals (WeightClass.WClass.WelterWeight) || wClass.Equals (WeightClass.WClass.MiddleWeight)) ? true : false;
		else if (type.Equals (Type.Bullseye))
			return (wClass.Equals (WeightClass.WClass.FlyWeight) || wClass.Equals (WeightClass.WClass.LightWeight)) ? true : false;
		else if (type.Equals (Type.GentleGiant))
			return (wClass.Equals (WeightClass.WClass.CruiserWeight) || wClass.Equals (WeightClass.WClass.HeavyWeight)) ? true : false;
		else if (type.Equals (Type.Slugger))
			return (wClass.Equals (WeightClass.WClass.CruiserWeight) || wClass.Equals (WeightClass.WClass.HeavyWeight)) ? true : false;
		else if (type.Equals (Type.Juggernaut))
			return (wClass.Equals (WeightClass.WClass.CruiserWeight) || wClass.Equals (WeightClass.WClass.HeavyWeight) ||
				wClass.Equals (WeightClass.WClass.MiddleWeight)) ? true : false;
		else if (type.Equals (Type.DoubleEdge))
			return (wClass.Equals (WeightClass.WClass.FlyWeight) || wClass.Equals (WeightClass.WClass.LightWeight) ||
				wClass.Equals (WeightClass.WClass.WelterWeight)) ? true : false;

		return (wClass.Equals (WeightClass.WClass.FlyWeight) || wClass.Equals (WeightClass.WClass.LightWeight) ||
			wClass.Equals (WeightClass.WClass.WelterWeight)) ? true : false;
	}


}


//Acc, End, Hlt : Legion - HeavyWieght, CruiserWeight
//Acc, End, Spd : Ranger - FlyWeight, LightWeight, WelterWeight
//Acc, End, Str : Gladiator - 
//Acc, Hlt, Spd : Matador - 
//Acc, Hlt, Str : BushWacker - 
//Acc, Spd, Str : Bullseye- FlyWeight, LightWeight, WelterWeight
//End, Hlt, Spd : Gentle Giant - HeavyWeight, CruiserWeight
//End, Hlt, Str : Slugger - HeavyWeight, CruiserWeight
//End, Spd, Str : Juggernaut - HeavyWeight, CruiserWeight,  MiddleWeight, WelterWeight
//Hlt, Spd, Str : Double Edge - LightWeight, WelterWeight, MiddleWeight