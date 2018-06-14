using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ListController : MonoBehaviour {

    public enum RowColor
	{
		Custom, Blue, Red, Green, Orange, Yellow, White
	}

	private Transform content;
	private Dictionary<RowColor, Transform> rowOptions;

	private Color customColor = Color.blue;
	private RowColor rowColour;

	private List<RowInfoInitializer> currentList;
    
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
		tile.GetComponent<RowInfo>().type = info.Type;
        tile.GetComponent<RowInfo>().id = info.ID;
        tile.GetComponent<RowInfo>().position = info.Position;
        tile.GetComponent<RowInfo>().text = info.Text;
        tile.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = info.Text;
        tile.SetParent(content);
		currentList.Add(info);
	}

    public void addRows(List<RowInfoInitializer> rowInfos)
	{
		currentList.Clear();
		foreach (RowInfoInitializer info in rowInfos)
		{
			Transform tile = Instantiate(rowOptions[rowColour], new Vector3(0.0f, 0.0f), Quaternion.identity) as Transform;
            tile.GetComponent<RowInfo>().type = info.Type;
            tile.GetComponent<RowInfo>().id = info.ID;
			tile.GetComponent<RowInfo>().position = info.Position;
			tile.GetComponent<RowInfo>().text = info.Text;
			tile.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = info.Text;
            tile.SetParent(content);
			currentList.Add(info);
		}
	}

    public void focusOnList()
	{
		content.GetChild(0).GetComponent<Button>().Select();
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

	// Update is called once per frame
	void Update () {
		
	}
}
