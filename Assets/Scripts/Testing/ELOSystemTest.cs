using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ELOSystemTest : MonoBehaviour {
	private DataPool worldData;

	private List<ManagerProtocol> managers;

	void Start () {
		worldData = new DataPool ();

		WorldBuilderProtocol.createWorld (ref worldData, 220, 220);

		managers = new List<ManagerProtocol> ();

		List<BoxerClass.Type> typeList = BoxerClass.getTypeList ();

		for (int m = 0; m < 200; m++) {
			Manager manager = new Manager ("Capn", "AT", 0, 155.0f, typeList[Random.Range(0, typeList.Count)]);
			manager.Record.setELO (Random.Range (50, 2200));
			worldData.Managers.Add (manager);

			ManagerProtocol mp = new ManagerProtocol (ref worldData, worldData.Managers.Count - 1, true);

			managers.Add (mp);
	
            List<Boxer> boxers = WorldBuilderProtocol.generateBoxerRecruits (ref worldData, manager.TownIndex, manager.Record.ELO);

			int bIndex = 0;
			float max = 0.0f;

			for (int i = 0; i < boxers.Count; i++) {
				float boxerEval = EvaluationProtocol.evaluateBoxer (boxers [i], worldData.Managers [mp.ManagerIndex].Preference);

				if (boxerEval > max) {
					max = boxerEval;
					bIndex = i;
				}
			}

			worldData.Boxers.Add (boxers [bIndex]);
			mp.recruitBoxer (worldData.Boxers.Count - 1);
			mp.updateELO (ref worldData);
			mp.upgradeFacilities (ref worldData);
		}

		simMultipleMatches ();
	}

	private List<int> findOpponents(ManagerProtocol mp, int band){
		List<int> possibleOpponents = new List<int> ();
		//for (int i = 0; i < managers.Count; i++) {
		//	if (mp.ManagerIndex != managers [i].ManagerIndex && managers[i].UpcomingMatch.Opponent < 0) {
		//		if (Mathf.Abs(mp.CurrentBoxerELO - managers [i].CurrentBoxerELO) < band && !mp.fightRecently(i)) {
		//			possibleOpponents.Add (i);
		//		}
		//	}
		//}

		return possibleOpponents;
	}

	private void scheduleMatches(){
		managers = managers.OrderByDescending(mg => mg.CurrentBoxerELO).ToList();
		//for (int i = 0; i < managers.Count; i++) {
		//	if (managers[i].UpcomingMatch.Opponent < 0) {
		//		List<int> possibleOpponents = findOpponents (managers[i], 200);

		//		if (possibleOpponents.Count > 0) {
		//			int opponent = possibleOpponents [Random.Range (0, possibleOpponents.Count)];
		//			managers [i].scheduleMatch (ref worldData, opponent);
		//			managers [opponent].scheduleMatch (ref worldData, i);
		//		} else {
		//			possibleOpponents = findOpponents (managers[i], 420);

		//			if (possibleOpponents.Count > 0) {
		//				int opponent = possibleOpponents [Random.Range (0, possibleOpponents.Count)];
		//				managers [i].scheduleMatch (ref worldData, opponent);
		//				managers [opponent].scheduleMatch (ref worldData, i);
		//			} 
		//		}
		//	}
		//}
	}

	private void simMatches(){
		foreach (ManagerProtocol mp in managers) {
			//if (mp.UpcomingMatch.Opponent >= 0) {
			//	float b1 = mp.CurrentBoxerELO;
			//	float b2 = managers [mp.UpcomingMatch.Opponent].CurrentBoxerELO;

			//	float m1 = mp.CurrentManagerELO;
			//	float m2 = managers [mp.UpcomingMatch.Opponent].CurrentManagerELO;

			//	float avg1 = worldData.Boxers [mp.BoxerIndex].getOverall ();
			//	float avg2 = worldData.Boxers [managers [mp.UpcomingMatch.Opponent].BoxerIndex].getOverall ();

			//	float e1Chances = Mathf.RoundToInt((avg1 / (avg1 + avg2)) * 100.0f);

			//	if (Random.Range (0, 100) < e1Chances) {
			//		worldData.Managers [mp.ManagerIndex].Record.addWin (m2);
			//		worldData.Managers [managers [mp.UpcomingMatch.Opponent].ManagerIndex].Record.addLoss (m1);
			//		worldData.Boxers [mp.BoxerIndex].Record.addWin (b2);
			//		worldData.Boxers [managers [mp.UpcomingMatch.Opponent].BoxerIndex].Record.addLoss (b1);
			//	} else {
			//		worldData.Managers [mp.ManagerIndex].Record.addLoss (m2);
			//		worldData.Managers [managers [mp.UpcomingMatch.Opponent].ManagerIndex].Record.addWin (m1);
			//		worldData.Boxers [mp.BoxerIndex].Record.addLoss (b2);
			//		worldData.Boxers [managers [mp.UpcomingMatch.Opponent].BoxerIndex].Record.addWin (b1);
			//	}

			//	mp.updateELO (ref worldData);
			//	managers [mp.UpcomingMatch.Opponent].updateELO (ref worldData);
			//	managers [mp.UpcomingMatch.Opponent].scheduleMatch (ref worldData, -1);
			//	mp.scheduleMatch (ref worldData, -1);
			//}
		}
	}

	public void simMultipleMatches(){

		for (int i = 0; i < 6; i++) {
			Debug.Log (worldData.Calendar.getDate (Calendar.DateType.fullLong));

			for (int k = 0; k < 3; k++) {
				scheduleMatches ();

				for (int j = 0; j < 16; j++) {
					foreach (ManagerProtocol mp in managers) {
						mp.executeWeek (ref worldData);
					}
					worldData.Calendar.progessWeek ();
//					Debug.Log (worldData.Calendar.getDate (Calendar.DateType.fullLong));
					//Debug.Log(worldData.Calendar.weeksAway(managers[0].UpcomingMatch.Date));
				}

				simMatches ();
			}
		}

		managers = managers.OrderByDescending (m => m.CurrentBoxerELO).ToList ();
		foreach (ManagerProtocol mp in managers) {
			Debug.Log (mp.getBoxerELOHistory () + " -> " + mp.CurrentBoxerELO + 
				" - W " + worldData.Boxers[mp.BoxerIndex].Record.Wins + 
				" - L " + worldData.Boxers[mp.BoxerIndex].Record.Losses + 
				" -- " + mp.getManagerELOHistory() + " -> " + mp.CurrentManagerELO
			);
			worldData.Boxers [mp.BoxerIndex].logBoxerStats ();
		}
	}
}
