﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Navigator : MonoBehaviour
{
private OVRBoundary boundary;
private AudioSource audioData;
    private OVRBoundary.BoundaryTestResult boundary_result;
    private OVRCameraRig Player;
    private Light RedLight;

    private LineRenderer lineRenderer;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Navigator started!");
        Player = GameObject.FindGameObjectWithTag("casa").GetComponent<OVRCameraRig>();
        RedLight = GameObject.FindGameObjectWithTag("RedLight").GetComponent<Light>();
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame 
    void Update()
    {
        boundary = OVRManager.boundary;
        /*
        This works!
        */
        /*
        if(boundary.GetVisible()){
            Debug.Log("Ah triggered!"); 
            GameObject.Find("GettingOut").GetComponent<AudioSource>().Play(0);
            GameObject.Find("RedLight").GetComponent<Light>().color =  Color.blue;
        }else{
            Debug.Log("Ah not triggered!");
            GameObject.Find("GettingOut").GetComponent<AudioSource>().Pause(0);
            GameObject.Find("RedLight").GetComponent<Light>().color =  Color.red;
        }
        */
        // OVRBoundary.BoundaryTestResult boundary_result_head = boundary.TestNode(OVRBoundary.Node.Head, OVRBoundary.BoundaryType.OuterBoundary);
        // OVRBoundary.BoundaryTestResult boundary_result_left_hand = boundary.TestNode(OVRBoundary.Node.HandLeft, OVRBoundary.BoundaryType.OuterBoundary);
        // OVRBoundary.BoundaryTestResult boundary_result_right_hand = boundary.TestNode(OVRBoundary.Node.HandRight, OVRBoundary.BoundaryType.OuterBoundary);
        // if(boundary_result_head.IsTriggering  || boundary_result_left_hand.IsTriggering || boundary_result_right_hand.IsTriggering ){
        //     QuestDebug.Instance.Log("Ah triggered!");
        //     GameObject.Find("RedLight").GetComponent<Light>().color =  Color.blue;
        // }else{
        //     QuestDebug.Instance.Log("Ah not triggered!");
        //      GameObject.Find("RedLight").GetComponent<Light>().color =  Color.red;
        // }  

        OVRBoundary.BoundaryTestResult boundary_result_head = boundary.TestNode(OVRBoundary.Node.Head, OVRBoundary.BoundaryType.OuterBoundary);
        // OVRBoundary.BoundaryTestResult boundary_result_left_hand = boundary.TestNode(OVRBoundary.Node.HandLeft, OVRBoundary.BoundaryType.OuterBoundary);
        // OVRBoundary.BoundaryTestResult boundary_result_right_hand = boundary.TestNode(OVRBoundary.Node.HandRight, OVRBoundary.BoundaryType.OuterBoundary);
        if(boundary_result_head.IsTriggering){
            // QuestDebug.Instance.Log("Ah triggered!");
            RedLight.color =  Color.blue;
        }else{
            // QuestDebug.Instance.Log("Ah not triggered!");
            RedLight.color =  Color.green;
        }
        String log = boundary_result_head.ClosestDistance.ToString();
        log += String.Format("\n x: {0}", Player.centerEyeAnchor.localPosition.x);
        log += String.Format("\n y: {0}", Player.centerEyeAnchor.localPosition.y);
        log += String.Format("\n z: {0}", Player.centerEyeAnchor.localPosition.z);
        QuestDebug.Instance.Log(log);
        //Debug.DrawRay(Player.centerEyeAnchor.position, Player.centerEyeAnchor.forward * 20, Color.red, 2.5f);
        //debugDrawForward();
        // Debug.Log(Player.transform.position.x);
        // QuestDebug.Instance.Log(boundary_result_head.ClosestDistance.ToString());
    }

    // private string chooseDirection(){

    // }

    private void debugDrawForward(){
        lineRenderer.SetVertexCount(2);
        lineRenderer.SetPosition(0, Player.centerEyeAnchor.position +  Player.centerEyeAnchor.forward * 2);
        lineRenderer.SetPosition(1, Player.centerEyeAnchor.forward * 20 + Player.centerEyeAnchor.position);
    }
}
