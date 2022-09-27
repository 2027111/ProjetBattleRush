﻿
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class PlayerAccount
    {

    public string _id;
    public string username;
    public int wins;
    public int losses;
    public int goldcoins;

    public static PlayerAccount connectedUser;
    public static string connectionToken;
    public static void Connected(PlayerAccount returnedAccount, string token)
    {
        if (connectedUser == null && connectionToken == null) {

            connectionToken = token;
            connectedUser = returnedAccount;

            Debug.Log($"{connectedUser.username} - {connectedUser.wins} - {connectedUser.losses} - {connectedUser.goldcoins}");

        }
        else
        {
            Debug.LogError("User alreadyconnected");
        }
    }

    public static void Disconnected()
    {
        if (connectedUser == null)
        {

            Debug.LogError("No user connected");
        }
        else
        {
            connectionToken = null;
            connectedUser = null; 
        }


        SceneManager.LoadScene("LoginScene");
    }


    public static bool IsConnected()
    {
        if(connectedUser == null)
        {
            SceneManager.LoadScene("LoginScene");
        }
        return connectedUser != null;
    }

    public void UpdateParams(PlayerAccount data)
    {
        if(connectedUser.username != data.username)
        {
            Disconnected();
        }
        else
        {
            this.goldcoins = data.goldcoins;
            this.wins = data.wins;
            this.losses = data.losses;
        }
    }
}
