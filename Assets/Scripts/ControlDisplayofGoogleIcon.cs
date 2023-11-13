using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlDisplayofGoogleIcon : MonoBehaviour
{
    public GameObject GoogleIcon;
    public GameObject GoogleBackArrow;

    public void Activate(){
        if(GoogleIcon.activeSelf){
            StartCoroutine(ControlIcon(GoogleIcon, GoogleBackArrow));
        }else{
            StartCoroutine(ControlIcon(GoogleBackArrow, GoogleIcon));
        }
    }
    public IEnumerator ControlIcon(GameObject Icon, GameObject SecondIcon){
        SecondIcon.SetActive(true);
        yield return new WaitForSeconds(.25f);
        Icon.SetActive(false);
    }

}
