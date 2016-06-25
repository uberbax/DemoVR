using UnityEngine;
using System.Collections;

public class Usable : MonoBehaviour {

    private HUDController playerHUD;
    private GameObject player;
    private bool inRange = false;

    public float distance;
    public string message;
    public KeyCode key;
    public IUsableAction onUse;

    void Awake()
    {
        playerHUD = FindObjectOfType<HUDController>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        float dist = (player.transform.position - transform.position).magnitude;


        if (dist < distance)
        {
            inRange = true;
            playerHUD.Show(message, this);
        }
        else
        {
            inRange = false;
            playerHUD.Hide(this);
        }

        //processing input
        if (inRange && Input.GetKeyDown(key) && onUse != null)
        {
            onUse.DoAction();
        }
    }
}
