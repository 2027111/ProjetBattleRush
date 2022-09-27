using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;
using RiptideNetworking.Utils;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.SceneManagement;
using System.Net;
public enum ServerToClientId : ushort
{
    playerSpawned = 1,
    playerMovement,
    animations,
    damage,
    startPositions,
    timeTillStart,
}
public enum ClientToServerId : ushort
{
    name = 1,
    input,
}

public enum CurrentServerState
{
    Open,
    InGame,
    Battle,
    Closed,
}
public enum CurrentLobbyType
{
    Casual,
    Ranked,
    Friend,
    Offline,
}

public class NetworkManager : MonoBehaviour
{


    [SerializeField] bool AutomaticStart;
    [SerializeField] private static string mainAddress = "http://127.0.0.1:5500/";


    [Range(63577, 64000)]
    int serverPort;


    private static NetworkManager _singleton;

    public static NetworkManager Singleton
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
                Debug.Log($"{nameof(NetworkManager)} instance already exists. Destroying duplicate!");
                Destroy(value.gameObject);
            }
        }
    }

    public CurrentServerState currentServerState = CurrentServerState.Open;
    public CurrentLobbyType currentLobbyType = CurrentLobbyType.Casual;

    public Server Server { get; private set; }


    [SerializeField] private ushort port;
    [SerializeField] private ushort maxClientCount;

    string ReturnAdress(string host)
    {

        IPHostEntry hostEntry;

        hostEntry = Dns.GetHostEntry(host);

        //you might get more than one ip for a hostname since 
        //DNS supports more than one record

        if (hostEntry.AddressList.Length > 0)
        {
            var ip = hostEntry.AddressList[0];
            return ip.ToString();
        }
        else
        {
            return null;
        }
    }

    public ushort GetClientMax()
    {
        return maxClientCount;
    }

    private void Awake()
    {
        Singleton = this;



    }
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        if (AutomaticStart)
        {
            StartServer();
        }
    }
    #region Server Starting
    public void ProcessServerStart()
    {
        port = (ushort)serverPort;
        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);

        Server = new Server();
        Server.Start(port, maxClientCount);
        Server.ClientConnected += PlayerJoined;
        Server.ClientDisconnected += PlayerLeft;
    }

    private void PlayerJoined(object sender, ServerClientConnectedEventArgs e)
    {
        if (Server.ClientCount == 2)
        {
            currentServerState = CurrentServerState.InGame;
            StartCoroutine(StartBattle());
            UpdateServer();
        }
        else
        {

            StartCoroutine(TryGetServer());
        }
    }

    private IEnumerator StartBattle()
    {
        //Send Who's left and who's right.
        //


        float t = 0;
        while (t < 1)
        {

            t += Time.deltaTime;
            yield return null;

        }




        for (int i = 0; i < 3; i++)
        {
            SendTimeStart(i);
            yield return new WaitForSeconds(1f);

        }


        SendTimeStart(3);
        currentServerState = CurrentServerState.Battle;
    }

    private void SendTimeStart(int threeminusi)
    {
        Message message = Message.Create(MessageSendMode.unreliable, ServerToClientId.timeTillStart);
        message.AddFloat(3 - threeminusi);
        Server.SendToAll(message);
    }




    private void GetFreePort()
    {
        serverPort = Random.Range(63577, 64000);
    }


    private void UpdateServer()
    {
        StartCoroutine(TryUpdateServer());
    }



    private IEnumerator TryUpdateServer()
    {

        WWWForm form = new WWWForm();
        form.AddField("ip", "127.0.0.1");
        form.AddField("port", port);
        form.AddField("maxPlayer", maxClientCount);
        form.AddField("lobbystate", currentServerState.ToString());
        form.AddField("lobbytype", currentLobbyType.ToString());
        form.AddField("connectedPlayer", Server.ClientCount);
        UnityWebRequest request = UnityWebRequest.Post($"{mainAddress}server/update", form);
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
            Debug.Log("Update Successful");
        }
        else
        {
            Debug.Log("Update Failed");
        }
        yield return null;


    }


    private IEnumerator TryGetServer()
    {

        WWWForm form = new WWWForm();
        form.AddField("port", port);
        UnityWebRequest request = UnityWebRequest.Post($"{mainAddress}server", form);
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



            DescResponse response = JsonUtility.FromJson<DescResponse>(request.downloadHandler.text);

            switch (response.code)
            {
                case 0:

                    currentLobbyType = CurrentLobbyType.Casual;
                    //0 means casual;


                    break;


                case 1:
                    //1 means ranked;
                    currentLobbyType = CurrentLobbyType.Ranked;
                    break;


                case 2:
                    //2 means friends
                    currentLobbyType = CurrentLobbyType.Friend;
                    break;

            }

            UpdateServer();
        }
        else
        {
            Debug.Log("Update Failed");
        }
        yield return null;


    }


    public void StartServer()
    {
        if (Server != null)
        {
            return;
        }
        GetFreePort();
        StartCoroutine(TryStartServer());
    }


    IEnumerator TryStartServer()
    {
        WWWForm form = new WWWForm();
        form.AddField("ip", "127.0.0.1");
        form.AddField("port", serverPort);
        form.AddField("maxPlayer", maxClientCount);
        UnityWebRequest request = UnityWebRequest.Post($"{mainAddress}server/On", form);
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

            DescResponse response = JsonUtility.FromJson<DescResponse>(request.downloadHandler.text);
            switch (response.code)
            {
                case 0:
                    ProcessServerStart();
                    break;


                case -1:


                    GetFreePort();
                    break;

                default:
                    GetFreePort();
                    break;

            }


        }
        else
        {
            Debug.Log("Server start failed");
            Application.Quit();
        }
        yield return null;
    }

    IEnumerator TryStopServer()
    {
        WWWForm form = new WWWForm();
        form.AddField("port", port);
        UnityWebRequest request = UnityWebRequest.Post($"{mainAddress}server/Off", form);
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
            ProcessServerStop();
        }
        else
        {
        }
        yield return null;
    }

    private void ProcessServerStop()
    {
        if (Server != null && Server.IsRunning)
        {
            Server.Stop();

        }
    }

    #endregion


    public void StopServer()
    {
        StartCoroutine(TryStopServer());
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (Server != null)
        {
            Server.Tick();

        }



    }
    public void KickPlayer(ushort id)
    {
        Server.DisconnectClient(id);

        if (Player.list.TryGetValue(id, out Player player))
        {
            Destroy(player.gameObject);
        }
    }


    private void OnApplicationQuit()
    {
        StopServer();
    }


    private void PlayerLeft(object sender, ClientDisconnectedEventArgs e)
    {
        if (Player.list.TryGetValue(e.Id, out Player player))
        {
            Destroy(player.gameObject);
        }

            UpdateServer();

    }

    private void RestartServer()
    {

        currentServerState = CurrentServerState.Open;
        currentLobbyType = CurrentLobbyType.Casual;
        UpdateServer();
    }


}