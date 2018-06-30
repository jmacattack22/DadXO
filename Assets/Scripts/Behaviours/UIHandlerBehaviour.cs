using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHandlerBehaviour : MonoBehaviour {

    public enum Type
	{
		Map, Inventory, Stats, Main
	}

	Dictionary<Type, Transform> uiHolders;
	Transform modal;
	Transform tabMenu;

	List<Type> typeList;
    
	void Awake()
	{
		typeList = new List<Type>(new Type[]{ Type.Map, Type.Inventory, Type.Stats, Type.Main});

		uiHolders = new Dictionary<Type, Transform>();
		loadUI();
	}

	private void loadUI()
	{
		uiHolders.Add(Type.Map, GameObject.FindWithTag("MapUI").transform);

		modal = GameObject.FindWithTag("ModalBackground").transform;
		tabMenu = GameObject.FindWithTag("TabMenu").transform;
		tabMenu.GetComponent<EasyTween>().OpenCloseObjectAnimation();
	}

	public void showUI(Type type)
	{
		List<Type> otherTypes = getOtherTypes(type);

		hideUI(otherTypes);
        
        if (uiHolders.ContainsKey(type))
		{
			toggleUI(uiHolders[type], true); 
		}

		toggleModal(true);
	}

	private void toggleModal(bool desiredState)
	{
		modal.GetComponent<Image>().enabled = desiredState;
        if (desiredState)
		{
			if (!tabMenu.GetComponent<EasyTween>().isActiveAndEnabled)
			{
				modal.GetComponent<EasyTween>().OpenCloseObjectAnimation();
			}
		}
		else
		{
			if (!tabMenu.GetComponent<EasyTween>().isActiveAndEnabled)
            {
                modal.GetComponent<EasyTween>().OpenCloseObjectAnimation();
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
}
