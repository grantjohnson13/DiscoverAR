using UnityEngine;

public class UIInteraction : MonoBehaviour
{
    // This function is called every frame.
    private void Update()
    {
        // Check if the left mouse button is pressed.
        if (Input.GetMouseButtonDown(0))
        {
            CheckForClick();
        }
    }

    private void CheckForClick()
    {
        // Create a ray from the camera to the clicked point in the game.
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Check if the ray intersects any object in the game.
        if (Physics.Raycast(ray, out hit))
        {
            // Check if the clicked object is a child of this parent object
            if (hit.collider.transform.IsChildOf(this.transform))
            {
                // Log to the console.
                Debug.Log($"{hit.collider.gameObject.name} was clicked!");
            }
        }
    }
}
