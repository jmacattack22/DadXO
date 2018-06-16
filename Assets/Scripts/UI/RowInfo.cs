using UnityEngine;
using System.Collections;

public class RowInfo : MonoBehaviour
{
	public enum Type
	{
		Boxer, Manager, Region, Town, Consumable, Arm, Legs, Implant
	}

	[HideInInspector]
	public Type type;
	[HideInInspector]
	public int id;
	[HideInInspector]
	public Vector2 position;
	[HideInInspector]
	public string text;

	public RowInfo(Type type, int id, Vector2 position, string text)
	{
		this.type = type;
		this.id = id;
		this.position = position;
		this.text = text;
	}
}
