using UnityEngine;
using System.Collections;

public class RowInfoInitializer
{

    private RowInfo.Type type;
    private int id;
    private Vector2 position;
    private string text;

	public RowInfoInitializer(RowInfo.Type type, int id, Vector2 position, string text)
    {
        this.type = type;
        this.id = id;
        this.position = position;
        this.text = text;
    }

    //Getters
    public RowInfo.Type Type
	{
		get { return type; }
	}

    public int ID
	{
		get { return ID; }
	}

	public Vector2 Position
	{
		get { return position; }
	}

    public string Text
	{
		get { return text; }
	}   
}
