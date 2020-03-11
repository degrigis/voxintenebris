using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Navigator : MonoBehaviour
{
    /*
    Global reference to the PlayerHead.
    We used this to get the current position opf the player.
    */
    private OVRCameraRig PlayerCamera;
    
    /*
    Global reference to the PlayerController component.
    We used this to extract the AudioSource of the heartbeat and 
    the breath.
    */
    private OVRPlayerController PlayerController;

    /*
    Getting the initialPlayer position when the game starts.
    This is used to rebase the LocalPosition of other objects.
    */
    private Vector3 initialPlayerPosition;
    private Vector3 initialPlayerLocalPosition;
    private Vector3 playAreaCenter;
    
    /*
    Global references to objects in the scene.
    */
    // private Light RedLight;
    // private Light RedLight;
    public GameObject TargetPrefab;
    private GameObject CurrentTarget;
    public GameObject BoundaryPoint;
    public GameObject EvilDoor;
    public GameObject SmashLightPrefab;
    public GameObject VictoryTargetPrefab;
    private GameObject EvilEye;
    private Light EyeLight;
    private Light LightUpEye;

    /*
    AudioSources references.
    */
    private AudioSource SmashLightSound;
    private AudioSource BenignTutorial;
    private AudioSource BenignStayStillAudio;
    private AudioSource BenignGoingDownAudio;
    private AudioSource BenignStayDownAudio;
    private AudioSource BenignStandUpAudio;
    private AudioSource BenignSmashLight;
    private AudioSource BenignOneLastStep;
    private AudioSource BenignWakeUpVictory;
    private AudioSource BadSpiritYouDead;
    private AudioSource Heartbeat;
    private AudioSource Breath;

    /*
    Debug stuff, will remove.
    */
    private LineRenderer lineRenderer;

    /*
    Global reference of the initial values of the 
    heartbeat pitch and the light intensity.
    */
    private float initialHeartbeatPitch;
    // private float initialLightIntensity;


    /*
    This is used to keep track of the time that 
    has been passed since last target hit by the player.
    */
    private float TimerDeadline;
    
    /*
    This is used to kill the player
    if he doesn't reach a target in time.
    */
    private float DeadlineDelta = 30;
    
    /*
    Boundary manager object used to get the Guardian's
    boundaries.
    */
    private OVRBoundary boundary;
    
    /*
    How many normal targets you are supposed to reach
    before generating a special event.
    */
    public float targetChangeThreshold = 3;
 
    /*
    The minimal distance from the player and a spawned target.
    This is used to accellerate the hearbeat.
    */
    private float targetMinDistance;

    private System.Random random;

    private List<GameObject> guardianBoundariesPoint;
    private List<GameObject> playAreaBoundariesPoint;

    /*
    Keeps track of how many target we have reached.
    */
    private int countTargetsHit = 0;

    /*
    stepGameTimer and stepGameDelay are 
    used to avoid triggering multiple collisions event.
    */
    private float stepGameTimer;
    private float stepGameDelay = 1f;
    
    private enum Directions {
        LEFT,
        RIGHT,
        STRAIGHT
    }

    /*
    Used when the player has to stay still.
    This is a reference to his position during the 
    "stayStill" event.
    */
    private Vector3 playerStillPosition;
    
    /*
    Boolean that tells us if the player is supposed to 
    stay still.
    */
    private bool isPlayerStill = false;
    
    /*
    Boolean that tells us is the player is moving down.
    */
    private bool isPlayerGoingDown = false;

    private bool isInsideTutorial = true;
    
    /*
    Boolean that tells us if the player is supposed to 
    stay down.
    */
    private bool isPlayerDown = false;

    /*
    Number of special events that the player needs to
    trigger before winning
    */
    public int eventsBeforeVictory = 5;

    /*
    Number of special events triggered so far
    */
    public int specialEventGenerated = 0;

    private float eyeRotationXConstant = 0.50f;
    private float eyeRotationYConstant = -1f;
    
    void Start()
    {
        Debug.Log("Navigator started!");
        stepGameTimer = 0f;
        PlayerCamera = GameObject.FindGameObjectWithTag("casa").GetComponent<OVRCameraRig>();
        PlayerController = GameObject.FindGameObjectWithTag("PlayerController").GetComponent<OVRPlayerController>();
        // RedLight = GameObject.FindGameObjectWithTag("RedLight").GetComponent<Light>();
        lineRenderer = GetComponent<LineRenderer>();
        Heartbeat = PlayerController.GetComponents<AudioSource>()[0];
        Breath = PlayerController.GetComponents<AudioSource>()[1];
        EvilEye = GameObject.FindGameObjectWithTag("Eye");
        EyeLight = GameObject.FindGameObjectWithTag("EyeLight").GetComponent<Light>();
        LightUpEye = GameObject.FindGameObjectWithTag("LightUpEye").GetComponent<Light>();

        // Getting audio sources 
        BenignTutorial = GetComponents<AudioSource>()[9];
        BenignWakeUpVictory = GetComponents<AudioSource>()[8];
        BenignOneLastStep = GetComponents<AudioSource>()[7];
        BenignSmashLight = GetComponents<AudioSource>()[6];
        BadSpiritYouDead = GetComponents<AudioSource>()[5];
        BenignStandUpAudio = GetComponents<AudioSource>()[4];
        BenignStayDownAudio = GetComponents<AudioSource>()[3];
        BenignGoingDownAudio = GetComponents<AudioSource>()[2];
        BenignStayStillAudio = GetComponents<AudioSource>()[1];
        SmashLightSound = GetComponents<AudioSource>()[0];

        // Set initial variables 
        initialHeartbeatPitch = Heartbeat.pitch;
        // initialLightIntensity = RedLight.intensity;
        initialPlayerPosition = PlayerCamera.centerEyeAnchor.position;
        initialPlayerLocalPosition = PlayerCamera.centerEyeAnchor.localPosition;
        boundary = OVRManager.boundary;
        guardianBoundariesPoint = new List<GameObject>();
        playAreaBoundariesPoint = new List<GameObject>();
        random = new System.Random();

        // getGuardianCenter();

        // BenignTutorial.Play(0);
    }

    // Update is called once per frame 
    void Update()
    {
        moveEye();

        if (isInsideTutorial && BenignTutorial.isPlaying){
            return;
        }

        if (isInsideTutorial && !BenignTutorial.isPlaying){
            isInsideTutorial = false;
            //Let's create the next target that the user is supposed to reach.
            //WARNING: the red cubes are for debug only, the player is supposed to find them
            //following the heartbeat.
            // createNextTarget(PlayerCamera.centerEyeAnchor.position);
            stayStillEvent();
        }

        // Updating our timers.
        stepGameTimer += Time.deltaTime;
        TimerDeadline += Time.deltaTime;
        
        // If the player didn't find the target in the 
        // given delta, we have to kill him.
        if(TimerDeadline > DeadlineDelta){
            killPlayer();
        }

        // Handling of the event in which the player is supposed to stay still.
        // If the player is supposed to stay still and the audio is playing 
        // we check its coordinate and if he is moving we kill him.
        if (isPlayerStill && BenignStayStillAudio.isPlaying) { 
            TimerDeadline = 0;
            var currentPosition = PlayerCamera.centerEyeAnchor.position;
            
            // Some delta (0.5f) to prevent unwanted imprecisions and kills.
            if (currentPosition.x > playerStillPosition.x + 0.5 || currentPosition.x < playerStillPosition.x - 0.5 || 
                currentPosition.z > playerStillPosition.z + 0.5 || currentPosition.z < playerStillPosition.z - 0.5){
                    killPlayer();
                }
        }

        /*
        If the player was supposed to stay still and the audio has finished to play
        it means that the player can move again. Let's restore the variables and generate
        the next target.
        */
        if (isPlayerStill && !BenignStayStillAudio.isPlaying) {
            turnOffEye();
            TimerDeadline = 0;
            countTargetsHit = 0;
            isPlayerStill = false;
            createNextTarget(PlayerCamera.centerEyeAnchor.position);
        }

        /*
        This is used in order to give the player time to get down if 
        we have generated a "stayDown" event.
        */
        if (isPlayerGoingDown && !BenignGoingDownAudio.isPlaying) {
            TimerDeadline = 0;
            isPlayerGoingDown = false;
            isPlayerDown = true;
            BenignStayDownAudio.Play(0);
        }

        /*
        If the player was supposed to stay down and the 'BenignStayDownAudio'
        is playing, we have to kill him.
        */
        if (isPlayerDown && BenignStayDownAudio.isPlaying) {
            TimerDeadline = 0;
            var currentPosition = PlayerCamera.centerEyeAnchor.position;

            // Some delta to prevent imprecision and unwanted kills (0.5f, again)
            if (currentPosition.y > playerStillPosition.y - 0.5) {
                killPlayer();
            }
        }

        /*
        If the player was supposed to stay down and the audio has finished to play
        it means that the player can get up. Let's restore the variables and generate
        the next target.
        */
        if (isPlayerDown && !BenignStayDownAudio.isPlaying) {
            TimerDeadline = 0;
            turnOffEye();
            BenignStandUpAudio.Play(0);
            countTargetsHit = 0;
            isPlayerDown = false;
            createNextTarget(PlayerCamera.centerEyeAnchor.position);
        }

        /*
        Scale heartbeat and light considering the distance of the player from
        the target he is supposed to reach
        */
        if(CurrentTarget != null) {
            Directions dir = getleftOrRight(PlayerCamera.centerEyeAnchor.forward, CurrentTarget.transform.position, PlayerCamera.centerEyeAnchor.up);
            float cur_distance = getDistanceFromTarget(PlayerCamera.centerEyeAnchor.position, CurrentTarget.transform.position);
            scaleHeartbeat(cur_distance);
            // scaleLight(cur_distance);
        }

        // Finally, let's scale the breath according to the time passed
        // since last time the player found a target.
        scaleBreath();
      
    }

    private void moveEye() {
        // if (EvilEye.transform.rotation.eulerAngles.x < 310 || EvilEye.transform.eulerAngles.x > 330) {
        //     eyeRotationXConstant = - eyeRotationXConstant;
        // }
        if (EvilEye.transform.rotation.y < -0.2  || EvilEye.transform.rotation.y > 0.2) {
            eyeRotationYConstant = - eyeRotationYConstant;
        }
        EvilEye.transform.Rotate(Vector3.up * eyeRotationYConstant * Time.deltaTime * 20); 
        //EvilEye.transform.Rotate(Vector3.up * 50.0f * Time.deltaTime); 
        //EvilEye.transform.LookAt(PlayerCamera.centerEyeAnchor.position);
    }

    /*
    Debug stuff..
    */
    private void debugDrawForward(Vector3 start, Vector3 end){
        lineRenderer.SetVertexCount(2);
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
    }

    /*
    Method that kills the player
    */
    private void killPlayer(){        
        SceneManager.LoadScene("GameOver");
        //BadSpiritYouDead.Play();
        //QuestDebug.Instance.Log("You dead!");
    }

    /*
    Helper to understand if the target is left or right of the current player position.
    We can use this to play more sounds if we want. (for instance benign spirit voice)
    */
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

    /*
    Helper to get the distance from two points.
    */
    private float getDistanceFromTarget(Vector3 start, Vector3 end){
        return Vector3.Distance(start, end);
    }

    /*
    Scaling the hearbeat according to the distance from target
    passed as parameter.
    */
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

    /*
    Scaling the breath
    */
    private void scaleBreath() {

        float modifierPitchFactor = TimerDeadline * 0.03f;
        
        if(modifierPitchFactor > 1){
            Breath.pitch = modifierPitchFactor;
        } else if (modifierPitchFactor > 1.1f){
            Breath.pitch = 1.1f;
        }
        else {
            Breath.pitch = 1;
        }    
        
        float modifierVolumeFactor = TimerDeadline * 0.033f;
        if (modifierVolumeFactor > 0.5) {
            Breath.volume = 0.5f;
        } else {
            Breath.volume = modifierVolumeFactor;
        }
        //QuestDebug.Instance.Log(String.Format("{0} - {1} - {2}", TimerDeadline, Breath.pitch, Breath.volume));
    }

    /*
    Scaling the global light. (do we want this?)
    */
    // private void scaleLight(float distanceFromTarget) {
    //     float modifierFactor = distanceFromTarget - targetMinDistance;
    //     //I'm going away from the target
    //     if (modifierFactor > 0) {
    //         RedLight.intensity = initialLightIntensity - modifierFactor * 20;
    //     } 
    //     // We are going towards the target so we have to update mindistance
    //     else {
    //         RedLight.intensity = initialLightIntensity;
    //     }
    // }

    /*
    Helper method that tells us if we need to trigger an answer to a collision
    or not.
    */
    public bool IsTimeToTriggerEvent(){
         if(stepGameTimer > stepGameDelay)
            return true;
        else 
            return false;
    }

    /*
    This is the callback to collisions detections triggered inside
    the objects SmashLightManager and TargetManager.
    */
    public void stepGame(MonoBehaviour MyEvent) {

        // Before let's check if we want to trigger a new event.
        if(IsTimeToTriggerEvent()){
            TimerDeadline = 0;

            // Which event did we receive? 
            switch(MyEvent.ToString()){
                
                case "SmashLightManager":  
                    //QuestDebug.Instance.Log("Destroying Lignt");
                    
                    // Play breaking light 
                    SmashLightSound.Play(0);
                    Destroy(CurrentTarget);
                    break;
                
                case "TargetManager":
                    countTargetsHit += 1;
                    Destroy(CurrentTarget);
                    break;

                case "VictoryTargetManager":
                    Destroy(CurrentTarget);
                    SceneManager.LoadScene("Victory");
                    break;
                
                default:
                    //QuestDebug.Instance.Log(MyEvent.ToString());
                    break;
            }

            stepGameTimer = 0;
            generateNextEvent(MyEvent.ToString());
       }
    }

    /*
    This is used to let the player walk around to find at least X targets.
    X is set to 3 as for now.
    */
    private void generateNextEvent(string prevEventId){
        if(prevEventId == "TargetManager" && countTargetsHit >= targetChangeThreshold){
            countTargetsHit = 0;
            generateNextRandomEvent();
        } else {
            createNextTarget(PlayerCamera.centerEyeAnchor.position);
        }
    }

    /*
    Procedurally generating a special event.
    As for now they all have same probability.
    */
    private void generateNextRandomEvent(){
        if (specialEventGenerated == eventsBeforeVictory){
            victoryEvent();
        } else {
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
            specialEventGenerated += 1;
        }
    }

    /*
    Here we generate the last target (VictoryTarget) that the player has
    to reach in order to win the game.
    */
    private void victoryEvent(){
        Vector3 furthestPoint = getFurthestPoint(PlayerCamera.centerEyeAnchor.position);
        var finalPosition = movePointTowardsPlayer(furthestPoint);
        CurrentTarget = Instantiate(VictoryTargetPrefab, finalPosition, transform.rotation);
        Directions dir = getleftOrRight(PlayerCamera.centerEyeAnchor.forward, CurrentTarget.transform.position, PlayerCamera.centerEyeAnchor.up);
        // Redirect the audio based on the target positon with respect to the player position
        switch (dir) {
            case Directions.LEFT:
                BenignOneLastStep.panStereo = -1;
                break;
            case Directions.RIGHT:
                BenignOneLastStep.panStereo = 1;
                break;
            default:
                BenignOneLastStep.panStereo = 0;
                break;
        }
        BenignOneLastStep.Play(0);
    }

    /*
    Here we force the player to get down to avoid to be killed.
    */
    private void stayDownEvent() {

        //initialize player position.
        playerStillPosition = PlayerCamera.centerEyeAnchor.position;
        isPlayerGoingDown = true; 
      
        // Benign spirit is telling the player to go down!
        BenignGoingDownAudio.Play(0);
        turnOnEye();
    }

    private void turnOnEye(){
        EyeLight.intensity = 30;
        LightUpEye.intensity = 30;
    }

    private void turnOffEye(){
        EyeLight.intensity = 0;
        LightUpEye.intensity = 0;
    }

    /*
    Here we force the player to stay still to avoid to be killed.
    */
    private void stayStillEvent(){
        playerStillPosition = PlayerCamera.centerEyeAnchor.position;
        isPlayerStill = true;
        BenignStayStillAudio.Play(0);
        turnOnEye();
    }

    /*
    Let's create a light that the player has to destroy to avoid to die.
    */
    private void lightEvent() {
        // The spirit is telling the player to smash the light!
        BenignSmashLight.Play(0);

        // Let's create the light at the Furthest point from the current position.
        Vector3 furthestPoint = getFurthestPoint(PlayerCamera.centerEyeAnchor.position);

        // We want to be sure that the light object is within the boundaries of the 
        // guardian, so we scale it a bit using the helper method.
        var finalPosition = movePointTowardsPlayer(furthestPoint);

        // Set the height of the light's object to be above the player.
        finalPosition.y = PlayerCamera.centerEyeAnchor.position.y + 0.5f;
        
        // FInally instantiate the light!
        CurrentTarget = Instantiate(SmashLightPrefab, finalPosition, transform.rotation);
        CurrentTarget.transform.Rotate(90, 0, 0);
    }

    /*
    Trying to instantiate a fucking door rotate in the correct position, but FAILED.
    TODO: Try to make this work!
    */
    // private void doorEvent() {
    //     Vector3 furthestPoint = getFurthestPoint(PlayerCamera.centerEyeAnchor.position);
    //     furthestPoint.y = 0;
    //     var asd = Instantiate(BoundaryPoint, furthestPoint, transform.rotation);
    //     asd.transform.position += initialPlayerPosition; 
    //     asd.transform.position += new Vector3(0, 2f, 0);
    //     debugDrawForward(PlayerCamera.centerEyeAnchor.position -  new Vector3(0, 2f, 0), asd.transform.position);
    //     var currPlayerDirection =  PlayerCamera.centerEyeAnchor.position;
    //     var currPlayerRotation =  PlayerCamera.centerEyeAnchor.rotation;
    //     currPlayerDirection.y = 0;
    //     // Vector3 direction = PlayerCamera.centerEyeAnchor.position - furthestPoint;
    //     Vector3 direction = furthestPoint - currPlayerDirection;
    //     direction.y = 0;
    //     // Vector3 perpendicularVector = Vector3.Cross(direction, Vector3.up).normalized;
    //     // var asd2 = Instantiate(BoundaryPoint, perpendicularVector, transform.rotation);
    //     // asd2.transform.position += initialPlayerPosition; 
    //     float angleValue = Vector3.Angle(transform.position, direction);
    //     // Vector3 result = currPlayerDirection + direction.normalized * 1.5f;
    //     float dir = Vector3.Dot(direction, Vector3.forward);
    //     QuestDebug.Instance.Log(direction.normalized.ToString());
    //     Vector3 result = PlayerCamera.centerEyeAnchor.position - PlayerCamera.centerEyeAnchor.forward * 1.5f;
    //     spawnDoor(result, angleValue);
    // }

    // private void spawnDoor(Vector3 doorPosition, float rotModifier){
    private void spawnDoor(){
        
        // // Vector3 doorPosition =  PlayerCamera.centerEyeAnchor.position - PlayerCamera.centerEyeAnchor.forward * 1.5f; 
        // doorPosition.y = 0;
        // // var asd = transform;
        // // asd.Rotate(new Vector3(0, rotModifier, 0));
        // // EvilDoor = Instantiate(EvilDoor, doorPosition, new Quaternion(0, PlayerCamera.centerEyeAnchor.rotation.eulerAngles.y, 0, 0));
        // // Quaternion qqq =  Quaternion.LookRotation(PlayerCamera.centerEyeAnchor.forward, Vector3.up);
        // // Quaternion qqq1 = new Quaternion(0, qqq.y, 0, 0);
        // doorPosition.x = initialPlayerPosition.x;
        // doorPosition.z = initialPlayerPosition.z;
        // EvilDoor = Instantiate(EvilDoor, initialPlayerPosition, transform.rotation);
        // var aaaa = PlayerCamera.centerEyeAnchor;
        // // aaaa.rotation = new Quaternion(0, aaaa.rotation.eulerAngles.y, 0, 0 );
        // EvilDoor.transform.LookAt(aaaa);
        // // QuestDebug.Instance.Log("Door spawned!!");
        // Vector3 playerPos = PlayerCamera.transform.position;
        // Vector3 playerDirection = PlayerCamera.transform.forward;
        // Quaternion playerRotation = PlayerCamera.transform.rotation;
        // float spawnDistance = 1;
        // Vector3 spawnPos = playerPos + playerDirection * spawnDistance;
        // spawnPos.y = 0;
        // var finalPosition = movePointTowardsPlayer(spawnPos, 3f);
        GameObject closesPlayAreaPoint = getClosestPoint(PlayerCamera.centerEyeAnchor.position);
        var asd = Instantiate(BoundaryPoint, playAreaCenter, transform.rotation);
        asd.transform.LookAt(closesPlayAreaPoint.transform);
        // var bbb = new Vector3(asd.transform.eulerAngles.x, asd.transform.eulerAngles.x, asd.transform.eulerAngles.z);
        // Destroy(asd);
        var door = Instantiate(EvilDoor, playAreaCenter, asd.transform.rotation);
        // door.transform.LookAt(closesPlayAreaPoint.transform);

    }
	

    /*
    Create the next target that the user is supposed to reach.
    We want to pick a random point on the Guardian's boundaries.
    */
    private void createNextTarget(Vector3 playerPosition){

        // Let's pick all the points beloning to the current guardian's boundaries.
        Vector3[] guardianBoundaries = boundary.GetGeometry(OVRBoundary.BoundaryType.OuterBoundary);
        
        // Let's pick one of those points and rebase it according to player position.
        Vector3 targetPosition = guardianBoundaries[random.Next(guardianBoundaries.Length)] + initialPlayerPosition;
        
        // Let's scale the position a bit to make sure it is inside the play area.
        var finalPosition = movePointTowardsPlayer(targetPosition);

        // Finally create the Target!
        CurrentTarget = Instantiate(TargetPrefab, finalPosition, transform.rotation);

        // Update the min distance of the player to the target, this is used to scale the 
        // heartbeat.
        targetMinDistance = getDistanceFromTarget(playerPosition, CurrentTarget.transform.position);
    }

    /*
    Given a starting point we want to return the furthest point 
    from there to the guardian's boundaries.
    */
    private Vector3 getFurthestPoint(Vector3 startPoint) {
        Vector3[] guardianBoundaries = boundary.GetGeometry(OVRBoundary.BoundaryType.OuterBoundary);
        float maxDistance = -1;
        float currDistance = -1;
        Vector3 furthestPosition = startPoint;
        foreach (Vector3 point in guardianBoundaries)
        {
            currDistance = getDistanceFromTarget(startPoint, point + initialPlayerPosition + new Vector3(0, 2f, 0));
            if (maxDistance < currDistance){
                maxDistance = currDistance;
                furthestPosition = point + initialPlayerPosition;
            }
        }
        return furthestPosition;
    }
    private GameObject getClosestPoint(Vector3 startPoint) {
        float minDistance = float.MaxValue;
        float currDistance = float.MaxValue;
        GameObject closestPosition = null;
        foreach (GameObject playAreaPoint in playAreaBoundariesPoint)
        {
            currDistance = getDistanceFromTarget(startPoint, playAreaPoint.transform.position + initialPlayerPosition + new Vector3(0, 2f, 0));
            if (minDistance > currDistance){
                minDistance = currDistance;
                closestPosition = playAreaPoint;
            }
        }
        return closestPosition;
    }
    
    /*
    Helper method used to scale the position of an object toward the player
    position.
    */
    private Vector3 movePointTowardsPlayer(Vector3 point){
        point.y = 0;
        var currPlayerPosition = PlayerCamera.centerEyeAnchor.position;
        currPlayerPosition.y = 0;
        Vector3 direction = point - currPlayerPosition;
        var finalPosition = point - direction.normalized * 0.5f;
        return finalPosition;
    }
    private Vector3 movePointTowardsPlayer(Vector3 point, float factor){
        point.y = 0;
        var currPlayerPosition = PlayerCamera.centerEyeAnchor.position;
        currPlayerPosition.y = 0;
        Vector3 direction = point - currPlayerPosition;
        var finalPosition = point - direction.normalized * factor;
        return finalPosition;
    }
    
    private void getGuardianCenter(){
        Vector3[] guardianBoundaries = boundary.GetGeometry(OVRBoundary.BoundaryType.PlayArea);
        for (int i = 0; i < guardianBoundaries.Length; i++)
        {
            Vector3 point = guardianBoundaries[i];
            var asd = Instantiate(BoundaryPoint, point, transform.rotation);
            asd.transform.position += initialPlayerPosition; 
            asd.transform.position += new Vector3(0, 2f, 0);
            playAreaBoundariesPoint.Add(asd);
        }
        var start = guardianBoundaries[0];
        var end = guardianBoundaries[2];
        start.y = 0;
        end.y = 0;
        playAreaCenter = (((start + end + 2*initialPlayerPosition)) * 0.5f);
        playAreaCenter.y = 0;
        // center.y = 2;
        // Instantiate(BoundaryPoint, center, transform.rotation);
    }
}
