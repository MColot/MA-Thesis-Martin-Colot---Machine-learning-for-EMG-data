using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gestureDisplay : MonoBehaviour
{
    public List<string> gestureNames;
    public List<Sprite> gestureImages;

    public SpriteRenderer imageDisplay;
    public GameObject goodResultDisplay;

    private string currentImage = "";


    // Start is called before the first frame update
    void Start()
    {
        updateImage();
    }
    

    public void notifyGesture(string imageName)
    {
        if(imageName == currentImage)
        {
            goodResultDisplay.SetActive(true);
            Invoke("updateImage", 1);
        }
    }

    public void updateImage()
    {
        goodResultDisplay.SetActive(false);

        int nextImageId = Random.Range(0, gestureNames.Count);

        imageDisplay.sprite = gestureImages[nextImageId];
        currentImage = gestureNames[nextImageId];
    }
}
