﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToHellManager : MonoBehaviour
{

    private Light doorLight;
    private float fadeFactor = 0.07f;
    private bool canFade = false;

    private void OnTriggerEnter(Collider other)
    {
        canFade = true;
    }

    // Start is called before the first frame update
    void Start()
    {
       doorLight = GameObject.FindGameObjectWithTag("DoorLight").GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
       if (canFade) {
           doorLight.intensity -= fadeFactor;
       } 

       if (doorLight.intensity <= 0){
           SceneManager.LoadScene("SampleScene");
       }
    }
}
