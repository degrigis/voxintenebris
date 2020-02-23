using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plot : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
         AudioSource audioSource = gameObject.AddComponent<AudioSource>();
         audioSource.clip = Resources.Load(name) as AudioClip;
         audioSource.Play();
            
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
