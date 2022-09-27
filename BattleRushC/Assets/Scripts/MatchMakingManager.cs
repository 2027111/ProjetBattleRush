using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MatchMakingManager : MonoBehaviour
{


    [SerializeField] GameObject LoadingUI;
    public void StartMatchMaking()
    {
        //Activer le layout de chargement,
        LoadingUI.SetActive(true);
        //Commencer MatchMaking
        StartCoroutine(AttemptConnectToServer());
    }

    IEnumerator AttemptConnectToServer()
    {

        WWWForm form = new WWWForm();
        form.AddField("token", PlayerAccount.connectionToken);
        UnityWebRequest request = UnityWebRequest.Post($"{ServerTalker.mainAddress}queue/join", form);
        var handler = request.SendWebRequest();
        float startTime = 0;
        while (!handler.isDone)
        {
            startTime += Time.deltaTime;
            if (startTime >= 10.0f)
            {
                break;
            }
            yield return null;
        }



        if (request.result == UnityWebRequest.Result.Success)
        {

            ServerResponse response = JsonUtility.FromJson<ServerResponse>(request.downloadHandler.text);

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
        else
        {
            PlayerAccount.Disconnected();
        }
    }
}
