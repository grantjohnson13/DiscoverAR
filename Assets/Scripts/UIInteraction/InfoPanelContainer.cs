using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InfoPanelContainer : MonoBehaviour, IPointerClickHandler
{

    public Button GoogleBackButton;
    public void OnPointerClick(PointerEventData eventData)
    {
        // Check if the click/touch is on the panel and not on a child UI element
        if (eventData.pointerCurrentRaycast.gameObject == gameObject)
        {
            // Deactivate the panel
            // gameObject.SetActive(false);
            GoogleBackButton.onClick.Invoke();
        }
    }
}
