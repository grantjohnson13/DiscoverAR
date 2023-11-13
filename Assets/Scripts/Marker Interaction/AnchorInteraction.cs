using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System;
using System.Collections.Generic;


[RequireComponent(typeof(Collider))]
public class AnchorInteraction : MonoBehaviour
{

    public GameObject PopUp;
    public Text PopUpNameText; 
    public Text PopUpRatingText; 
    public Text PopUpIsOpenText; 
    public Text PopUpPhoneText; 
    public Text PopUpWebsiteText; 
    public Text PopUpDirectionsText; 

    [Serializable]
    public class RootObject
    {
        public Result result;
        public string status;
    }

    [Serializable]
    public class Result
    {
        public string name;
        public string website;
        public float rating;
        public int user_ratings_total;
        public OpeningHours opening_hours;
        public string formatted_phone_number;
        // If by "directions" you mean the URL provided by Google, you can use this:
        public string url; 
    }

    [Serializable]
    public class OpeningHours
    {
        public bool open_now; // This indicates if the place is open at the time of the query
    }

    private const string BASE_URL = "https://maps.googleapis.com/maps/api/place/details/json?";
    private const string API_KEY = "AIzaSyAemXwdQeipL3w8oNrbgi58Ts6X4w2jQWo";

    public Text nameDisplay;  // Drag your UI Text component here in the inspector.
    public Text placeID;

    private string placeName;
    private string place_id;

    // This function will be called from the script where you instantiate the anchor.
    public void SetPlaceName(string name, string placeID)
    {
        Debug.Log($"{name} {placeID} reached aqui");
        placeName = name;
        place_id = placeID;
    }

    public void HandleTouch()
{
    Debug.Log("HandleTouch method called");
    if (placeName != null)
    {
        nameDisplay.text = placeName;
        placeID.text = place_id;
        Debug.Log($"{nameDisplay} with the id {place_id} has been selected");
        StartCoroutine(GetInfoOnPlace(place_id));
    }
}

IEnumerator GetInfoOnPlace(string id)
    {
        string url = $"{BASE_URL}place_id={id}&key={API_KEY}";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log("Error: " + webRequest.error);
            } 
            else
            {
                // Debug.Log("Response: " + webRequest.downloadHandler.text);

                string jsonString = webRequest.downloadHandler.text;
                RootObject root = JsonUtility.FromJson<RootObject>(jsonString);

                // Initialize default values
                string name = "N/A"; // Default value if 'name' is not present or empty
                string website = "No Website Found"; // Default value if 'website' is not present or empty
                float rating = 0.0f; // Default value if 'rating' is not present or empty
                int userRatingsTotal = 0; // Default value if 'user_ratings_total' is not present or empty
                bool isOpen = false; // Default value if 'opening_hours.open_now' is not present or empty
                string phoneNumber = "No Number Found"; // Default value if 'formatted_phone_number' is not present or empty
                string directionsUrl = "No Location Found"; // Default value if 'url' is not present or empty

                // Check if the status is OK before accessing the properties
                if (root.status == "OK")
                {
                    // Assign values from the JSON if they exist and are not empty
                    name = !string.IsNullOrEmpty(root.result.name) ? root.result.name : name;
                    website = !string.IsNullOrEmpty(root.result.website) ? root.result.website : website;
                    rating = root.result.rating != 0 ? root.result.rating : rating;
                    userRatingsTotal = root.result.user_ratings_total != 0 ? root.result.user_ratings_total : userRatingsTotal;
                    isOpen = root.result.opening_hours != null ? root.result.opening_hours.open_now : isOpen;
                    phoneNumber = !string.IsNullOrEmpty(root.result.formatted_phone_number) ? root.result.formatted_phone_number : phoneNumber;
                    directionsUrl = !string.IsNullOrEmpty(root.result.url) ? root.result.url : directionsUrl;

                    string openStatus = isOpen ? "Open" : "Closed";

                    // The response is good, and you can use the data
                    PopUp.SetActive(true);
                    Debug.Log("response successful " + name);
                    PopUpNameText.text = name;
                    PopUpRatingText.text = rating.ToString();
                    PopUpIsOpenText.text = openStatus;
                    PopUpPhoneText.text = phoneNumber;
                    PopUpDirectionsText.text = directionsUrl;
                }
                else
                {
                    Debug.LogError("Response status is not OK: " + root.status);
                }
            }
        }
    }
}
