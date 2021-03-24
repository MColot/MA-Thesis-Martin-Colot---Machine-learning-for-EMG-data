﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dataSaving : MonoBehaviour
{

    public OVRSkeleton leftHand;
    public OVRSkeleton rightHand;
    public GameObject savingDisplay;
    public Material savingMaterialOn;
    public Material savingMaterialOff;

    string path;
    bool isSaving = false;
    string currentRecordingName = "";
    

    // Update is called once per frame
    void FixedUpdate()
    {
        testSaving();

        string frameLeftHand = computeFrameDesc(leftHand, leftHand.GetComponent<OVRHand>());
        string frameRightHand = computeFrameDesc(rightHand, leftHand.GetComponent<OVRHand>());

        if (isSaving)
            System.IO.File.AppendAllText(path, Time.time + ";" + frameLeftHand + frameRightHand + "\n");
    }

    private void testSaving()
    {
        string fileName;
        //test Start Recording
        fileName = Application.persistentDataPath + "/Data/startRecording.txt";
        if (System.IO.File.Exists(fileName)){
            if (isSaving) stopRecording();
            currentRecordingName = System.IO.File.ReadAllText(fileName);
            System.IO.File.Delete(fileName);
            startRecording();
        }

        //test Stop Recording
        fileName = Application.persistentDataPath + "/Data/stopRecording.txt";
        if (System.IO.File.Exists(fileName))
        {
            System.IO.File.Delete(fileName);
            stopRecording();
        }
    }
    
    private void startRecording()
    {
        isSaving = true;
        savingDisplay.GetComponent<Renderer>().material = savingMaterialOn; //display that the recording has started

        path = Application.persistentDataPath + "/Data/" + currentRecordingName + ".txt";
        System.IO.File.WriteAllText(path, "");
    }

    private void stopRecording()
    {
        isSaving = false;
        savingDisplay.GetComponent<Renderer>().material = savingMaterialOff; //display that the recording has stopped
    }



    private string computeFrameDesc(OVRSkeleton handSkeleton, OVRHand hand)
    {
        OVRSkeleton.SkeletonPoseData pose = handSkeleton.getBoneData();

        string text = "";
        text += pose.IsDataValid + ";";
        text += pose.RootPose.ToString() + ";";
        //text += pose.RootScale + ";";
        for(int i=0; i< 19; ++i)
        {
            text += pose.BoneRotations[i].ToString() + ";";
        }
        for (int i = 1; i < 5; ++i)
        {
            text += hand.GetFingerIsPinching((OVRHand.HandFinger)i) + ";";
        }
        for (int i = 0; i < 5; ++i)
        {
            text += hand.GetFingerConfidence((OVRHand.HandFinger)i) + ";";
        }

        return text;
    }
}
