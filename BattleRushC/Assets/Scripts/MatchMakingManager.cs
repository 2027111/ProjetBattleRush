using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MatchMakingManager : MonoBehaviour
{



    [SerializeField] GameObject LoadingUI;
    
    
    public void DebugConnectToDefaultServer()
    {
        NetworkManager.Singleton.ConnectTo("127.0.0.1", "63577");
    }

    public void StartMatchMaking()
    {
        //Activer le layout de chargement,
        LoadingUI.SetActive(true);
        //Commencer MatchMaking
        //StartCoroutine(AttemptConnectToServer());
        Action<ServerResponse> Success = new Action<ServerResponse>(MatchMakingSuccess);
        Action Failure = new Action(delegate{ LoadingUI.SetActive(false); });
        WWWForm form = new WWWForm();
        form.AddField("token", PlayerAccount.connectionToken);
        string link = "queue/join";
        StartCoroutine(ServerTalker.PostRequestToMasterServer<ServerResponse>(link, form, Success, Failure));

    }

    void MatchMakingSuccess(ServerResponse response)
    {
        switch (response.code)
        {
            case 0:
                Server s = response.data;
                Debug.Log(s);
                NetworkManager.Singleton.ConnectTo(s);
                break;

            case -9:
                PlayerAccount.Disconnected();
                break;
            default:
                PlayerAccount.Disconnected();
                break;
        }

    }
 }
