using UnityEngine;
using System.Collections;



public class ButtonScript : MonoBehaviour {

#if UNITY_WP8 || UNITY_IOS || UNITY_ANDROID
    string otherName = "Player";
#else
    string otherName = "First Person Controller";
#endif
    bool m_PlayerInTrigger = false;

    public delegate void ButtonPressed();
    public event ButtonPressed OnButtonPressed;

    void OnTriggerEnter(Collider in_other)
    {
        if (in_other.name == otherName)
        {
            m_PlayerInTrigger = true;
        }
    }

    void OnTriggerExit(Collider in_other)
    {
        if (in_other.name == otherName)
        {
            m_PlayerInTrigger = false;
        }
    }

	// Update is called once per frame
	void Update () 
    {
        if (OnButtonPressed != null && m_PlayerInTrigger && (Input.GetKeyDown("e") || Input.GetButtonUp("Fire1")) )
        {
            OnButtonPressed();
        }
	}
}
