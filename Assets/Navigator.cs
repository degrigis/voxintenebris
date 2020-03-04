using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Navigator : MonoBehaviour
{
    private OVRBoundary.BoundaryTestResult boundary_result;
    private OVRCameraRig PlayerCamera;
    private OVRPlayerController PlayerController;
    private Light RedLight;

    public GameObject Target;
    public GameObject BoundaryPoint;
    private LineRenderer lineRenderer;

    private AudioSource Heartbeat;
    private float initialHeartbeatPitch;
    private float initialLightIntensity;
    private Vector3 initialPlayerPosition;
    private OVRBoundary boundary;
    
    private float targetMinDistance;

    private System.Random random;

    private List<GameObject> guardianBoundariesPoint;

    private int countTargetsHit = 0;
    private enum Directions {
        LEFT,
        RIGHT,
        STRAIGHT
    }
    
    void Start()
    {
        Debug.Log("Navigator started!");
        PlayerCamera = GameObject.FindGameObjectWithTag("casa").GetComponent<OVRCameraRig>();
        PlayerController = GameObject.FindGameObjectWithTag("PlayerController").GetComponent<OVRPlayerController>();
        RedLight = GameObject.FindGameObjectWithTag("RedLight").GetComponent<Light>();
        lineRenderer = GetComponent<LineRenderer>();
        Heartbeat = PlayerController.GetComponent<AudioSource>();
        initialHeartbeatPitch = Heartbeat.pitch;
        initialLightIntensity = RedLight.intensity;
        initialPlayerPosition = PlayerCamera.centerEyeAnchor.position;
        boundary = OVRManager.boundary;
        guardianBoundariesPoint = new List<GameObject>();
        random = new System.Random();
        createNextTarget(PlayerCamera.centerEyeAnchor.position);
    }

    // Update is called once per frame 
    void Update()
    {
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
        // if(boundary_result_head.IsTriggering){
        //     // QuestDebug.Instance.Log("Ah triggered!");
        //     RedLight.color =  Color.blue;
        // }else{
        //     // QuestDebug.Instance.Log("Ah not triggered!");
        //     RedLight.color =  Color.green;
        // }
        String log = boundary_result_head.ClosestDistance.ToString();

        debugDrawForward(PlayerCamera.centerEyeAnchor.position, Target.transform.position);
        Directions dir = getleftOrRight(PlayerCamera.centerEyeAnchor.forward, Target.transform.position, PlayerCamera.centerEyeAnchor.up);

        float cur_distance = getDistanceFromTarget(PlayerCamera.centerEyeAnchor.position, Target.transform.position);
        scaleHeartbeat(cur_distance);
        scaleLight(cur_distance);

        // log += String.Format("\n x: {0}", PlayerCamera.centerEyeAnchor.position.x);
        // log += String.Format("\n y: {0}", PlayerCamera.centerEyeAnchor.position.y);
        // log += String.Format("\n z: {0}", PlayerCamera.centerEyeAnchor.position.z);
        // log += String.Format("\n Albi position: {0}", Target.transform.position);
        // log += String.Format("\n Target is at your {0}", dir);

        // QuestDebug.Instance.Log(log);

        // if (dir == Directions.RIGHT) {
        //     RedLight.color = Color.yellow;
        // }
        // else { 
        //     RedLight.color = Color.magenta;
        // }
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

    private float getDistanceFromTarget(Vector3 start, Vector3 end){
        return Vector3.Distance(start, end);
    }

    private void scaleHeartbeat(float distanceFromTarget) {
        float modifierFactor = distanceFromTarget - targetMinDistance;
        //I'm going away from the target
        if (modifierFactor > 0) {
            // if(Heartbeat.pitch < 2 ){
            Heartbeat.pitch = initialHeartbeatPitch + modifierFactor * 2;
            // }
        } 
        // We are going towards the target so we have to update mindistance
        else {
            Heartbeat.pitch = initialHeartbeatPitch;
            targetMinDistance = distanceFromTarget;
        }
    }

    private void scaleLight(float distanceFromTarget) {
        float modifierFactor = distanceFromTarget - targetMinDistance;
        //I'm going away from the target
        if (modifierFactor > 0) {
            RedLight.intensity = initialLightIntensity - modifierFactor * 10;
        } 
        // We are going towards the target so we have to update mindistance
        else {
            RedLight.intensity = initialLightIntensity;
        }
    }

    public void stepGame() {
        countTargetsHit += 1;
        QuestDebug.Instance.Log(String.Format("Hit {0} targets", countTargetsHit));
        Destroy(Target);
        foreach (var item in guardianBoundariesPoint)
        {
            Destroy(item);
        }
        guardianBoundariesPoint.Clear();
        // Put here the procedural stufff
        createNextTarget(PlayerCamera.centerEyeAnchor.position);
    }
	
    private void createNextTarget(Vector3 playerPosition){
        Vector3[] guardianBoundaries = boundary.GetGeometry(OVRBoundary.BoundaryType.OuterBoundary);
        // float maxDistance = -1;
        // float currDistance = -1;
        // Vector3 targetPosition = playerPosition;
        Vector3 targetPosition = guardianBoundaries[random.Next(guardianBoundaries.Length)];

        // for (int i = 0; i < Math.Min(1, guardianBoundaries.Length); i++)
        // {
        //     int idx = random.Next(guardianBoundaries.Length);
        //     Vector3 point = guardianBoundaries[idx];
            
        //     var asd = Instantiate(BoundaryPoint, point, transform.rotation);
        //     asd.transform.position += initialPlayerPosition; 
        //     asd.transform.position += new Vector3(0, 2f, 0);
        //     guardianBoundariesPoint.Add(asd);

        //     currDistance = getDistanceFromTarget(playerPosition, point + initialPlayerPosition + new Vector3(0, 2f, 0));
        //     if (maxDistance < currDistance){
        //         maxDistance = currDistance;
        //         targetPosition = point;
        //     }
        // }
        
        // foreach (Vector3 point in guardianBoundaries)
        // {
        //     // var asd = Instantiate(BoundaryPoint, point, transform.rotation);
        //     // asd.transform.position += initialPlayerPosition; 
        //     // asd.transform.position += new Vector3(0, 2f, 0);
        //     // guardianBoundariesPoint.Add(asd);

        //     currDistance = getDistanceFromTarget(playerPosition, point + initialPlayerPosition + new Vector3(0, 2f, 0));
        //     if (maxDistance < currDistance){
        //         maxDistance = currDistance;
        //         targetPosition = point;
        //     }
        // }
        Target = Instantiate(Target, targetPosition, transform.rotation);
        Target.transform.position += initialPlayerPosition; 
        Target.transform.position += new Vector3(0, 2f, 0);
        targetMinDistance = getDistanceFromTarget(playerPosition, Target.transform.position);
        // QuestDebug.Instance.Log("Created new target!");
    }
}
