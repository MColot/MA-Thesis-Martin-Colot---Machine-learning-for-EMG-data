using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class handDataDisplaying : MonoBehaviour
{

    public Transform[] reconstructedHand;
    public string filePath;

    private StreamReader reader;
    public int startRotationIndex = 3;
    private int endRotationIndex;

    private void Start()
    {
        reader = new StreamReader(filePath);
        endRotationIndex = startRotationIndex + 19;
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        string line = reader.ReadLine();
        if(line is null)
        {
            reader = new StreamReader(filePath);
        }
        else
        {
            List<Quaternion> boneRotations = new List<Quaternion>();
            int indexInLine = 0;
            string current = "";
            for(int i=0; i<line.Length; ++i)
            {
                char c = line[i];
                if(c == ';')
                {
                    if(indexInLine >= startRotationIndex && indexInLine < endRotationIndex)
                    {
                        boneRotations.Add(stringToQuaternion(current));
                    }
                    ++indexInLine;
                    current = "";
                }
                else
                {
                    current += c;
                }
            }
            
            for (int i = 0; i < reconstructedHand.Length; ++i)
            {
                reconstructedHand[i].localRotation = boneRotations[i];
            }
        }

    }


    private Quaternion stringToQuaternion(string s)
    {
        s += ",";
        float[] values = new float[4];
        int index = 0;
        string current = "";
        for (int i = 0; i < s.Length; ++i)
        {
            char c = s[i];
            if (c == ',')
            {
                values[index] = float.Parse(current);
                current = "";
                ++index;
            }
            else if(c == '.')
            {
                current += ',';
            }
            else
            {
                current += c;
            }
        }

        return new Quaternion(values[0], values[1], values[2], values[3]);
    }
}
