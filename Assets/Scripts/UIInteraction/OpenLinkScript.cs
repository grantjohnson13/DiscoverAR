using UnityEngine;
using UnityEngine.UI; // If using UI Text or Button

public class OpenLinkScript : MonoBehaviour
{
    public Text linkText;
    public void OpenURL()
    {
        if (linkText != null)
        {
            Application.OpenURL(linkText.text);
        }
        else
        {
            Debug.LogError("Link Text component is not assigned.");
        }
    }
}
