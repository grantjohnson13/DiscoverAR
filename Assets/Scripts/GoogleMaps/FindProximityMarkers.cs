// ... other using statements ...
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI; // If you want to display logs on a UI Text component
using System.Collections;
using Google.XR.ARCoreExtensions.GeospatialCreator.Internal;
using System;
using System.Collections.Generic;
using System.Linq;


public class FindProximityMarkers : MonoBehaviour
{
    private const string BASE_URL = "https://places.googleapis.com/v1/places:searchNearby";
    private const string BASE_URL_GeoCoding = "https://maps.googleapis.com/maps/api/geocode/json";
    private const string API_KEY = "<insert api key here>";
    private const int RADIUS = 1000;
    public GameObject GenAIInfo;

    public GameObject locationPrefab; // Drag your prefab here in the inspector

    public PanelUpdater panelUpdater;
    public Material restaurantMaterial;
    public Material defaultMaterial;
    public Material shoppingMaterial;

    
    // Optional: A Text component to display logs/status. Attach in Inspector.
    public Text logText;


    [System.Serializable]
    public class AddressComponent
    {
        public string long_name;
        public string short_name;
        public List<string> types;
    }

    [System.Serializable]
    public class Location
    {
        public float latitude;
        public float longitude;
    }

    // public class Geometry
    // {
    //     public Location location;
    //     public string location_type;
    // }

    // [System.Serializable]
    // public class Result
    // {
    //     public List<AddressComponent> address_components;
    //     public string formatted_address;
    //     public Geometry geometry;
    //     public string place_id;
    //     public PlusCode plus_code;
    //     public List<string> types;
    // }

    [System.Serializable]
    public class NearbySearchResponse
    {
        public Place[] places;
    }

    [System.Serializable]
    public class Place
    {
        public DisplayName displayName;
        public string id;
        public List<string> types;
        public string nationalPhoneNumber;
        public string internationalPhoneNumber;
        public string formattedAddress;
        public List<AddressComponent> addressComponents;
        public PlusCode plusCode;
        public Location location;
        public float rating;
        public string googleMapsUri;
        public string websiteUri;
        public RegularOpeningHours regularOpeningHours; // Added
    }

    [System.Serializable]
    public class DisplayName
    {
        public string text;
    }

    [System.Serializable]
    public class RegularOpeningHours
    {
        public bool openNow;
    }


    [System.Serializable]
    public class PlusCode
    {
        public string globalCode;
        public string compoundCode;
    }
    public class LocationCode
    {
        public string compound_code;
        public string global_code;
    }

    [System.Serializable]
    public class GeoCodingResponse
    {
        public LocationCode plus_code;
    }



    void Start()
    {
        StartCoroutine(GetDeviceLocation());
    }

    IEnumerator GetDeviceLocation()
    {
        // Check if user has location service enabled
        if (!Input.location.isEnabledByUser)
        {
            Log("Location service not enabled by user.");
            yield break;
        }

        // Start the location service
        Input.location.Start();

        // Wait until service initializes
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // Service didn't initialize in 20 seconds
        if (maxWait < 1)
        {
            Log("Location service timed out.");
            yield break;
        }

        // Connection failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Log("Unable to determine device location.");
            yield break;
        }
        else
        {
            // Get latitude and longitude
            float latitude = Input.location.lastData.latitude;
            float longitude = Input.location.lastData.longitude;
            
            // Use the location data (latitude and longitude)
            StartCoroutine(GetNearbyPlaces(latitude, longitude));
            StartCoroutine(CurrentCity(latitude, longitude));
        }

        // Stop the location service
        Input.location.Stop();
    }

    IEnumerator CurrentCity(float latitude, float longitude)
    {
        string url = $"{BASE_URL_GeoCoding}?latlng={latitude},{longitude}&key={API_KEY}";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                Log("Error: " + webRequest.error);
            } 
            else
            {
                Debug.Log(webRequest.downloadHandler.text);
                // Parsing the JSON
                GeoCodingResponse response = JsonUtility.FromJson<GeoCodingResponse>(webRequest.downloadHandler.text);

                try
                {
                    // Here you can now extract the compound_code
                    string compoundCode = response.plus_code.compound_code;
                    int index = compoundCode.IndexOf(' ');
                    if (index >= 0) // Make sure a space was found
                    {
                        string city = compoundCode.Substring(index + 1);
                        Debug.Log("Modified Compound Code: " + city);
                        SendCompoundCode(city);
                    }
                    else
                    {
                        Debug.LogError("No space found in compound code");
                    }
                    
                    // If you need to update a panel or UI with this information
                    // panelUpdater.UpdatePanel(compoundCode);

                    // Continue with your logic, if there's more
                }
                catch (Exception ex)
                {
                    Debug.LogError("Error " + ex);
                }
            }
        }
    }

   IEnumerator GetNearbyPlaces(float latitude, float longitude)
    {
        // float latitudeTest = 37.691215f;
        // float longitudeTest = -97.349626f;
        // Create the request body

        string jsonBody = @"
        {
            ""maxResultCount"": 20,
            ""rankPreference"": ""DISTANCE"",
            ""locationRestriction"": {
                ""circle"": {
                    ""center"": { ""latitude"": " + latitude + @", ""longitude"": " + longitude + @" },
                    ""radius"": 1000.0
                }
            }
        }";

        // Convert body to JSON string
        Debug.Log("json body " + jsonBody);

        // Create a new UnityWebRequest for a POST request
        using (UnityWebRequest webRequest = new UnityWebRequest(BASE_URL, "POST"))
        {
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonBody);
            webRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
            webRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

            // Set headers
            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.SetRequestHeader("X-Goog-Api-Key", API_KEY);
            webRequest.SetRequestHeader("X-Goog-FieldMask", "*");

            Debug.Log("webRequest " + webRequest);

            // Send the request
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                Log("Error: " + webRequest.error);
            } 
            else
            {
                // Parsing the JSON
                NearbySearchResponse response = JsonUtility.FromJson<NearbySearchResponse>(webRequest.downloadHandler.text);
                Debug.Log("response from api" + webRequest.downloadHandler.text);
                try
                {
                    // Update panel with new data structure
                    panelUpdater.UpdatePanel(response.places);
                }
                catch (Exception ex)
                {
                    Debug.LogError("Error: " + ex);
                }

                foreach (Place place in response.places)
                {
                    // Updated logging according to the new response structure
                    // Log($"Place Name: {place.name}, Location: {place.location.latitude}, {place.location.longitude}");

                    try
                    {
                        // Instantiate object at the given location
                        InstantiateLocationObject(place.location.latitude, place.location.longitude, place.displayName.text, place.id ,place.types);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError("Error with sending objects: " + ex);
                    }
                }
            }
        }
    }



    void InstantiateLocationObject(float lat, float lng, string placeName, string place_id, List<string> types)
    {
        // Convert the latitude and longitude into a position (this might need adjustment depending on your implementation)
        Vector3 position = new Vector3(lat, 0, lng); // Note: This is a simplistic conversion. You might need a more sophisticated method depending on your world scale and mapping.

        // Instantiate the prefab
        GameObject instance = Instantiate(locationPrefab, position, Quaternion.identity);
        // Find the child GameObject named 'cube'
        Transform cubeTransform = instance.transform.Find("Cube");
        if (cubeTransform != null)
        {
            MeshRenderer cubeMeshRenderer = cubeTransform.GetComponent<MeshRenderer>();
            if (cubeMeshRenderer != null)
            {
                Material selectedMaterial = DetermineMaterial(types);
                if (selectedMaterial != null)
                {
                    cubeMeshRenderer.material = selectedMaterial;
                }
            }
            else
            {
                Debug.LogError("MeshRenderer not found on 'cube' child object.");
            }
        }
        else
        {
            Debug.LogError("'cube' child object not found in prefab.");
        }
        // Set the properties, for example:
        ARGeospatialCreatorAnchor anchor = instance.GetComponent<ARGeospatialCreatorAnchor>();
        if (anchor != null)
        {
            anchor.Latitude = lat;
            anchor.Longitude = lng;
            // Set other properties as needed
        }

        AnchorInteraction interaction = instance.GetComponent<AnchorInteraction>();
        if (interaction != null)
        {
            interaction.SetPlaceName(placeName, place_id);
            Log($"Successfully created {placeName} with the place_id {place_id}");
        }
    }

    Material DetermineMaterial(List<string> types)
    {
        var lowerTypes = types.ConvertAll(t => t.ToLower());
        // Check for specific types and return the corresponding prefab
        string typesString = string.Join(", ", lowerTypes);
        Debug.Log("listy listy " + typesString);
        if (lowerTypes.Any(t => t.Contains("restaurant")) || lowerTypes.Any(t => t.Contains("bar")))
        {
            return restaurantMaterial;
        }
        else if (lowerTypes.Any(t => t.Contains("store")) || lowerTypes.Any(t => t.Contains("shopping")) || lowerTypes.Any(t => t.Contains("establishment")))
        {
            return shoppingMaterial;
        }
        else
        {
            return defaultMaterial;
        }

    }

    private void SendCompoundCode(string modifiedCode)
    {
        if (GenAIInfo != null)
        {
            // Get the Searching script component and call a method on it
            Searching searchingScript = GenAIInfo.GetComponent<Searching>();
            if (searchingScript != null)
            {
                searchingScript.SetCity(modifiedCode);
            }
            else
            {
                Debug.LogError("Searching script not found on GenAIInfo GameObject.");
            }
        }
        else
        {
            Debug.LogError("GenAIInfo GameObject is not assigned.");
        }
    }

    // Log method to optionally update a UI Text component
    void Log(string message)
    {
        Debug.Log(message);
        if (logText != null)
        {
            logText.text = message;
        }
    }

}

