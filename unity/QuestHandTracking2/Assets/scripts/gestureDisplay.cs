using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class gestureDisplay : MonoBehaviour
{
    public int movePerSeq = 5;
    public int seqRep = 5;


    public int phase = 0;

    public GameObject handMvcDisplay;
    public GameObject signLanguageDisplay;
    public GameObject pinchingDisplay;
    public GameObject freeMoveDisplay;
    
    public List<Sprite> signLanguageImages;
    public Sprite signLanguageNeutralImage;
    public GameObject pronationDisplay;
    public GameObject supinationDisplay;
    public TextMesh descriptionDisplay;


    public SpriteRenderer signLanguageSpriteRenderer;
    public GameObject goodResultDisplay;

    private int currentImageId = -1;
    private int imageCounter = 0;
    private bool waitingForGesture = true;


    private void Update()
    {
        //test displayNextPhase
        string fileName = Application.persistentDataPath + "/Data/nextPhase.txt";
        if (System.IO.File.Exists(fileName))
        {
            System.IO.File.Delete(fileName);
            nextPhase();
        }
    }

    public void nextPhase()
    {
        phase = (phase + 1) % 4;
        if(phase == 0)
        {
            freeMoveDisplay.SetActive(false);
            handMvcDisplay.SetActive(true);
        }
        else if (phase == 1)
        {
            handMvcDisplay.SetActive(false);
            signLanguageDisplay.SetActive(true);
            imageCounter = 0;
            updateImage();
            //pronationDisplay.SetActive(true);
            //supinationDisplay.SetActive(false);
        }
        /*
        else if (phase == 2)
        {
            imageCounter = 0;
            updateImage();
            //pronationDisplay.SetActive(false);
            //supinationDisplay.SetActive(true);
        }*/
        else if (phase == 2)
        {
            signLanguageDisplay.SetActive(false);
            pinchingDisplay.SetActive(true);
        }
        else if (phase == 3)
        {
            pinchingDisplay.SetActive(false);
            freeMoveDisplay.SetActive(true);
        }
    }
    

    public void notifyGesture(int imageId)
    {
        if (phase != 1 || !waitingForGesture) return;
        if(imageId == currentImageId)
        {
            waitingForGesture = false;
            goodResultDisplay.SetActive(true);
            Invoke("displayNeutral", 1);
            Invoke("updateImage", 2);
        }
    }

    public void displayNeutral()
    {
        signLanguageSpriteRenderer.sprite = signLanguageNeutralImage;
    }

    public void updateImage()
    {
        goodResultDisplay.SetActive(false);

        int nextImageId = imageToDisplay(imageCounter);
        if (nextImageId >= signLanguageImages.Count) return;
        currentImageId = nextImageId;
        ++imageCounter;

        signLanguageSpriteRenderer.sprite = signLanguageImages[nextImageId];
        waitingForGesture = true;
    }

    int imageToDisplay(int counter)
    {
        int imageGroup = counter / (movePerSeq * seqRep);
        int groupRep = (counter % (movePerSeq * seqRep)) / seqRep;
        int imageId = (counter % (movePerSeq * seqRep)) % seqRep;

        descriptionDisplay.text = "Gesture sequence " + imageGroup.ToString() + ", repetition " + groupRep.ToString() + ", gesture " + (groupRep * imageGroup + imageId).ToString();
        

        return seqRep * imageGroup + imageId;
    }
}
