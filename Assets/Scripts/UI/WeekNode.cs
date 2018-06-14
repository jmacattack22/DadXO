using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeekNode : MonoBehaviour {

    public enum WeekNodeLabel
	{
		Rank, QualifierCount, TournamentCount
	}

	private Dictionary<WeekNodeLabel, TMPro.TextMeshProUGUI> uiElements;

	void Start () {
		uiElements = new Dictionary<WeekNodeLabel, TMPro.TextMeshProUGUI>();
		loadUI();
	}
    
    public void setupNode(TournamentProtocol.Level level, int qCount, int tCount)
	{
		uiElements[WeekNodeLabel.Rank].SetText(level.ToString());
		uiElements[WeekNodeLabel.QualifierCount].SetText(qCount.ToString());
		uiElements[WeekNodeLabel.TournamentCount].SetText(tCount.ToString());
	}

    private void loadUI()
	{
		uiElements.Add(WeekNodeLabel.Rank, transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>());
		uiElements.Add(WeekNodeLabel.QualifierCount, transform.GetChild(2).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>());
		uiElements.Add(WeekNodeLabel.TournamentCount, transform.GetChild(3).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>());
	}
}
