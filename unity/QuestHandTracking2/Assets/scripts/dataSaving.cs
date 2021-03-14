using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dataSaving : MonoBehaviour
{

    public OVRSkeleton leftHand;
    public OVRSkeleton rightHand;

    string path;

    // Start is called before the first frame update
    void Start()
    {
        path = Application.persistentDataPath + "/Data/test.txt";
        System.IO.File.WriteAllText(path, "");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        string frameLeftHand = computeFrameDesc(leftHand);
        string frameRightHand = computeFrameDesc(rightHand);

        System.IO.File.AppendAllText(path, Time.time + ";" + frameLeftHand + ";" + frameRightHand + "\n");
    }

    private string computeFrameDesc(OVRSkeleton hand)
    {
        OVRSkeleton.SkeletonPoseData pose = hand.getBoneData();
        string text = "";
        text += pose.RootPose.ToString() + ";";
        text += pose.RootScale + ";";
        for(int i=0; i<pose.BoneRotations.Length; ++i)
        {
            text += pose.BoneRotations[i].ToString() + ";";
        }
        text += pose.IsDataValid + ";";
        text += pose.IsDataHighConfidence + ";";
        text += pose.SkeletonChangedCount + ";";

        return text;
    }
}
