using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARTouchHandler : MonoBehaviour
{
    private Camera arCamera;

    private void Start()
    {
        // Get the AR camera
        arCamera = GetComponent<ARSessionOrigin>().camera;
    }

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = arCamera.ScreenPointToRay(touch.position);
                RaycastHit hitObject;
                
                if (Physics.Raycast(ray, out hitObject))
                {
                    Debug.Log("Touchy Touchy");
                    hitObject.transform.gameObject.SendMessage("HandleTouch", SendMessageOptions.DontRequireReceiver);
                }
            }
        }
    }
}
