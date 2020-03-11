using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetManager : MonoBehaviour
{
    // Start is called before the first frame update

    private Navigator navigator;

    // Light lt;
    // float setIntensity = 0.1f;
   
    /*
    When a collision is detected with a target we call the stepGame of the
    game Master object.
    */
    private void OnTriggerEnter(Collider other)
    {
        // QuestDebug.Instance.Log("Trigger Detected!!");
        navigator = GameObject.FindGameObjectWithTag("GameMaster").GetComponent<Navigator>();
        navigator.stepGame(this);
    }
    void Start()
    {
        // lt = GetComponent<Light>();
        // lt.intensity = setIntensity;
    }

    // Update is called once per frame
    void Update()
    {
        // setIntensity = AudioInput.lerpY * 10f;
        // lt.intensity = setIntensity;
    }

    public override string ToString(){
        return "TargetManager";
    }
}
