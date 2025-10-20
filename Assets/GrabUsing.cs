using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class GrabUsing : MonoBehaviour
{
    public ParticleSystem particles;
    public GameObject WaveOne;
    public GameObject WaveTwo;
    public GameObject WaveThree;
    // Start is called before the first frame update
    void Start()
    {
        XRGrabInteractable grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.activated.AddListener(b => UnlockPower());
    }


    public void UnlockPower()
    {
        //触发UI提示物品信息
        //particles.Play();
        WaveOne.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
