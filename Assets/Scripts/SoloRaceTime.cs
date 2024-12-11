using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class SoloRaceTime : MonoBehaviour
{

    [SerializeField] public TMP_Text raceTimer;
    // Start is called before the first frame update
    void Start()
    {
        float raceTime = PlayerPrefs.GetFloat("Time");
        TimeSpan timeSpan = TimeSpan.FromSeconds(raceTime);
        string timeText = string.Format("{0:D2}:{1:D2}:{2:D3}", timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);
        raceTimer.text = "Fastest Solo Race: " + timeText.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
