using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System;
using UnityEngine.EventSystems;

public class TextClickHandler : MonoBehaviour, IPointerClickHandler
{
    public GameObject PopUpToRemove;
    public string name;
    public string address;
    public bool open_now;
    public string place_id;

    private const string BASE_URL = "https://maps.googleapis.com/maps/api/place/details/json?";
    private const string API_KEY = "AIzaSyAemXwdQeipL3w8oNrbgi58Ts6X4w2jQWo";

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
        public string url;
    }

    [Serializable]
    public class OpeningHours
    {
        public bool open_now;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        StartCoroutine(GetInfoOnPlace(place_id));
    }

    IEnumerator GetInfoOnPlace(string id)
    {
        string url = $"{BASE_URL}place_id={id}&key={API_KEY}";
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.LogError("Error: " + webRequest.error);
            }
            else
            {
                string jsonString = webRequest.downloadHandler.text;
                RootObject root = JsonUtility.FromJson<RootObject>(jsonString);

                if (root == null)
                {
                    Debug.LogError("Root is null.");
                    yield break;
                }

                if (root.status == "OK" && root.result != null)
                {
                    // Set default values if any of these are null or empty
                    string name = string.IsNullOrEmpty(root.result.name) ? "N/A" : root.result.name;
                    string website = string.IsNullOrEmpty(root.result.website) ? "No Website Found" : root.result.website;
                    string phoneNumber = string.IsNullOrEmpty(root.result.formatted_phone_number) ? "No Phone Number Found" : root.result.formatted_phone_number;
                    string directionsUrl = string.IsNullOrEmpty(root.result.url) ? "No Location Found" : root.result.url;
                    // Default value for rating could be 0, assuming a place cannot have a negative rating
                    float rating = root.result.rating > 0 ? root.result.rating : 0f;
                    float numberOfRatings = root.result.user_ratings_total > 0 ? root.result.user_ratings_total : 0f;
                    bool isOpen = root.result.opening_hours != null && root.result.opening_hours.open_now;

                    // Now, update the UI elements
                    PopUp.SetActive(true);
                    PopUpNameText.text = name;
                    PopUpRatingText.text = rating.ToString("F1") + "★" + $"({numberOfRatings})";
                    PopUpIsOpenText.text = isOpen ? "Open" : "Closed";
                    PopUpPhoneText.text = phoneNumber;

                        PopUpWebsiteText.text = website;
                    PopUpDirectionsText.text = directionsUrl;

                    if (PopUpToRemove != null) 
                    {
                        PopUpToRemove.SetActive(false);
                    }
                }
                else
                {
                    Debug.LogError($"API call failed with status: {root.status}");
                }
            }
        }
    }
}
