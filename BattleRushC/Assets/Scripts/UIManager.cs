using RiptideNetworking;
using RiptideNetworking.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager _singleton;

    public static UIManager Singleton
    {
        get => _singleton;
        private set
        {
            if (_singleton == null)
            {

                _singleton = value;
            }
            else if (_singleton != value)
            {
                Debug.Log($"{nameof(UIManager)} instance already exists. Destroying duplicate!");
                Destroy(value);
            }
        }
    }

    [SerializeField] private MenuPreset[] menus;
    [SerializeField] private Text usernameText;
    [SerializeField] private Text goldAmountText;

    private void Awake()
    {
        Singleton = this;

    }



    private void Start()
    {
        UpdateParameters();
    }




    public void LeaveGame()
    {
        Application.Quit();
    }


    public void TurnOffAll(MenuPreset thisM)
    {
        foreach(MenuPreset mp in menus)
        {

            if(mp != thisM)
            {
                mp.OnHoverExit();
            }
        }
    }

    public void UpdateParameters()
    {

        if (PlayerAccount.IsConnected())
        {
            usernameText.text = PlayerAccount.connectedUser.username;

            goldAmountText.text = PlayerAccount.connectedUser.goldcoins.ToString();
        }
    }
}
