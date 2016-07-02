#if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2014 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////
using UnityEngine;
using System;
using System.Collections;

public class AkTriggerButtonPress : AkTriggerBase
{
    void Start()
    {
        ButtonScript buttonCode = (ButtonScript)gameObject.GetComponentInChildren(typeof(ButtonScript));
        if (buttonCode)
        {
            buttonCode.OnButtonPressed += OnButtonPressed;
        }
    }

	void OnButtonPressed()
	{
		if(triggerDelegate != null) 
		{
			triggerDelegate(gameObject);
		}
	}
}

#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.