using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarGraphics : MonoBehaviour
{

    public Vector3 EmissionColor;
    public Vector3 CarroserieColor;
    public Vector3 RimsColor;

    internal void Set(Vector3 colorBody, Vector3 colorEmi, Vector3 colorRims)
    {
        CarroserieColor = colorBody;
        EmissionColor = colorEmi;
        RimsColor = colorRims;

    }
}
