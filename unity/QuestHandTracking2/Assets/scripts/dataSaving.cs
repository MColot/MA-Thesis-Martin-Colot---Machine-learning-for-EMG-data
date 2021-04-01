﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dataSaving : MonoBehaviour
{

    public OVRSkeleton leftHand;
    public OVRSkeleton rightHand;
    public GameObject savingDisplay;
    public Transform pinchingDisplay;
    public Material savingMaterialOn;
    public Material savingMaterialOff;
    public Material pinchingMaterialOff;
    public Material pinchingMaterialOn;

    string path;
    bool isSaving = false;
    string currentRecordingName = "";
    


    void pingTest()
    {
        string fileName;
        fileName = Application.persistentDataPath + "/Data/pingTest.txt";
        if (System.IO.File.Exists(fileName))
        {
            System.IO.File.WriteAllText(fileName, "test");
        }
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        pingTest();


        testSaving();

        string frameLeftHand = computeFrameDesc(leftHand, leftHand.GetComponent<OVRHand>(), 0);
        string frameRightHand = computeFrameDesc(rightHand, rightHand.GetComponent<OVRHand>(), 4);

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



    private string computeFrameDesc(OVRSkeleton handSkeleton, OVRHand hand, int fingerIndex)
    {
        OVRSkeleton.SkeletonPoseData pose = handSkeleton.getBoneData();

        string text = "";
        text += pose.IsDataValid + ";";
        text += pose.RootPose.ToString() + ";";
        //text += pose.RootScale + ";";
        for(int i=0; i< 19; ++i)
        {
            Quaternion boneRotation =  new Quaternion();
            text += pose.BoneRotations[i].x + "," + -pose.BoneRotations[i].y + "," + -pose.BoneRotations[i].z +","+ pose.BoneRotations[i].w + ";";
        }
        for (int i = 1; i < 5; ++i)
        {
            bool pinching = hand.GetFingerIsPinching((OVRHand.HandFinger)i);
            text += pinching + ";";
            if (pinching)
            {
                pinchingDisplay.GetChild(fingerIndex + i -1).GetComponent<Renderer>().material = pinchingMaterialOn;
            }
            else
            {
                pinchingDisplay.GetChild(fingerIndex + i -1).GetComponent<Renderer>().material = pinchingMaterialOff;
            }
        }
        for (int i = 0; i < 5; ++i)
        {
            text += hand.GetFingerConfidence((OVRHand.HandFinger)i) + ";";
        }

        return text;
    }
}
