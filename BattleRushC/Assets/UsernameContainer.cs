using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UsernameContainer : MonoBehaviour
{

    [SerializeField] Text usernameText;


    public void SetText(string t)
    {
        usernameText.text = t;
    }
}
