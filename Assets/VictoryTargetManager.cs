using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryTargetManager : MonoBehaviour
{
    private Navigator navigator;
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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override string ToString(){
        return "VictoryTargetManager";
    }
}
