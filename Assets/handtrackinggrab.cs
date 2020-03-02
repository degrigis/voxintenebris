using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OculusSampleFramework;

public class handtrackinggrab : OVRGrabber
{

    private OVRHand hand;
    public float pinchTreshold = 0.7f;
    protected override void Start()
    {
        base.Start();
        GetComponent<OVRHand>();
    }

    public override void Update()
    {
        base.Update();
        CheckIndexPinch();
    }

    void CheckIndexPinch()
    {
        float pinchStrength = GetComponent<OVRHand>().GetFingerPinchStrength(OVRHand.HandFinger.Index);
        //float pinchStrength = hand.GetFingerPinchStrength(OVRHand.HandFinger.Index);
        //OVRLipSyncDebugConsole.print(OVRHand.HandFinger.Index);
        bool isPinching = pinchStrength > pinchTreshold;

        if(!m_grabbedObj && isPinching && m_grabCandidates.Count > 0)
        {
            GrabBegin();
        }else if(m_grabbedObj && !isPinching)
        {
            GrabEnd();
        }
    }

}