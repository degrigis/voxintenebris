using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaIt : MonoBehaviour
{
int done_play = 0;
AudioSource m_MyAudioSource;
    // Start is called before the first frame update
    void Start()
    {
        m_MyAudioSource = GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update()
    {
if(done_play == 0){
         int maybe_play = Random.Range(0, 600);
if(maybe_play == 300){
	m_MyAudioSource.Play();
	done_play  = 1;
 }
    }
 }
}
