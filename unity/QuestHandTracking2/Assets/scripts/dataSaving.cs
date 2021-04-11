using System.Collections;
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
    bool sendFeedBack = false;


    // Update is called once per frame (50 Hz)
    void FixedUpdate()
    {
        getRemoteInputs();

        string frameLeftHand = computeFrameDesc(leftHand, leftHand.GetComponent<OVRHand>(), 0);
        string frameRightHand = computeFrameDesc(rightHand, rightHand.GetComponent<OVRHand>(), 4);

        if (isSaving) {
            string time = ((System.DateTime.UtcNow.Ticks - 621355968000000000) / 10000000.0d).ToString();
            System.IO.File.AppendAllText(path, time + ";" + frameLeftHand + frameRightHand + "\n");
        }

        if (sendFeedBack) {
            System.IO.File.AppendAllText(Application.persistentDataPath + "/Data/startRecording.txt", " test");
            sendFeedBack = false;
        }
    }

    //tests if a file with a certain name has been created by a remote control to command an action
    private void getRemoteInputs()
    {
        string fileName;
        //test Start Recording
        fileName = Application.persistentDataPath + "/Data/startRecording.txt";
        if (System.IO.File.Exists(fileName) && !isSaving)
        {
            currentRecordingName = System.IO.File.ReadAllText(fileName);
            startRecording();
            sendFeedBack = true;
        }

        //test Stop Recording
        fileName = Application.persistentDataPath + "/Data/stopRecording.txt";
        if (System.IO.File.Exists(fileName) && isSaving)
        {
            System.IO.File.Delete(fileName);
            stopRecording();
        }
        
    }
    
    //starts the recording, creates a file to put the record in and displays that the recording has started
    private void startRecording()
    {
        isSaving = true;
        savingDisplay.GetComponent<Renderer>().material = savingMaterialOn; //display that the recording has started

        path = Application.persistentDataPath + "/Data/" + currentRecordingName + ".txt";
        System.IO.File.WriteAllText(path, "");
    }


    //stops the recording and displays that the recording has stopped
    private void stopRecording()
    {
        isSaving = false;
        savingDisplay.GetComponent<Renderer>().material = savingMaterialOff; //display that the recording has stopped
    }


    //gets all the informations on the hand gesture and formats it in a string on one line in csv format
    private string computeFrameDesc(OVRSkeleton handSkeleton, OVRHand hand, int fingerIndex)
    {
        OVRSkeleton.SkeletonPoseData pose = handSkeleton.getBoneData();

        string text = "";
        text += (pose.IsDataValid ? 1 : 0) + ";";
        text += pose.RootPose.ToString() + ";";
        for(int i=0; i< 19; ++i)
        {
            Vector3 boneRotation = pose.BoneRotations[i].FromFlippedXQuatf().eulerAngles;
            text += boneRotation.x + "," + boneRotation.y + "," + boneRotation.z + ";";
        }
        for (int i = 1; i < 5; ++i)
        {
            bool pinching = hand.GetFingerIsPinching((OVRHand.HandFinger)i);
            text += (pinching ? 1 : 0) + ";";
            //displays pinching on screen
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
            text += (hand.GetFingerConfidence((OVRHand.HandFinger)i).ToString() == "High" ? 1 : 0) + "; ";
        }

        return text;
    }
}
