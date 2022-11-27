using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionMenuDebug : MonoBehaviour
{

    public static ConnectionMenuDebug singleton;

    private void Start()
    {
        singleton = this;
    }


    public void OnReadyClick()
    {
        if (!NetworkManager.Singleton.Client.IsConnected)
        {
            return;
        }

        NetworkManager.Singleton.SendReady();



    }
}
