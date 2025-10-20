using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class BottonPushOpenDoor : MonoBehaviour
{
    public Animator animator;
    public string boolName = "Open";
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<XRSimpleInteractable>().selectEntered.AddListener(x => TooggleDoorOpen());
    }

    public void TooggleDoorOpen()
    {
        bool isOpen = animator.GetBool(boolName);
        animator.SetBool(boolName, !isOpen);
    }

}
