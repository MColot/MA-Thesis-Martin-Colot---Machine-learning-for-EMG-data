using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class gestureDisplay : MonoBehaviour
{
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

    private int repetitions = 0;
    private List<int> gesturesToPerform;


    private void Update()
    {
        //test display MVC
        string fileName = Application.persistentDataPath + "/Data/mvc.txt";
        if (System.IO.File.Exists(fileName))
        {
            System.IO.File.Delete(fileName);
            displayMVC();
        }

        //test display sign language
        fileName = Application.persistentDataPath + "/Data/signLang.txt";
        if (System.IO.File.Exists(fileName))
        {
            string param = System.IO.File.ReadAllText(fileName);
            parseParamSignLang(param);
            System.IO.File.Delete(fileName);
            displaysignLanguage();
        }

        //test display free moves
        fileName = Application.persistentDataPath + "/Data/freemove.txt";
        if (System.IO.File.Exists(fileName))
        {
            System.IO.File.Delete(fileName);
            displayFreeMove();
        }
    }

    private void displayMVC()
    {
        phase = 0;
        freeMoveDisplay.SetActive(false);
        signLanguageDisplay.SetActive(false);
        handMvcDisplay.SetActive(true);
    }

    private void displaysignLanguage()
    {
        phase = 1;
        freeMoveDisplay.SetActive(false);
        signLanguageDisplay.SetActive(true);
        handMvcDisplay.SetActive(false);
        updateImage();
    }

    private void displayFreeMove()
    {
        phase = 2;
        freeMoveDisplay.SetActive(true);
        signLanguageDisplay.SetActive(false);
        handMvcDisplay.SetActive(false);
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
        if (imageCounter >= repetitions * gesturesToPerform.Count) return;
        currentImageId = nextImageId;
        ++imageCounter;

        signLanguageSpriteRenderer.sprite = signLanguageImages[nextImageId];
        waitingForGesture = true;
    }

    int imageToDisplay(int counter)
    {

        int imageId = gesturesToPerform[counter % gesturesToPerform.Count];

        descriptionDisplay.text = "Repetition " +  (1+ (counter / gesturesToPerform.Count)).ToString() + "/" + repetitions.ToString() + ", gesture " + imageId.ToString();
        
        return imageId;
    }


    void parseParamSignLang(string p)
    {
        currentImageId = -1;
        imageCounter = 0;
        repetitions = 0;
        gesturesToPerform = new List<int>();

        string curr = "";
        char c = p[0];
        int i = 0;
        while(c != '\n')
        {
            curr += c;
            ++i;
            c = p[i];
        }
        repetitions = int.Parse(curr);
        curr = "";
        while(i < p.Length-1)
        {
            ++i;
            c = p[i];
            if (c != ' ') curr += c;
            else
            {
                gesturesToPerform.Add(int.Parse(curr));
                curr = "";
            }
        }
        gesturesToPerform.Add(int.Parse(curr));
    }
}
