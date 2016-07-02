using UnityEngine;
using System.Collections;

public class SetController : MonoBehaviour {

	// Use this for initialization
	void Start () 
    {
        GameObject firstPersonController = GameObject.Find("First Person Controller");
        GameObject mobileFirstPersonController = GameObject.Find("First Person Controls");

#if (UNITY_WP8 || UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
        firstPersonController.SetActive(false);
        mobileFirstPersonController.SetActive(true);
#else
        firstPersonController.SetActive(true);
        mobileFirstPersonController.SetActive(false);
#endif
    }
}
