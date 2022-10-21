using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomizeManager : MonoBehaviour
{


    [SerializeField] bool hasSpoiler = true;
    [SerializeField] GameObject displayCar;
    public static Color EmissionColor = Color.white;
    public static Color CarroserieColor = Color.white;
    public static Color RimsColor = Color.white;



    public void ToggleSpoiler()
    {
        hasSpoiler = !hasSpoiler;
    }


    public void OnBodyColorChange(Color c)
    {
        CarroserieColor = c;
        displayCar.GetComponent<CarGraphics>().SetBody(c);
    }

    public void OnAccessoriesColorChange(Color c)
    {
        EmissionColor = c;
        displayCar.GetComponent<CarGraphics>().SetEmissions(c);
        RimsColor = c;
        displayCar.GetComponent<CarGraphics>().SetRims(c);
    }


    public static Vector3 ColorToVector(int returnobject)
    {
        switch (returnobject)
        {
            case 0:
                return new Vector3(CarroserieColor.r, CarroserieColor.g, CarroserieColor.b);

            case 1:
                return new Vector3(EmissionColor.r, EmissionColor.g, EmissionColor.b);

            case 2:
                return new Vector3(RimsColor.r, RimsColor.g, RimsColor.b);

            default:
                return new Vector3(CarroserieColor.r, CarroserieColor.g, CarroserieColor.b);


        }
    }
}
