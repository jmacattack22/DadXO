using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHandlerBehaviour : MonoBehaviour {

    public enum Type
	{
		Map, Equipment, Stats, Inventory, Options
	}

	private Dictionary<Type, Transform> uiHolders;
	private Dictionary<Type, Transform> tabHolders;
	private Transform modal;
	private Transform tabMenu;

	private List<Type> typeList;
   
	private Type currentState = Type.Map;
    
	void Awake()
	{
		typeList = new List<Type>(new Type[]{ Type.Map, Type.Equipment, Type.Stats, Type.Inventory, Type.Options});

		uiHolders = new Dictionary<Type, Transform>();
		tabHolders = new Dictionary<Type, Transform>();
		loadUI();
	}

	public Type getNextType()
	{
		return typeList[(typeList.IndexOf(currentState) + 1) % typeList.Count];
	}

	public Type getPreviousType()
	{
		int index = typeList.IndexOf(currentState) - 1;
		return typeList[(index < 0 ? typeList.Count - 1 : index)];
	}

	private void loadUI()
	{
		uiHolders.Add(Type.Map, GameObject.FindWithTag("MapUI").transform);
		uiHolders.Add(Type.Equipment, GameObject.FindWithTag("EquipmentUI").transform);
		uiHolders.Add(Type.Stats, GameObject.FindWithTag("StatsUI").transform);
		uiHolders.Add(Type.Inventory, GameObject.FindWithTag("InventoryUI").transform);
		uiHolders.Add(Type.Options, GameObject.FindWithTag("OptionsUI").transform);

		modal = GameObject.FindWithTag("ModalBackground").transform;
		tabMenu = GameObject.FindWithTag("TabMenu").transform;

		tabHolders.Add(Type.Map, tabMenu.GetChild(0));
		tabHolders.Add(Type.Equipment, tabMenu.GetChild(1));
		tabHolders.Add(Type.Stats, tabMenu.GetChild(2));
		tabHolders.Add(Type.Inventory, tabMenu.GetChild(3));
		tabHolders.Add(Type.Options, tabMenu.GetChild(4));
	}

	public void showUI(Type type)
	{
		List<Type> otherTypes = getOtherTypes(type);

		hideUI(otherTypes);
		unFocusTabs(otherTypes);
        
        if (uiHolders.ContainsKey(type))
		{
			toggleUI(uiHolders[type], true);
			focusOnTab(tabHolders[type]);
		}

		toggleModal(true);
		currentState = type;
	}

	private void unFocusTabs(List<Type> otherTypes)
	{
		foreach (Type t in otherTypes)
		{
			tabHolders[t].GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f);
		}
	}

	private void focusOnTab(Transform transform)
	{
		transform.GetComponent<Image>().color = new Color(255.0f / 255.0f, 152.0f / 255.0f, 90.0f / 255.0f);
	}

	private void toggleModal(bool desiredState)
	{
		modal.GetComponent<Image>().enabled = desiredState;
        if (desiredState)
		{
			if (!tabMenu.GetComponent<EasyTween>().isActiveAndEnabled)
			{
				tabMenu.GetComponent<EasyTween>().OpenCloseObjectAnimation();
			}
		}
		else
		{
			if (tabMenu.GetComponent<EasyTween>().isActiveAndEnabled)
            {
				tabMenu.GetComponent<EasyTween>().OpenCloseObjectAnimation();
            }
		}
	}

	private void toggleUI(Transform transform, bool desiredState)
	{
		if (transform.GetComponent<EasyTween>().IsObjectOpened() != desiredState)
        {
            transform.GetComponent<EasyTween>().OpenCloseObjectAnimation();
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
		}

		toggleModal(false);
	}

	private void hideUI(List<Type> otherTypes)
	{
		foreach (Type type in otherTypes)
		{
			if (uiHolders.ContainsKey(type))
			{
				toggleUI(uiHolders[type], false);          
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

    public bool isDisplaying()
	{
		return modal.GetComponent<Image>().enabled;
	}

    //Getters
    public Type CurrentMenu
	{
		get { return currentState; }
	}
}
