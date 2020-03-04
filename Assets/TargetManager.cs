using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetManager : MonoBehaviour
{
    // Start is called before the first frame update

   private Navigator navigator;
   void OnCollisionEnter(Collision collision)
    {
        QuestDebug.Instance.Log("Collision Detected!!");
    }

    private void OnTriggerEnter(Collider other)
    {
        // QuestDebug.Instance.Log("Trigger Detected!!");
        navigator = GameObject.FindGameObjectWithTag("GameMaster").GetComponent<Navigator>();
        navigator.stepGame();
    }
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
