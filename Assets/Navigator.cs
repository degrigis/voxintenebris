using System;
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

    public GameObject albi;
    private LineRenderer lineRenderer;
    // Start is called before the first frame update
    
    private enum Directions {
        LEFT,
        RIGHT,
        STRAIGHT
    }
    
    void Start()
    {
        Debug.Log("Navigator started!");
        Player = GameObject.FindGameObjectWithTag("casa").GetComponent<OVRCameraRig>();
        RedLight = GameObject.FindGameObjectWithTag("RedLight").GetComponent<Light>();
        lineRenderer = GetComponent<LineRenderer>();
        albi = GameObject.FindGameObjectWithTag("albi");
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

        debugDrawForward(Player.centerEyeAnchor.position, albi.transform.position);
        Directions dir = getleftOrRight(Player.centerEyeAnchor.forward, albi.transform.position, Player.centerEyeAnchor.up);

        log += String.Format("\n x: {0}", Player.centerEyeAnchor.position.x);
        log += String.Format("\n y: {0}", Player.centerEyeAnchor.position.y);
        log += String.Format("\n z: {0}", Player.centerEyeAnchor.position.z);
        log += String.Format("\n albi is at your {0}", dir);

        QuestDebug.Instance.Log(log);

        if (dir == Directions.RIGHT) {
            RedLight.color = Color.yellow;
        }
        else { 
            RedLight.color = Color.magenta;
        }
        //Debug.DrawRay(Player.centerEyeAnchor.position, Player.centerEyeAnchor.forward * 20, Color.red, 2.5f);
        //debugDrawForward(Player.centerEyeAnchor.position +  Player.centerEyeAnchor.forward * 2,  Player.centerEyeAnchor.forward * 20 + Player.centerEyeAnchor.position);
        // Debug.Log(Player.transform.position.x);
        // QuestDebug.Instance.Log(boundary_result_head.ClosestDistance.ToString());
    }

    // private string chooseDirection(){

    // }

    private void debugDrawForward(Vector3 start, Vector3 end){
        lineRenderer.SetVertexCount(2);
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
    }

   	private Directions getleftOrRight(Vector3 fwd, Vector3 targetDir, Vector3 up) {
		Vector3 perp = Vector3.Cross(fwd, targetDir);
		float dir = Vector3.Dot(perp, up);
		
		if (dir > 0f) {
			return Directions.RIGHT; 
		} else if (dir < 0f) {
			return Directions.LEFT;
		} else {
			return Directions.STRAIGHT;
		}
	}
	
}
