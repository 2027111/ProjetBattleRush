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
using System;

public enum ServerToClientId : ushort
{
    sync = 1,
    playerSpawned,
    playerMovement,
    messageText,
    damage,
    startPositions,
    timeTillStart,
    stats,
    time,
    part,
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
    int serverPort = 63578;
    float timer = 300;
    int lasttime = 300;
    [SerializeField] float maxTime = 120;


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

    public uint  CurrentTick { get; private set; } = 0;


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
        timer = maxTime;


        currentServerState = CurrentServerState.Open;
        currentLobbyType = CurrentLobbyType.Casual;
    RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);

        Server = new Server();
        Server.Start(port, maxClientCount);
        Server.ClientConnected += PlayerJoined;
        Server.ClientDisconnected += PlayerLeft;
        Console.Clear();
        Debug.Log("SERVER HAS SUCESSFULLY OPEN ON PORTS : " + port);
    }

    private void PlayerJoined(object sender, ServerClientConnectedEventArgs e)
    {   

    }

    internal void ConfirmAccountConnection(Player player)
    {
        StartCoroutine(TryTestAccount(player));
    }


    public void ConfirmConnection()
    {
        if (Server.ClientCount == Server.MaxClientCount)
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
        currentServerState = CurrentServerState.InGame;
        UpdateServer();
        SendTextNotif("Game will start soon!");
        for(int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(1f);
        }


        for (int i = 0; i < 5; i++)
        {
            SendTextNotif(""+ (5-i));
            yield return new WaitForSeconds(1f);
        }
        SendTextNotif("Go!");
        foreach (KeyValuePair<ushort, Player> car in Player.list)
        {
            car.Value.ChangerState(new EtatVoitureMouvement(car.Value.gameObject));
        }
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
        serverPort = UnityEngine.Random.Range(63577, 64000);
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

    IEnumerator TryTestAccount(Player p)
    {
        WWWForm form = new WWWForm();
        form.AddField("rUsername", p.Username);
        UnityWebRequest request = UnityWebRequest.Post($"{mainAddress}user", form);
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
            LoginResponse response = JsonUtility.FromJson<LoginResponse>(request.downloadHandler.text);
            switch (response.code)
            {
                case 0:
                    p.thisaaccounttemp = response.data;
                    break;
                default:
                    break;
            }

        }
        else
        {
            KickPlayer(p.Id);
        }
        ConfirmConnection();
        yield return null;
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

            if (CurrentTick % 200 == 0)
            {
                SendSync();
            }

            CurrentTick++;

        }



    }

    private void SendSync()
    {
        Message message = Message.Create(MessageSendMode.unreliable, (ushort)ServerToClientId.sync);
        message.Add(CurrentTick);
        Server.SendToAll(message);
    }

    private void SendTextNotif(string s)
    {
        Message message = Message.Create(MessageSendMode.unreliable, (ushort)ServerToClientId.messageText);
        message.AddString(s);
        Server.SendToAll(message);
    }

    private void SendTime(string time)
    {
        Message message = Message.Create(MessageSendMode.unreliable, (ushort)ServerToClientId.time);
        message.AddString(time);
        Server.SendToAll(message);
    }


    public void KickPlayer(ushort id)
    {
        Server.DisconnectClient(id);

        if (Player.list.TryGetValue(id, out Player player))
        {
            Destroy(player.gameObject);
        }
    }

    private void Update()
    {
        if(currentServerState == CurrentServerState.Battle)
        {
            timer -= Time.deltaTime;
            amountofTimeLeft();
            if(timer <= 0)
            {
                EndGame();
            }
        }
    }


    string amountofTimeLeft()
    {
        float minutes = Mathf.Floor(timer / 60);
        float seconds = timer % 60;
        int truesec = Mathf.RoundToInt(seconds);

        string t = "";
        t = minutes < 10 ? t += "0" + minutes : t += minutes;
        t+=":";
        t = truesec < 10 ? t+="0" + truesec : t+=truesec;
        if (lasttime > truesec)
        {
            SendTime(t);
        }
        lasttime = truesec;


        return t;
    }
    private void EndGame()
    {

        //Send to Master Server Who Wins or loses
        //Kick Players
        foreach(KeyValuePair<ushort, Player> t in Player.list)
        {
            KickPlayer(t.Key);
        }

        RestartServer();
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
        if(Server.ClientCount == 0)
        {
            RestartServer();
        }
        else
        {

            UpdateServer();
        }

    }

    private void RestartServer()
    {

        currentServerState = CurrentServerState.Open;
        currentLobbyType = CurrentLobbyType.Casual;
        timer = maxTime;
        UpdateServer();
    }


}
