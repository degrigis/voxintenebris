using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightControl : MonoBehaviour
{
    Light lt;
    float setIntensity = 0.1f;

    // float duration = 1.0f;
    //    Color color0 = Color.red;
    //    Color color1 = Color.blue;

    // Start is called before the first frame update
    void Start()
    {
        lt = GetComponent<Light>();
        lt.intensity = setIntensity;
        Debug.Log("init intensity " + lt.intensity);
    }

    // Update is called once per frame
    void Update()
    {
        setIntensity = AudioInput.lerpY;
        lt.intensity = setIntensity;
        Debug.Log("updt intensity " + lt.intensity);

        // float t = Mathf.PingPong(Time.time, duration) / duration;
        // lt.color = Color.Lerp(color0, color1, t);
        // Debug.Log("updt intensity "+lt.color);
    }
}
