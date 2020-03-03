using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioInput : MonoBehaviour
{
    public AudioSource audioSource;
    private int audioSampleRate = 44100;
    // public Transform audioSpectrumObjects;
    public static float lerpY = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Started");
        foreach (string device in Microphone.devices)
        {
            Debug.Log("Found Microphone " + device);
        }

        audioSource = GetComponent<AudioSource>();
        string microphone = "";
        audioSource.clip = Microphone.Start(microphone, true, 1, audioSampleRate);
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log("Updating");
        if (audioSource.clip == null)
        {
            Debug.Log("Microphone not working");
        }
        else
        {
            // Debug.Log("clip samples " + audioSource.clip.samples.ToString());
            // Debug.Log("clip channels " + audioSource.clip.channels.ToString());
            float[] samples = new float[audioSource.clip.samples * audioSource.clip.channels];
            audioSource.clip.GetData(samples, 0);

            float mse = 0;
            for (int i = 0; i < samples.Length; ++i)
            {
                mse += samples[i] * samples[i];
            }
            mse = mse / samples.Length;
            // Debug.Log("clip mse " + mse.ToString());
            lerpY = mse * 1e3f;
            // Debug.Log("clip lerpY " + lerpY.ToString());
            // Vector3 newScale = new Vector3(audioSpectrumObjects.localScale.x, lerpY, audioSpectrumObjects.localScale.z);

            // appply new scale to object
            // audioSpectrumObjects.localScale = newScale;
        }
    }
}
