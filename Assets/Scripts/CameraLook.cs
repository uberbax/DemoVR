using UnityEngine;
using System.Collections;

public class CameraLook : MonoBehaviour {

    private Transform player;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        Vector3 direction = player.position - transform.position;

        transform.forward = new Vector3(direction.x, 0, direction.z);
    }
}
