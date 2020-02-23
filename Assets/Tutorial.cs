using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    private AudioSource gs_hey_stuck_here;
    private AudioSource gs_have_to_wake_up;
    private AudioSource gs_dont_move;

    private AudioSource dark_slam;
    private int hasPlayed = 0;

    // Start is called before the first frame update
    void Start()
    {
        var aSources = GameObject.Find("GoodSpirit").GetComponents<AudioSource>();
        gs_hey_stuck_here = aSources[0];
        gs_have_to_wake_up = aSources[1];
        gs_dont_move = aSources[2];
    }

    // Update is called once per frame
    void Update()
    {
        switch(hasPlayed){
            case 0: {
                gs_hey_stuck_here.Play();
                hasPlayed = 1;
            }break;
            
            case 1: {
                if(!gs_hey_stuck_here.isPlaying){
                    gs_have_to_wake_up.Play();
                    hasPlayed=2;
                }
            }break;

            // TO FIX, APPARENTLY THIS IS NOT THE RIGHT WAY TO DO THIS.
            /*
            case 2:{
                if(!gs_have_to_wake_up.isPlaying){
                    dark_slam = GameObject.Find("DarkSlam").GetComponent<AudioSource>();
                    dark_slam.Play();
                    hasPlayed=3;   
                }
            }break;

            case 3:{
                if(!dark_slam.isPlaying){
                    gs_dont_move.Play();
                    hasPlayed=4;
                }
            }break;
            */
        }
    }
}
