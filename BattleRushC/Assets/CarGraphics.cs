using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarGraphics : MonoBehaviour
{

    [SerializeField] GameObject Body;
    [SerializeField] GameObject Spoiler;
    [SerializeField] GameObject[] Wheels;

    public void SetRims(Color c)
    {
        foreach(GameObject wheel in Wheels)
        {

            Material m = wheel.GetComponent<MeshRenderer>().materials[0];
            m.SetColor("_Color", c);
        }
    }
    public void SetEmissions(Color c)
    {
        Material m = Body.GetComponent<MeshRenderer>().materials[1];
        m.SetColor("_Color", c);
    }


    public void SetBody(Color c)
    {
        Material m = Body.GetComponent<MeshRenderer>().materials[0];
        m.SetColor("_Color", c);
    }
}
