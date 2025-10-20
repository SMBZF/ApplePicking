using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class GrabEvent : MonoBehaviour
{
    public GameObject leftHandModel;
    public GameObject rightHandModel;

    // 抓取交互触发后，判断标签使得手部模型消失，可修改为其他触发功能
    void Start()
    {
        XRGrabInteractable grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(HideGrabbingHand);
        grabInteractable.selectExited.AddListener(ShowGrabbingHand);
    }

    public void HideGrabbingHand(SelectEnterEventArgs args)
    {
        
        if(args.interactorObject.transform.tag == "Left Hand")
        {
            leftHandModel.SetActive(false);
        }else if (args.interactorObject.transform.tag == "Right Hand")
        {
            rightHandModel.SetActive(false);
        }
    }

    public void ShowGrabbingHand(SelectExitEventArgs args)
    {
        if (args.interactorObject.transform.tag == "Left Hand")
        {
            leftHandModel.SetActive(true);
        }
        else if (args.interactorObject.transform.tag == "Right Hand")
        {
            rightHandModel.SetActive(true);
        }
    }


}
