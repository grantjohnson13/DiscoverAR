using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.Networking;
using System.Text;
using Newtonsoft.Json.Linq;

public class Searching : MonoBehaviour
{

    [System.Serializable]
    public class ResponseData
    {
        public Candidate[] candidates;
    }
    [System.Serializable]
    public class Candidate
    {
        public string output;
    }
    public TMP_InputField inputField;
    public GameObject UserTextPrefab;
    public GameObject AITextPrefab;
    public Transform layoutGroupTransform;
    private GameObject newTextObject;

    string CurrentCityLocation = "";
    string ItemClicked = "";


    private const string BASE_URL = "https://generativelanguage.googleapis.com/v1beta2/models/text-bison-001:generateText";
    private const string API_KEY = "<insert api key here>";


    public void openAndroidKeyboard()
{
    Debug.Log("openAndroidKeyboard got selected");
    inputField.gameObject.SetActive(true);
    inputField.text = "";
    inputField.ActivateInputField();
    inputField.Select();
    inputField.onEndEdit.AddListener(HandleEndEdit);
    TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
}
// private void hideInputField(string text)
// {
//     if(!string.IsNullOrWhiteSpace(text))
//     {
//         TouchScreenKeyboard.hideInput = true;
//         CreateTextObject(text);
//     }
// }

private void HandleEndEdit(string text){
    Debug.Log("The user has finished editing the text " + text);
    if(!string.IsNullOrWhiteSpace(text))
    {
        CreateTextObject(text, true);
        StartCoroutine(AIResponse(text));
    }
}

private void CreateTextObject(string text, bool isUser)
{
    if(isUser == true){
        newTextObject = Instantiate(UserTextPrefab, layoutGroupTransform);
    }else{
        newTextObject = Instantiate(AITextPrefab, layoutGroupTransform);
    }
    TMP_Text newTextComponent = newTextObject.GetComponent<TMP_Text>();

    if(newTextComponent != null)
    {
        newTextComponent.text = text;
    }
    else
    {
        Debug.LogError("Text Component not found on the prefab.");
    }
}

IEnumerator AIResponse(string text)
    {
        string url = $"{BASE_URL}?&key={API_KEY}";
        string prompt = "You are a tour guide and I am a tourist.";
        if(CurrentCityLocation != "")
        {
            prompt += " I am currently in " + CurrentCityLocation + ". ";
        }

        if(ItemClicked != "")
        {
            prompt += "I am asking about " + ItemClicked + ". ";
        }

        prompt += text;
        Debug.Log("prompting " + prompt);
        WWWForm form = new WWWForm();
        string jsonData = $"{{\"prompt\": {{\"text\": \"{prompt}\"}}}}";
        byte[] postData = Encoding.UTF8.GetBytes(jsonData);

        using (UnityWebRequest webRequest = UnityWebRequest.Post(url, "POST"))
        {
            webRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(postData);
            webRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log("Error: " + webRequest.error);
            } 
            else
            {
                Debug.Log("Response Yipee " + webRequest.downloadHandler.text);
                ResponseData responseData = JsonUtility.FromJson<ResponseData>(webRequest.downloadHandler.text);
                foreach (var candidate in responseData.candidates){
                    Debug.Log(candidate.output);
                    CreateTextObject(candidate.output, false);
                }
            }
        }
    }

    public void SetCity(string city)
    {
        CurrentCityLocation = city;
    }

    public void SetItemClicked(GameObject clickedObject)
    {
        Text textComponent = clickedObject.GetComponent<Text>();
        if (textComponent != null)
        {
            ItemClicked = textComponent.text;
        }
        else
        {
            Debug.LogError("Text component not found on the clicked object.");
        }
    }

}
