using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class ScrollRectPosition : MonoBehaviour
{

    RectTransform scrollRectTransform;
    RectTransform contentPanel;
    RectTransform selectedRectTransform;
    GameObject lastSelected;

    void Start()
    {
        scrollRectTransform = GetComponent<RectTransform>();
        contentPanel = GetComponent<ScrollRect>().content;
    }

    void Update()
    {
        GameObject selected = EventSystem.current.currentSelectedGameObject;
        
        if (selected == null)
        {
            return;
        }
        if (selected.transform.parent != contentPanel.transform)
        {
            return;
        }
        if (selected == lastSelected)
        {
            return;
        }
        
        selectedRectTransform = selected.GetComponent<RectTransform>();
        // The position of the selected UI element is the absolute anchor position,
        // ie. the local position within the scroll rect + its height if we're
        // scrolling down. If we're scrolling up it's just the absolute anchor position.
        float selectedPositionY = Mathf.Abs(selectedRectTransform.anchoredPosition.y) + selectedRectTransform.rect.height;

        // The upper bound of the scroll view is the anchor position of the content we're scrolling.
        float scrollViewMinY = contentPanel.anchoredPosition.y;
        // The lower bound is the anchor position + the height of the scroll rect.
        float scrollViewMaxY = contentPanel.anchoredPosition.y + scrollRectTransform.rect.height;

        // If the selected position is below the current lower bound of the scroll view we scroll down.
        if (selectedPositionY > scrollViewMaxY)
        {
            float newY = selectedPositionY - scrollRectTransform.rect.height;
            contentPanel.anchoredPosition = new Vector2(contentPanel.anchoredPosition.x, newY);
        }
        // If the selected position is above the current upper bound of the scroll view we scroll up.
        else if (Mathf.Abs(selectedRectTransform.anchoredPosition.y) < scrollViewMinY)
        {
            contentPanel.anchoredPosition = new Vector2(contentPanel.anchoredPosition.x, Mathf.Abs(selectedRectTransform.anchoredPosition.y));
        }

        lastSelected = selected;
    }
}