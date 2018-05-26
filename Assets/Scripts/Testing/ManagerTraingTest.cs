using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerTraingTest : MonoBehaviour {

	private DataPool worldData;

	void Start () {
		worldData = new DataPool ();

		WorldBuilderProtocol.createWorld (ref worldData, 220, 220);

		Manager manager = new Manager ("Capn", "AT", 0, 155.0f, BoxerClass.Type.Bullseye);
		worldData.Managers.Add (manager);

        List<Boxer> boxers = WorldBuilderProtocol.generateBoxerRecruits (ref worldData, manager.TownIndex, 0);

		int bIndex = 0;
		float max = 0.0f;

		for (int i = 0; i < boxers.Count; i++) {
			float boxerEval = EvaluationProtocol.evaluateBoxer (boxers [i], worldData.Managers [0].Preference);

			Debug.Log (boxerEval);
			boxers [i].logBoxerStats ();

			if (boxerEval > max) {
				max = boxerEval;
				bIndex = i;
			}
		}

		worldData.Boxers.Add (boxers [bIndex]);
		manager.recruitBoxer(worldData.Boxers.Count - 1);

		for (int i = 0; i < 5; i++) {
			Debug.Log ("Year " + (i + 1));
			for (int j = 0; j < 48; j++) {
				ManagerProtocol.executeWeek(ref worldData, 0);
				worldData.Boxers [manager.BoxerIndex].logBoxerStats ();
			}
		}
	}

}
