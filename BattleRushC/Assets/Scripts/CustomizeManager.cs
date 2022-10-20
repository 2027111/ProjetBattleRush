using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomizeManager : MonoBehaviour
{



    public static Color carColorMaterial = Color.white;
    [SerializeField] bool hasSpoiler = true;
    [SerializeField] GameObject displayCar;



    public void ToggleSpoiler()
    {
        hasSpoiler = !hasSpoiler;
    }


    public void OnColorChange(Color c)
    {
        carColorMaterial = c;
        GameObject body = displayCar.transform.GetChild(0).gameObject;
        Material m = body.GetComponent<MeshRenderer>().material;
        m.SetColor("_Color", carColorMaterial);
    }
}
