using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomizeManager : MonoBehaviour
{



    [SerializeField] Color carColorMaterial = Color.white;
    [SerializeField] bool hasSpoiler = true;
    [SerializeField] GameObject displayCar;



    public void ToggleSpoiler()
    {
        hasSpoiler = !hasSpoiler;
    }


    public void OnColorChange(Color c)
    {
        carColorMaterial = c;
        displayCar.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material.SetColor(0, carColorMaterial);
    }
}
