﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class timeDisplay : MonoBehaviour
{
    public TextMesh textDisplay;
    public sineWaveExample sound;
    

    // Update is called once per frame
    void Update()
    {
        textDisplay.text = System.DateTime.UtcNow.ToLocalTime().ToString() + "." + System.DateTime.UtcNow.ToLocalTime().Millisecond.ToString();
    }
}
