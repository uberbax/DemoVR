using UnityEngine;
using System.Collections;

public class enciende : MonoBehaviour {

	public GameObject elCubo;
	private bool isOn=false;
	public GameObject DirectionalLight;
	private bool DirLightIsOn=true;

	void OnGUI() {
		if (GUI.Button (new Rect (20, 20, 150, 60), "Crate On/Off")) {
			if (!isOn) {
				shaderGlow gls= elCubo.GetComponent<shaderGlow>();
				gls.lightOn();
				isOn=true;
			}
			else {
				shaderGlow gls= elCubo.GetComponent<shaderGlow>();
				gls.lightOff();
				isOn=false;
			}
		}

		if (GUI.Button (new Rect (20, 90, 150, 60), "Light On/Off")) {
			if (!DirLightIsOn) {
				DirectionalLight.SetActive(true);
				DirLightIsOn=true;
			}
			else {
				shaderGlow gls= elCubo.GetComponent<shaderGlow>();
				gls.lightOff();
				DirectionalLight.SetActive(false);
				DirLightIsOn=false;
			}
		}
		
	}

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
