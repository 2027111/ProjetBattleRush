using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomizeManager : MonoBehaviour
{


    [SerializeField] bool hasSpoiler = true;
    [SerializeField] GameObject displayCar;
    public static Color[] Colors = { Color.white, Color.white, Color.white };//Emission, Carroserie, Rims
    public static Action<Color>[] actions = new Action<Color>[3];
    public Slider rSlider;
    public Slider gSlider;
    public Slider bSlider;
    public static Color EmissionColor = Color.white;
    public static Color CarroserieColor = Color.white;
    public static Color RimsColor = Color.white;
    public static int ColorSelected = 0;


    public void SelectPart(int i)
    {
        if(Colors.Length < i)
        {
            return;
        }
        ColorSelected = i;
        rSlider.value = Colors[ColorSelected].r;
        gSlider.value = Colors[ColorSelected].g;
        bSlider.value = Colors[ColorSelected].b;
    }

    private void Start()
    {
        actions[0] = new Action<Color>(OnAccessoriesColorChange);
        actions[1] = new Action<Color>(OnBodyColorChange);
        actions[2] = new Action<Color>(OnRimsColorChange);
        for(int i=0; i < Colors.Length; i++)
        {
            SelectPart(i);
        }
        
    }
    public void ToggleSpoiler()
    {
        hasSpoiler = !hasSpoiler;
    }

    public void OnrSliderChange(float Value)
    {
        Color color = Colors[ColorSelected];
        color.r = Value;
        OnColorChange(color);
    }
    public void OngSliderChange(float Value)
    {
        
        Color color = Colors[ColorSelected];
        color.g = Value;
        OnColorChange(color);
    }
    public void OnbSliderChange(float Value)
    {
        Color color = Colors[ColorSelected];
        color.b = Value;
        OnColorChange(color);
    }

    public void OnColorChange(Color c)
    {
        Colors[ColorSelected] = c;
        actions[ColorSelected](c);
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
    }

    public void OnRimsColorChange(Color c)
    {
        RimsColor = c;
        displayCar.GetComponent<CarGraphics>().SetRims(c);
    }

    public static Vector3 ColorToVector(int returnobject)
    {
        ColorSelected = returnobject;
        return new Vector3(Colors[ColorSelected].r, Colors[ColorSelected].g, Colors[ColorSelected].b);
    }
}
