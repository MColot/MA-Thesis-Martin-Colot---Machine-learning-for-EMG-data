using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dataSaving : MonoBehaviour
{

    public OVRSkeleton leftHand;
    public OVRSkeleton rightHand;
    public Transform[] reconstructedHand;
    public GameObject savingDisplay;
    public Material savingMaterialOn;
    public Material savingMaterialOff;

    string path;
    bool isSaving = false;

    // Start is called before the first frame update
    void Start()
    {
        path = Application.persistentDataPath + "/Data/test.txt";
        System.IO.File.WriteAllText(path, "");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        testSaving();

        string frameLeftHand = computeFrameDesc(leftHand);
        string frameRightHand = computeFrameDesc(rightHand);

        if (isSaving)
            System.IO.File.AppendAllText(path, Time.time + ";" + frameLeftHand + ";" + frameRightHand + "\n");
    }

    private void testSaving()
    {
        string File = Application.persistentDataPath + "/Data/toggleSave.txt";
        if (System.IO.File.Exists(File)){
            System.IO.File.Delete(File);
            isSaving = !isSaving;
            if (isSaving) savingDisplay.GetComponent<Renderer>().material = savingMaterialOn;
            else savingDisplay.GetComponent<Renderer>().material = savingMaterialOff;
        }
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

            if (i < reconstructedHand.Length)
            {
                reconstructedHand[i].localRotation = OVRExtensions.FromQuatf(pose.BoneRotations[i]);
            }
        }
        text += pose.IsDataValid + ";";
        text += pose.IsDataHighConfidence + ";";
        text += pose.SkeletonChangedCount + ";";

        return text;
    }
}
