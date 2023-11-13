using UnityEngine;
using System;


public class ButtonLogger : MonoBehaviour
{

    public Animator GoogleButton;

    public Animator Panel;

    private void Start()
    {
        GoogleButton = GetComponent<Animator>();
        Panel = GetComponent<Animator>();
    }
    public void LogButtonClick()
    {
        Debug.Log("Button was clicked!");

        try
        {
            // Activate the animation
            if (GoogleButton != null)
            {
                GoogleButton.SetTrigger("GoogleIconAnimation");
                Debug.Log("Setting Animation");
            }
            else{
                Debug.Log("Animator not found");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error setting animation: {e.Message}");
        }

        try
        {
            // Activate the animation
            if (Panel != null)
            {
                Panel.SetTrigger("PanelEnlarge");
                Debug.Log("Setting Animation Panel Enlarge");
            }
            else{
                Debug.Log("Animator not found");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error setting animation: {e.Message}");
        }

    }
}
