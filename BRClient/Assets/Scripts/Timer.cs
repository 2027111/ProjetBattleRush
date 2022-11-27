using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [SerializeField] Text timerText;
    [SerializeField] Sprite[] available;
    [SerializeField] Image thisImage;

    internal void Amountis(string t)
    {
        timerText.text = t;
        if (t == "01:00")
        {
            thisImage.sprite = available[1];
        }
    }
}
