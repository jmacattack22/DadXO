using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHandlerBehaviour : MonoBehaviour {

    public enum Type
	{
		Map, Inventory, Stats, Main
	}

	Dictionary<Type, List<Transform>> uiHolders;

	Dictionary<Type, List<Transform>> fadeElements;

	List<Type> typeList;
    
	void Start()
	{
		typeList = new List<Type>(new Type[]{ Type.Map, Type.Inventory, Type.Stats, Type.Main});

		uiHolders = new Dictionary<Type, List<Transform>>();
		fadeElements = new Dictionary<Type, List<Transform>>();
		loadUI();
	}

	private void loadUI()
	{
		uiHolders.Add(Type.Map, new List<Transform>());
		fadeElements.Add(Type.Map, new List<Transform>());
		fadeElements[Type.Map].Add(GameObject.FindWithTag("MapDrawers").transform.GetChild(0).transform);
		Transform mapUI = GameObject.FindWithTag("MapUI").transform;
        uiHolders[Type.Map].Add(mapUI.GetChild(0).transform);
		uiHolders[Type.Map].Add(mapUI.GetChild(1).transform);
		uiHolders[Type.Map].Add(mapUI.GetChild(2).transform);
		uiHolders[Type.Map].Add(mapUI.GetChild(3).transform);
		uiHolders[Type.Map].Add(mapUI.GetChild(4).transform);
		uiHolders[Type.Map].Add(mapUI.GetChild(5).transform);

	}

	public void showUI(Type type)
	{
		List<Type> otherTypes = getOtherTypes(type);

		hideUI(otherTypes);

        if (uiHolders.ContainsKey(type))
		{
			toggleUI(uiHolders[type], true); 
		}

        if (fadeElements.ContainsKey(type))
		{
			toggleFade(fadeElements[type], true);
		}
	}

	private void toggleUI(List<Transform> transforms, bool desiredState)
	{
		foreach (Transform t in transforms)
        {
			if (t.GetComponent<EasyTween>().IsObjectOpened() != desiredState)
			{
				t.GetComponent<EasyTween>().OpenCloseObjectAnimation();
			}
        } 
	}

    public void hideAllUI()
	{
		foreach (Type type in typeList)
		{
			if (uiHolders.ContainsKey(type))
			{
				toggleUI(uiHolders[type], false);
			}

            if (fadeElements.ContainsKey(type))
			{
				toggleFade(fadeElements[type], false);
			}
		}
	}

	private void hideUI(List<Type> otherTypes)
	{
		foreach (Type type in otherTypes)
		{
			if (uiHolders.ContainsKey(type))
			{
				toggleUI(uiHolders[type], false);          
			}

			if (fadeElements.ContainsKey(type))
            {
                toggleFade(fadeElements[type], false);
            }
		}
	}

	private List<Type> getOtherTypes(Type type)
	{
		List<Type> otherTypes = new List<Type>();

        foreach (Type t in typeList)
		{
			if (!t.Equals(type))
			{
				otherTypes.Add(t);
			}
		}

		return otherTypes;
	}

	private void toggleFade(List<Transform> transforms, bool fadeOut)
    {
        foreach (Transform t in transforms)
        {
            if (t.GetComponent<SpriteRenderer>().color.a <= (1.0f / 255.0f) && !fadeOut)
            {
                //t.GetComponent<FadeOut>().fadeIn();
            }
            else if (t.GetComponent<SpriteRenderer>().color.a >= (253.0f / 255.0f) && fadeOut)
            {
                //t.GetComponent<FadeOut>().fadeOut();
            }
        }
    }
}
