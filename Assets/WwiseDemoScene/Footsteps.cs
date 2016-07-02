using UnityEngine;
using System.Collections;

public class Footsteps : MonoBehaviour {

    float footstepTimer = 0.0f;

    Vector3 lastPos = new Vector3(0, 0, 0);

	// Update is called once per frame
	void Update () {

        bool isMoving = false;
        Transform thisTransform = (Transform)GetComponent("Transform");
        if (lastPos.x != thisTransform.position.x || lastPos.y != thisTransform.position.y)
        {
            isMoving = true;
        }

        lastPos = thisTransform.position;

        if (/*Input.GetAxis("Horizontal") != 0.0f || Input.GetAxis("Vertical") != 0.0f ||*/ isMoving)
        {
            if (footstepTimer > 0.30f)
            {
                AkSoundEngine.PostEvent("Footstep", gameObject);
                footstepTimer = 0.0f;
            }

            footstepTimer += Time.deltaTime;
        }
	}
}
