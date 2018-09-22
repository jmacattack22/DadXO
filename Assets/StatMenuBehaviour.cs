using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatMenuBehaviour : MonoBehaviour {

    public enum Stats
	{
		Accuracy, Endurance, Health, Speed, Strength
	}

	private Dictionary<Stats, Transform> progressBars;
	private Dictionary<Stats, TMPro.TextMeshProUGUI> progress;
	private Dictionary<Stats, TMPro.TextMeshProUGUI> levels;

	void Start () {

		progressBars = new Dictionary<Stats, Transform>();
		progress = new Dictionary<Stats, TMPro.TextMeshProUGUI>();
		levels = new Dictionary<Stats, TMPro.TextMeshProUGUI>();

		loadUI();
	}

    private void loadUI()
	{
		Transform acc = transform.GetChild(0).GetChild(3).transform;
		Transform end = transform.GetChild(0).GetChild(4).transform;
		Transform lif = transform.GetChild(0).GetChild(5).transform;
		Transform spd = transform.GetChild(0).GetChild(6).transform;
		Transform str = transform.GetChild(0).GetChild(7).transform;
		
		progressBars.Add(Stats.Accuracy, acc.GetChild(2).GetChild(0).GetComponent<Transform>());
		progressBars.Add(Stats.Endurance, end.GetChild(2).GetChild(0).GetComponent<Transform>());
		progressBars.Add(Stats.Health, lif.GetChild(2).GetChild(0).GetComponent<Transform>());
		progressBars.Add(Stats.Speed, spd.GetChild(2).GetChild(0).GetComponent<Transform>());
		progressBars.Add(Stats.Strength, str.GetChild(2).GetChild(0).GetComponent<Transform>());

		levels.Add(Stats.Accuracy, acc.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>());
		levels.Add(Stats.Endurance, end.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>());
		levels.Add(Stats.Health, lif.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>());
		levels.Add(Stats.Speed, spd.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>());
		levels.Add(Stats.Strength, str.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>());
        
		progress.Add(Stats.Endurance, acc.GetChild(2).GetChild(1).GetComponent<TMPro.TextMeshProUGUI>());
		progress.Add(Stats.Endurance, acc.GetChild(2).GetChild(1).GetComponent<TMPro.TextMeshProUGUI>());
		progress.Add(Stats.Health, acc.GetChild(2).GetChild(1).GetComponent<TMPro.TextMeshProUGUI>());
		progress.Add(Stats.Speed, acc.GetChild(2).GetChild(1).GetComponent<TMPro.TextMeshProUGUI>());
		progress.Add(Stats.Strength, acc.GetChild(2).GetChild(1).GetComponent<TMPro.TextMeshProUGUI>());
	}

    public void updateStats(Boxer boxer)
	{
		progressBars[Stats.Accuracy].GetComponent<RectTransform>().sizeDelta =
			new Vector2((float)boxer.Accuracy / 999.0f, 30.0f);
		progressBars[Stats.Endurance].GetComponent<RectTransform>().sizeDelta =
            new Vector2((float)boxer.Endurance / 999.0f, 30.0f);
		progressBars[Stats.Health].GetComponent<RectTransform>().sizeDelta =
            new Vector2((float)boxer.Health / 999.0f, 30.0f);
		progressBars[Stats.Speed].GetComponent<RectTransform>().sizeDelta =
            new Vector2((float)boxer.Speed / 999.0f, 30.0f);
		progressBars[Stats.Strength].GetComponent<RectTransform>().sizeDelta =
            new Vector2((float)boxer.Strength / 999.0f, 30.0f);

		progress[Stats.Accuracy].text = boxer.Accuracy.ToString();
		progress[Stats.Endurance].text = boxer.Endurance.ToString();
		progress[Stats.Health].text = boxer.Health.ToString();
		progress[Stats.Speed].text = boxer.Speed.ToString();
		progress[Stats.Strength].text = boxer.Strength.ToString();
	}
}
