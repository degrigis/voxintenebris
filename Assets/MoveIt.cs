using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveIt : MonoBehaviour
{
 float timeCounter = 0;
  int done = 0;
  int steps = 0;
AudioSource m_MyAudioSource;

//Play the music
bool m_Play;
//Detect when you use the toggle, ensures music isn’t played multiple times
bool m_ToggleChange;

 void Start() {
m_MyAudioSource = GetComponent<AudioSource>();
 m_Play = true;
}


 void Update(){

 Vector3 player = GameObject.Find("Player").transform.position;
  
  int move = Random.Range(0, 600);
  
  timeCounter += Time.deltaTime;
if(move < 5 && done == 0 ){

float x = Mathf.Cos(timeCounter) + 5; // + X regulates the distance!
float y = Mathf.Sin(timeCounter) + 5 ;
float z = 0;

//transform.position = new Vector3(x,y,z);
   transform.position = new Vector3(player.x + Random.Range(-1.0f, 1.0f),  player.y + Random.Range(0f, 20.0f), player.z + Random.Range(-5.0f, 5.0f));

    //Play the audio you attach to the AudioSource component
    m_MyAudioSource.Play();
    //Ensure audio doesn’t play more than once
   done++;
   
}}
}
