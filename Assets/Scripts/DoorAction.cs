using UnityEngine;
using System.Collections;

public class DoorAction : IUsableAction
{
    public float rotationSpeed = 0.7f;
    private bool isOpen = false;

    public override void DoAction()
    {
        if (isOpen)
        {
            return;
        }
        isOpen = true;
        StartCoroutine(OpenDoor());
    }

    public IEnumerator OpenDoor()
    {
        for (int i = 0; i < 90; i++)
        {
            yield return null;
            transform.Rotate(0, rotationSpeed, 0);
        }
    }
}
