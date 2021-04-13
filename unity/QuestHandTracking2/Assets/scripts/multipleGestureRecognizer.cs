using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class multipleGestureRecognizer : MonoBehaviour
{
    public GestureRecognizer gestureRecognizer;
    public OVRSkeleton leftHand;
    public OVRSkeleton rightHand;
    public TextMesh textDisplay;
    
    private int gestureCount = 0;

    private void Start()
    {
        IList<OVRBone> bonesR = rightHand.Bones;
        for (int i = 0; i < 5; ++i)
        {
            gestureRecognizer.fingers[i] = bonesR[bonesR.Count - 5 + i].Transform.gameObject;
        }
        gestureRecognizer.hand = bonesR[0].Transform.gameObject;
    }


    void Update()
    {
        //test save gesture
        string fileName = Application.persistentDataPath + "/Data/saveGesture.txt";
        if (System.IO.File.Exists(fileName))
        {
            textDisplay.text += "Saving new Gesture \n";
            System.IO.File.Delete(fileName);
            saveCurrentGesture();
        }
    }

    public void saveCurrentGesture()
    {
        try
        {
            gestureRecognizer.SaveAsGestureWithName(gestureCount.ToString());
            gestureRecognizer.savedGestures[gestureCount].onRecognized.AddListener(() => { notifyGesture(); });
            ++gestureCount; 

            textDisplay.text += "New gesture saved \n";
        }
        catch(Exception e)
        {
            textDisplay.text += e + "\n";
        }
    }

    public void notifyGesture()
    {
        textDisplay.text += "Gesture detected : " + gestureRecognizer.gestureDetected.gestureName + "\n";
    }
    
}
