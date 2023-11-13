using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Assuming you're using the standard UI
using System;

public class PanelUpdater : MonoBehaviour
{
    public GameObject PopUpToRemove;
    public GameObject textPrefab;
    public GameObject textContainer;

    public GameObject PopUp;
    public Text PopUpNameText; 
    public Text PopUpRatingText; 
    public Text PopUpIsOpenText; 
    public Text PopUpPhoneText; 
    public Text PopUpWebsiteText; 
    public Text PopUpDirectionsText; 

    public void CreateTextAsChild(string textContent, string place_id)
    {
        if (textPrefab == null)
        {
            Debug.LogError("textPrefab is not assigned in the inspector.");
            return;
        }

        if (textContainer == null)
        {
            Debug.LogError("textContainer is not assigned in the inspector.");
            return;
        }

        // Instantiate a new GameObject from the textPrefab
        GameObject textObject = Instantiate(textPrefab, textContainer.transform, false);
        textObject.name = "TextObject";

        // Try to find the Text component in the instantiated prefab
        Text textComponent = textObject.GetComponentInChildren<Text>(true);

        if (textComponent != null)
        {
            textComponent.text = textContent;
        }
        else
        {
            Debug.LogError("UnityEngine.UI.Text component not found on the prefab!");
            return;
        }

        // Assuming you have a similar script for handling clicks for the regular Text component
        // The TextClickHandler script should be already attached to the textPrefab
        TextClickHandler clickHandler = textObject.GetComponent<TextClickHandler>();
        if (clickHandler != null)
        {
            // clickHandler.open_now = open_now;
            clickHandler.place_id = place_id;
            clickHandler.PopUp = PopUp;
            clickHandler.PopUpNameText = PopUpNameText;
            clickHandler.PopUpRatingText = PopUpRatingText;
            clickHandler.PopUpIsOpenText = PopUpIsOpenText;
            clickHandler.PopUpPhoneText = PopUpPhoneText;
            clickHandler.PopUpDirectionsText = PopUpDirectionsText;
            clickHandler.PopUpToRemove = PopUpToRemove;
            clickHandler.PopUpWebsiteText = PopUpWebsiteText;
        }
        else
        {
            Debug.LogError("TextClickHandler component not found on the prefab!");
            return;
        }

        // Debug log for confirmation
        Debug.Log("Text object created with content: " + textComponent.text);
    }

    public void UpdatePanel(FindProximityMarkers.Place[] places)
    {
        if (places == null)
        {
            Debug.LogError("Results array is null.");
            return;
        }

        // Clear existing text elements
        foreach (Transform child in textContainer.transform)
        {
            Destroy(child.gameObject);
        }

        // Loop through the place array and create new text elements
        foreach (FindProximityMarkers.Place place in places)
        {
            try
            {
                string name = !string.IsNullOrEmpty(place.displayName.text) ? place.displayName.text : "N/A";
                // string icon = !string.IsNullOrEmpty(place.icon) ? place.icon : "N/A";
                // Make sure to handle potential null reference for opening_hours
                // string website = !string.IsNullOrEmpty(result.website) ? result.website : "N/A";
                // bool open_now = places.regularOpeningHours != null && place.regularOpeningHours.openNow;
                string place_id = !string.IsNullOrEmpty(place.id) ? place.id : "N/A";

                // Use the variables that now have either the data or default values
                CreateTextAsChild(name, place_id);
            }
            catch (Exception ex)
            {
                Debug.LogError("Error occurred when trying to update panel: " + ex);
            }
        }
    }
}
