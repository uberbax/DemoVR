using UnityEngine;
using System.Collections;

public class SubtitleDemo : MonoBehaviour {

    TextMesh m_RawMarkerText = null;
    TextMesh m_SubtitleText = null;

#if UNITY_EDITOR
    private static string activateButtonName = "e";
#elif UNITY_PS3 || UNITY_PS4 || UNITY_PSP2
    private static string activateButtonName = "X";
#elif UNITY_XBOX360 || UNITY_XBOXONE
    private static string activateButtonName = "A";
#else
    private static string activateButtonName = "e";
#endif

#if (UNITY_WP8 || UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
	private static string instructionString = "Tap the screen to start demo";
#else
	private static string instructionString = "Press \'" + activateButtonName + "\' on the button to start demo";
#endif

    // Array containing the subtitles.
    private static string[] ms_EnglishSubtitles = new string[]
	{
		"",
		"In this tutorial...",
		"...we will look at creating...",
		"...actor-mixers...",
		"...and control buses.",
		"We will also look at the...",
		"...actor-mixer and master-mixer structures...",
		"...and how to manage these structures efficiently.",
        "END OF DEMO."
	};

	// Use this for initialization
	void Start ()
    {
        // Get the subtitle text component here, to avoid doing it every callback.
        TextMesh[] foundText = gameObject.GetComponentsInChildren<TextMesh>();
        foreach (TextMesh text in foundText)
        {
            if (text.name == "Subtitle Text")
            {
                m_SubtitleText = text;
            }
            if (text.name == "Raw Marker Text")
            {
                m_RawMarkerText = text;
            }

            if (text.name == "Instruction Text")
            {
                text.text = SubtitleDemo.instructionString;
            }
        }
	}
	
    void MarkerCallback(object in_callbackInfo)
    {
        AkEventCallbackMsg callbackInfo = (AkEventCallbackMsg)in_callbackInfo;
        AkCallbackManager.AkMarkerCallbackInfo MarkerCallbackInfo = (AkCallbackManager.AkMarkerCallbackInfo)callbackInfo.info;

        m_RawMarkerText.text = MarkerCallbackInfo.strLabel;

        m_SubtitleText.text = ms_EnglishSubtitles[MarkerCallbackInfo.uIdentifier];
    }
}
