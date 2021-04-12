using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ballButton : MonoBehaviour
{
    public GameObject ballPrefab;
    public Vector3 spawnPos;
    public Vector3 releasedPos;
    public Vector3 pressedPos;
    public Transform display;

    private bool pressed = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != 9 && !pressed)
        {
            display.localPosition = pressedPos;
            GameObject newBall = Instantiate(ballPrefab);
            newBall.transform.position = spawnPos;
            pressed = true;
            Invoke("release", 1);
        }
    }


    private void release()
    {
        if (pressed)
        {
            display.localPosition = releasedPos;
            pressed = false;
        }
    }
}
