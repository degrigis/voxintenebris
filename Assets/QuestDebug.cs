using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestDebug : MonoBehaviour
{

    public static QuestDebug Instance;

    public bool inMenu;
    // List<string> logEntry;

    Text logText;

    public int threshold = 10;

    void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        var rt = DebugUIBuilder.instance.AddLabel("Debug");
        logText = rt.GetComponent<Text>();
        DebugUIBuilder.instance.Show();
        this.Log("Logging system enabled!!");
    }

    // Update is called once per frame
    void Update()
    {
        if(OVRInput.GetDown(OVRInput.Button.Two) || OVRInput.GetDown(OVRInput.Button.Start))
        {
            if (inMenu) DebugUIBuilder.instance.Hide();
            else DebugUIBuilder.instance.Show();
            inMenu = !inMenu;
        } 
        // if (inMenu) DebugUIBuilder.instance.Hide();
        // else DebugUIBuilder.instance.Show();
    }

    public void Log(string message){
        // if(logEntry.Count > threshold){
        //     logEntry.RemoveAt(0);
        // }
        // logEntry.Add(message);
        // logText.text = String.Join("\n", logEntry);
        logText.text = message;
    }
}
