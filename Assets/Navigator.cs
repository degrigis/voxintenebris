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

    public GameObject TargetPrefab;
    private GameObject CurrentTarget;
    public GameObject BoundaryPoint;
    public GameObject BenignSpirit;
    public GameObject EvilDoor;
    public GameObject SmashLightPrefab;
    private AudioSource SmashLightSound;
    private AudioSource BenignStayStillAudio;
    private AudioSource BenignGoingDownAudio;
    private AudioSource BenignStayDownAudio;
    private AudioSource BenignStandUpAudio;

    private AudioSource BenignSmashLight;
    private AudioSource BadSpiritYouDead;
    private LineRenderer lineRenderer;

    private AudioSource Heartbeat;
    private AudioSource Breath;

    private float initialHeartbeatPitch;
    private float initialLightIntensity;
    private Vector3 initialPlayerPosition;
    private Vector3 initialPlayerLocalPosition;

    private float TimerDeadline;
    private float DeadlineDelta = 30;

    private OVRBoundary boundary;
    
    public float targetChangeThreshold = 3;
    private float targetMinDistance;

    private Boolean already_hit;

    private System.Random random;

    private List<GameObject> guardianBoundariesPoint;

    private int countTargetsHit = 0;
    private int methodCounter = 0;
    private float stepGameTimer;
    private float stepGameDelay = 1f;
    private enum Directions {
        LEFT,
        RIGHT,
        STRAIGHT
    }
    private Vector3 playerStillPosition;
    private bool isPlayerStill = false;
    private bool isPlayerGoingDown = false;
    private bool isPlayerDown = false;
    
    void Start()
    {
        Debug.Log("Navigator started!");
        stepGameTimer = 0f;
        PlayerCamera = GameObject.FindGameObjectWithTag("casa").GetComponent<OVRCameraRig>();
        PlayerController = GameObject.FindGameObjectWithTag("PlayerController").GetComponent<OVRPlayerController>();
        RedLight = GameObject.FindGameObjectWithTag("RedLight").GetComponent<Light>();
        lineRenderer = GetComponent<LineRenderer>();
        Heartbeat = PlayerController.GetComponents<AudioSource>()[0];
        Breath = PlayerController.GetComponents<AudioSource>()[1];

        // Getting audio sources 
        BenignSmashLight = GetComponents<AudioSource>()[6];
        BadSpiritYouDead = GetComponents<AudioSource>()[5];
        BenignStandUpAudio = GetComponents<AudioSource>()[4];
        BenignStayDownAudio = GetComponents<AudioSource>()[3];
        BenignGoingDownAudio = GetComponents<AudioSource>()[2];
        BenignStayStillAudio = GetComponents<AudioSource>()[1];
        SmashLightSound = GetComponents<AudioSource>()[0];

        // Set initial variables 
        initialHeartbeatPitch = Heartbeat.pitch;
        initialLightIntensity = RedLight.intensity;
        initialPlayerPosition = PlayerCamera.centerEyeAnchor.position;
        initialPlayerLocalPosition = PlayerCamera.centerEyeAnchor.localPosition;
        boundary = OVRManager.boundary;
        guardianBoundariesPoint = new List<GameObject>();
        random = new System.Random();

        //QuestDebug.Instance.Log(initialPlayerPosition.ToString());
        // EvilDoor = Instantiate(EvilDoor, new Vector3(0,0,0) + new Vector3(initialPlayerLocalPosition.x, 0, initialPlayerLocalPosition.z), transform.rotation);
        // EvilDoor = Instantiate(EvilDoor, new Vector3(0,0,0) + , transform.rotation);
 
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

        // OVRBoundary.BoundaryTestResult boundary_result_head = boundary.TestNode(OVRBoundary.Node.Head, OVRBoundary.BoundaryType.OuterBoundary);
        // OVRBoundary.BoundaryTestResult boundary_result_left_hand = boundary.TestNode(OVRBoundary.Node.HandLeft, OVRBoundary.BoundaryType.OuterBoundary);
        // OVRBoundary.BoundaryTestResult boundary_result_right_hand = boundary.TestNode(OVRBoundary.Node.HandRight, OVRBoundary.BoundaryType.OuterBoundary);
        // if(boundary_result_head.IsTriggering){
        //     // QuestDebug.Instance.Log("Ah triggered!");
        //     RedLight.color =  Color.blue;
        // }else{
        //     // QuestDebug.Instance.Log("Ah not triggered!");
        //     RedLight.color =  Color.green;
        // }
        // String log = boundary_result_head.ClosestDistance.ToString();

        // debugDrawForward(PlayerCamera.centerEyeAnchor.position, Target.transform.position);
        stepGameTimer += Time.deltaTime;
        TimerDeadline += Time.deltaTime;
        
        if(TimerDeadline > DeadlineDelta){
            killPlayer();
        }

        if (isPlayerStill && BenignStayStillAudio.isPlaying) { 
            TimerDeadline = 0;
            var currentPosition = PlayerCamera.centerEyeAnchor.position;
            if (currentPosition.x > playerStillPosition.x + 0.5 || currentPosition.x < playerStillPosition.x - 0.5 || 
                currentPosition.z > playerStillPosition.z + 0.5 || currentPosition.z < playerStillPosition.z - 0.5){
                    killPlayer();
                }
        }

        if (isPlayerStill && !BenignStayStillAudio.isPlaying) {
            TimerDeadline = 0;
            countTargetsHit = 0;
            isPlayerStill = false;
            createNextTarget(PlayerCamera.centerEyeAnchor.position);
        }

        if (isPlayerGoingDown && !BenignGoingDownAudio.isPlaying) {
            TimerDeadline = 0;
            isPlayerGoingDown = false;
            isPlayerDown = true;
            BenignStayDownAudio.Play(0);
        }

        if (isPlayerDown && BenignStayDownAudio.isPlaying) {
            TimerDeadline = 0;
            var currentPosition = PlayerCamera.centerEyeAnchor.position;
            if (currentPosition.y > playerStillPosition.y - 0.5) {
                killPlayer();
            }
        }

        if (isPlayerDown && !BenignStayDownAudio.isPlaying) {
            TimerDeadline = 0;
            BenignStandUpAudio.Play(0);
            countTargetsHit = 0;
            isPlayerDown = false;
            createNextTarget(PlayerCamera.centerEyeAnchor.position);
        }

        if(CurrentTarget != null) {
            Directions dir = getleftOrRight(PlayerCamera.centerEyeAnchor.forward, CurrentTarget.transform.position, PlayerCamera.centerEyeAnchor.up);
            float cur_distance = getDistanceFromTarget(PlayerCamera.centerEyeAnchor.position, CurrentTarget.transform.position);
            scaleHeartbeat(cur_distance);
            scaleLight(cur_distance);
        }

        scaleBreath();
      
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

    private void killPlayer(){        
        
        BenignStandUpAudio.Stop();
        BenignStayDownAudio.Stop();
        BenignGoingDownAudio.Stop();
        BenignStayStillAudio.Stop();
        SmashLightSound.Stop();  
        Heartbeat.Stop();
        Breath.Stop();
        BadSpiritYouDead.Play();

        QuestDebug.Instance.Log("You dead!");
        
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
            Heartbeat.pitch = initialHeartbeatPitch + modifierFactor * 1.3f;
            // }
        } 
        // We are going towards the target so we have to update mindistance
        else {
            Heartbeat.pitch = initialHeartbeatPitch;
            targetMinDistance = distanceFromTarget;
        }
    }


    private void scaleBreath() {

        float modifierPitchFactor = TimerDeadline * 0.05f;
        
        if(modifierPitchFactor > 1){
            Breath.pitch = modifierPitchFactor;
        }else{
            Breath.pitch = 1;
        }    
        
        float modifierVolumeFactor = TimerDeadline * 0.033f;
        Breath.volume = modifierVolumeFactor;
        //QuestDebug.Instance.Log(String.Format("{0} - {1} - {2}", TimerDeadline, Breath.pitch, Breath.volume));
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

    public bool IsTimeToTriggerEvent(){
         if(stepGameTimer > stepGameDelay)
            return true;
        else 
            return false;
    }


    public void stepGame(MonoBehaviour MyEvent) {
        // QuestDebug.Instance.Log(String.Format("Before {0}", MyEvent.ToString()));
        if(IsTimeToTriggerEvent()){
            TimerDeadline = 0;
            QuestDebug.Instance.Log(String.Format("{0} - {1}", MyEvent.ToString(), stepGameTimer));
            // QuestDebug.Instance.Log(String.Format("Triggering {0}", MyEvent.ToString()));
            switch(MyEvent.ToString()){

                case "SmashLightManager":  
                    QuestDebug.Instance.Log("Destroying Lignt");
                    // Play breaking light 
                    SmashLightSound.Play(0);
                    Destroy(CurrentTarget);
                    // QuestDebug.Instance.Log("Creating next target"); 
                    // createNextTarget(PlayerCamera.centerEyeAnchor.position);
                    // QuestDebug.Instance.Log("Next target created"); 
                    break;
                
                case "TargetManager":
                    countTargetsHit += 1;
                    //QuestDebug.Instance.Log(String.Format("Hit {0} targets", countTargetsHit));
                    Destroy(CurrentTarget);
                    // foreach (var item in guardianBoundariesPoint)
                    // {
                    //     Destroy(item);
                    // }
                    // guardianBoundariesPoint.Clear();
                        // Put here the procedural stuff
                    // spawnDoor();
                    // doorEvent();
                    // createNextTarget(PlayerCamera.centerEyeAnchor.position);
                    // stayStillEvent();
                    // stayDownEvent();
                    break;
                default:
                    QuestDebug.Instance.Log(MyEvent.ToString());
                    break;
            }
            stepGameTimer = 0;
            generateNextEvent(MyEvent.ToString());
       }
    }

    private void generateNextEvent(string prevEventId){
        if(prevEventId == "TargetManager" && countTargetsHit >= targetChangeThreshold){
            countTargetsHit = 0;
            generateNextRandomEvent();
        } else {
            createNextTarget(PlayerCamera.centerEyeAnchor.position);
        }
    }

    private void generateNextRandomEvent(){
        int choice = random.Next(3);
        switch(choice){
            case 0:
                lightEvent();
                break;
            case 1:
                stayStillEvent();
                break;
            case 2:
                stayDownEvent();
                break;
            default:
                QuestDebug.Instance.Log("WTF?!");
                break;
        }
    }

    private void stayDownEvent() {
        playerStillPosition = PlayerCamera.centerEyeAnchor.position;
        isPlayerGoingDown = true; 
        BenignGoingDownAudio.Play(0);
    }

    private void stayStillEvent(){
        playerStillPosition = PlayerCamera.centerEyeAnchor.position;
        isPlayerStill = true;
        BenignStayStillAudio.Play(0);
    }

    private void lightEvent() {
        BenignSmashLight.Play(0);
        Vector3 furthestPoint = getFurthestPoint(PlayerCamera.centerEyeAnchor.position);
        var finalPosition = movePointTowardsPlayer(furthestPoint);
        finalPosition.y = PlayerCamera.centerEyeAnchor.position.y + 0.5f;
        CurrentTarget = Instantiate(SmashLightPrefab, finalPosition, transform.rotation);
        CurrentTarget.transform.Rotate(90, 0, 0);
        // AudioSource ass = CurrentTarget.GetComponent<AudioSource>();
        // SmashLightSound = gameObject.AddComponent<AudioSource>();
        // SmashLightSound.clip = ass.clip;
        // debugDrawForward(PlayerCamera.centerEyeAnchor.position -  new Vector3(0, 2f, 0), SmashLight.transform.position);
    }

    private void doorEvent() {
        Vector3 furthestPoint = getFurthestPoint(PlayerCamera.centerEyeAnchor.position);
        furthestPoint.y = 0;
        var asd = Instantiate(BoundaryPoint, furthestPoint, transform.rotation);
        asd.transform.position += initialPlayerPosition; 
        asd.transform.position += new Vector3(0, 2f, 0);
        debugDrawForward(PlayerCamera.centerEyeAnchor.position -  new Vector3(0, 2f, 0), asd.transform.position);
        var currPlayerDirection =  PlayerCamera.centerEyeAnchor.position;
        var currPlayerRotation =  PlayerCamera.centerEyeAnchor.rotation;
        currPlayerDirection.y = 0;
        // Vector3 direction = PlayerCamera.centerEyeAnchor.position - furthestPoint;
        Vector3 direction = furthestPoint - currPlayerDirection;
        direction.y = 0;
        // Vector3 perpendicularVector = Vector3.Cross(direction, Vector3.up).normalized;
        // var asd2 = Instantiate(BoundaryPoint, perpendicularVector, transform.rotation);
        // asd2.transform.position += initialPlayerPosition; 
        float angleValue = Vector3.Angle(transform.position, direction);
        // Vector3 result = currPlayerDirection + direction.normalized * 1.5f;
        float dir = Vector3.Dot(direction, Vector3.forward);
        QuestDebug.Instance.Log(direction.normalized.ToString());
        Vector3 result = PlayerCamera.centerEyeAnchor.position - PlayerCamera.centerEyeAnchor.forward * 1.5f;
        spawnDoor(result, angleValue);
    }

    private void spawnDoor(Vector3 doorPosition, float rotModifier){
        
        // Vector3 doorPosition =  PlayerCamera.centerEyeAnchor.position - PlayerCamera.centerEyeAnchor.forward * 1.5f; 
        doorPosition.y = 0;
        // var asd = transform;
        // asd.Rotate(new Vector3(0, rotModifier, 0));
        // EvilDoor = Instantiate(EvilDoor, doorPosition, new Quaternion(0, PlayerCamera.centerEyeAnchor.rotation.eulerAngles.y, 0, 0));
        // Quaternion qqq =  Quaternion.LookRotation(PlayerCamera.centerEyeAnchor.forward, Vector3.up);
        // Quaternion qqq1 = new Quaternion(0, qqq.y, 0, 0);
        doorPosition.x = initialPlayerPosition.x;
        doorPosition.z = initialPlayerPosition.z;
        EvilDoor = Instantiate(EvilDoor, initialPlayerPosition, transform.rotation);
        var aaaa = PlayerCamera.centerEyeAnchor;
        // aaaa.rotation = new Quaternion(0, aaaa.rotation.eulerAngles.y, 0, 0 );
        EvilDoor.transform.LookAt(aaaa);
        // QuestDebug.Instance.Log("Door spawned!!");
    }
	
    private void createNextTarget(Vector3 playerPosition){
        Vector3[] guardianBoundaries = boundary.GetGeometry(OVRBoundary.BoundaryType.OuterBoundary);
        QuestDebug.Instance.Log("After guardianBoundaries"); 
        // float maxDistance = -1;
        // float currDistance = -1;
        // Vector3 targetPosition = playerPosition;
        Vector3 targetPosition = guardianBoundaries[random.Next(guardianBoundaries.Length)] + initialPlayerPosition;
        QuestDebug.Instance.Log("After Target Position"); 

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
        var finalPosition = movePointTowardsPlayer(targetPosition);
        CurrentTarget = Instantiate(TargetPrefab, finalPosition, transform.rotation);
        //QuestDebug.Instance.Log("After Instantiate"); 
        targetMinDistance = getDistanceFromTarget(playerPosition, CurrentTarget.transform.position);
        //QuestDebug.Instance.Log("Before return"); 

    }

    private Vector3 getFurthestPoint(Vector3 startPoint) {
        Vector3[] guardianBoundaries = boundary.GetGeometry(OVRBoundary.BoundaryType.OuterBoundary);
        float maxDistance = -1;
        float currDistance = -1;
        Vector3 furthestPosition = startPoint;
        foreach (Vector3 point in guardianBoundaries)
        {
            // var asd = Instantiate(BoundaryPoint, point, transform.rotation);
            // asd.transform.position += initialPlayerPosition; 
            // asd.transform.position += new Vector3(0, 2f, 0);
            // guardianBoundariesPoint.Add(asd);

            currDistance = getDistanceFromTarget(startPoint, point + initialPlayerPosition + new Vector3(0, 2f, 0));
            if (maxDistance < currDistance){
                maxDistance = currDistance;
                furthestPosition = point + initialPlayerPosition;
            }
        }
        return furthestPosition;
    }
    
    private Vector3 movePointTowardsPlayer(Vector3 point){
        point.y = 0;
        var currPlayerPosition = PlayerCamera.centerEyeAnchor.position;
        currPlayerPosition.y = 0;
        Vector3 direction = point - currPlayerPosition;
        var finalPosition = point - direction.normalized * 0.5f;
        return finalPosition;
    }
}
