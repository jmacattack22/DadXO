using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ListController : MonoBehaviour {

    public enum RowColor
	{
		Custom, Blue, Red, Green, Orange, Yellow, White
	}

    public enum ListState
	{
		None, Focused, Clicked
	}

	public Button unfocusButton;

	private Transform content;
	private Dictionary<RowColor, Transform> rowOptions;

	private Color customColor = Color.blue;
	private RowColor rowColour;

	private List<RowInfoInitializer> currentList;

	private ListState state = ListState.None;
    
	void Start () {
		loadUI();

		rowOptions = new Dictionary<RowColor, Transform>();
		loadRowOptions();

		rowColour = RowColor.Blue;
		currentList = new List<RowInfoInitializer>();
	}

    public void addRow(RowInfoInitializer info)
	{
		Transform tile = Instantiate(rowOptions[rowColour], new Vector3(0.0f, 0.0f), Quaternion.identity) as Transform;
		tile.GetComponent<Button>().onClick.AddListener(rowClick);
		tile.GetComponent<RowInfo>().type = info.Type;
        tile.GetComponent<RowInfo>().id = info.ID;
        tile.GetComponent<RowInfo>().position = info.Position;
        tile.GetComponent<RowInfo>().text = info.Text;
        tile.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = info.Text;
        tile.SetParent(content);
		currentList.Add(info);

		tile.localScale = new Vector3(1.0f, 1.0f, 1.0f);
	}

    public void addRows(List<RowInfoInitializer> rowInfos)
	{
		cleanupList();
		currentList.Clear();

		rowInfos = rowInfos.OrderBy(x => x.Text).ToList();

		foreach (RowInfoInitializer info in rowInfos)
		{
			Transform tile = Instantiate(rowOptions[rowColour], new Vector3(0.0f, 0.0f), Quaternion.identity) as Transform;
			tile.GetComponent<Button>().onClick.AddListener(rowClick);
            tile.GetComponent<RowInfo>().type = info.Type;
            tile.GetComponent<RowInfo>().id = info.ID;
			tile.GetComponent<RowInfo>().position = info.Position;
			tile.GetComponent<RowInfo>().text = info.Text;
			tile.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = info.Text;
            tile.SetParent(content);
			currentList.Add(info);

			tile.localScale = new Vector3(1.0f, 1.0f, 1.0f);
		}
	}

    public void acknowledgeClick()
	{
		state = ListState.Focused;
	}

    public void cleanupList()
	{
		var children = new List<GameObject>();
        foreach (Transform child in content) children.Add(child.gameObject);
        children.ForEach(child => Destroy(child));
	}

    public void rowClick()
	{
		state = ListState.Clicked;
	}

    public void resolveSelection()
	{
		if (state.Equals(ListState.Focused))
		{
			RowInfoInitializer infoInitializer = getSelectedRow();
			if (infoInitializer.ID == -1)
			{
				focusOnList();
			}
		}
	}

    public void focusOnList()
	{
		foreach (Transform btn in content)
        {
            btn.GetComponent<Button>().interactable = true;
        }

		content.GetChild(0).GetComponent<Button>().Select();         
		state = ListState.Focused;
	}

    public void unfocusList()
	{
		unfocusButton.Select();

        foreach (Transform btn in content)
		{
			btn.GetComponent<Button>().interactable = false;
		}

		state = ListState.None;
	}

    public RowInfoInitializer getSelectedRow()
	{
		for (int i = 0; i < content.childCount; i++)
		{
			if (content.GetChild(i).gameObject == EventSystem.current.currentSelectedGameObject)
			{
				return new RowInfoInitializer(
					content.GetChild(i).GetComponent<RowInfo>().type,
					content.GetChild(i).GetComponent<RowInfo>().id,
					content.GetChild(i).GetComponent<RowInfo>().position,
					content.GetChild(i).GetComponent<RowInfo>().text
				);
			}
		}

		return new RowInfoInitializer(RowInfo.Type.Region, -1, new Vector2(0, 0), "");
	}

	private void loadRowOptions()
	{
		rowOptions.Add(RowColor.Blue, Resources.Load<Transform>("Prefabs/RowLabels/BlueRow"));
		rowOptions.Add(RowColor.Red, Resources.Load<Transform>("Prefabs/RowLabels/RedRow"));
		rowOptions.Add(RowColor.Green, Resources.Load<Transform>("Prefabs/RowLabels/GreenRow"));
		rowOptions.Add(RowColor.Orange, Resources.Load<Transform>("Prefabs/RowLabels/OrangeRow"));
		rowOptions.Add(RowColor.Yellow, Resources.Load<Transform>("Prefabs/RowLabels/YellowRow"));
		rowOptions.Add(RowColor.White, Resources.Load<Transform>("Prefabs/RowLabels/WhiteRow"));
		rowOptions.Add(RowColor.Custom, Resources.Load<Transform>("Prefabs/RowLabels/WhiteRow"));
	}

	private void loadUI()
	{
		content = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Transform>();      
	}

	public void setColor(RowColor color)
	{
		rowColour = color;
	}

    //Getters
    public ListState State
	{
		get { return state; }
	}
}
