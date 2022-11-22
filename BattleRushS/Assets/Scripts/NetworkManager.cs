using Riptide;
using Riptide.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;

public enum ServerToClientId : ushort
{
    sync = 1,
    playerSpawned,
    playerConnected,
    playerisReady,
    playerMovement,
    scene,
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
    ready,
    gameSceneLoaded,
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
    float timer = 20;
    int lasttime = 20;
    [SerializeField] float maxTime = 20;


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

    Coroutine readyCoroutine;


    [Header("Debug")]
    [SerializeField] bool debug = false;

    private bool IsEveryoneLoaded()
    {
        if (Player.list.Count == 0)
        {
            return false;
        }
        foreach (KeyValuePair<ushort, Player> player in Player.list)
        {
            if (!player.Value.HasLoadedGameScene)
            {
                return false;
            }
        }
        return true;
    }
    private void OnPlayerLoadedGameScene()
    {
        if (IsEveryoneLoaded())
        {
            foreach (KeyValuePair<ushort, Player> player in Player.list)
            {
                player.Value.SpawnEveryone();
            }
            StartCoroutine(StartBattle());
        }
    }
    public void OnPlayerReady()
    {
        foreach (KeyValuePair<ushort, Player> player in Player.list)
        {
            player.Value.SendReady();
        }
        if (IsEveryoneReady())
        {
            if (readyCoroutine == null)
            {
                readyCoroutine = StartCoroutine(OnEveryoneReady());
            }
        }
        else
        {
            if (readyCoroutine != null)
            {

                StopCoroutine(readyCoroutine);
                readyCoroutine = null;
                Debug.Log("Stopped Ready");
            }
        }
    }
    private bool IsEveryoneReady()
    {
        if (Player.list.Count == 0)
        {
            return false;
        }
        foreach (KeyValuePair<ushort, Player> player in Player.list)
        {
            if (!player.Value.isReady)
            {
                return false;
            }
        }
        return true;
    }

    IEnumerator OnEveryoneReady()
    {
        for (int i = 0; i <= 3; i++)
        {
            yield return new WaitForSeconds(1f);
            Debug.Log(3 - i);
        }
        SendChangeSceneToGame();
    }

    private void SendChangeSceneToGame()
    {
        Debug.Log("Yes");
        Message message = Message.Create(MessageSendMode.Reliable, ServerToClientId.scene);
        NetworkManager.Singleton.Server.SendToAll(message);
    }

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

    private void PlayerLeft(object sender, ServerDisconnectedEventArgs e)
    {
        if (Player.list.TryGetValue(e.Client.Id, out Player player))
        {
            Destroy(player.gameObject);
        }
        if (debug)
        {
            return;
        }
        if (Server.ClientCount == 0)
        {
            RestartServer();
        }
        else
        {

            UpdateServer();
        }

    }
 
    private void PlayerJoined(object sender, ServerConnectedEventArgs e)
    {
    }


    private IEnumerator StartBattle()
    {
        //Send Who's left and who's right.
        //
        currentServerState = CurrentServerState.InGame;

        foreach (KeyValuePair<ushort, Player> car in Player.list)
        {
            car.Value.ChangerState(new EtatVoitureDebutPartie(car.Value.gameObject));
        }
        if (!debug)
        {
            UpdateServer();
        }
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


    private void GetFreePort()
    {
        serverPort = UnityEngine.Random.Range(63577, 64000);
    }


    private void UpdateServer()
    {

        Action<DescResponse> Success = new Action<DescResponse>(delegate { UpdateSuccess(true); });
        Action Failure = new Action(delegate { UpdateSuccess(false); });
        WWWForm form = new WWWForm();
        form.AddField("ip", "127.0.0.1");
        form.AddField("port", port);
        form.AddField("maxPlayer", maxClientCount);;
        form.AddField("lobbystate", currentServerState.ToString());
        form.AddField("lobbytype", currentLobbyType.ToString());
        form.AddField("connectedPlayer", Server.ClientCount);
        string link = "server/update";
        StartCoroutine(NetworkManager.PostRequestToMasterServer<DescResponse>(link, form, Success, Failure));

        //StartCoroutine(TryUpdateServer());
    }

    void UpdateSuccess(bool update)
    {
        string t = update ? "Update Successful" : "Update failed";
        Debug.Log(t);
    }




    public void ConfirmAccountConnection(Player player)
    {
        if (debug)
        {
            ConfirmConnection(null);
            return;
        }
        Action<DescResponse> Success = new Action<DescResponse>(ConfirmConnection);
        Action Failure = new Action(delegate { KickPlayer(player.Id);});
        WWWForm form = new WWWForm();
        form.AddField("rUsername", player.Username);
        string link = "user";

        StartCoroutine(NetworkManager.PostRequestToMasterServer<DescResponse>(link, form, Success, Failure));

    }


    public void ConfirmConnection(DescResponse response)
    {
            if (debug)
            {
                return;
            }
            GetFreePort();
                Action<DescResponse> Success = new Action<DescResponse>(ConfirmConnectionSuccess);
                Action Failure = new Action(delegate { UpdateSuccess(false); });
                WWWForm form = new WWWForm();
                form.AddField("port", port);
                string link = "server";
                StartCoroutine(NetworkManager.PostRequestToMasterServer<DescResponse>(link, form, Success, Failure));



        UpdateServer();


    }
    void ConfirmConnectionSuccess(DescResponse response)
    {
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
    #region [ServerStart]
    void ServerStartSuccess(DescResponse response)
    {
        switch (response.code)
        {
            case 0:
                ProcessServerStart();
                break;

            default:
                GetFreePort();
                break;

        }

    }

    void ServerStartFailure()
    {

        Debug.Log("Server start failed");
        Application.Quit();
    }
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
    public void StartServer()
    {
        if (Server != null)
        {
            return;
        }

        GetFreePort();

        if (debug)
        {
            ProcessServerStart();
        }
        else
        {
            Action<DescResponse> Success = new Action<DescResponse>(ServerStartSuccess);
            Action Failure = new Action(ServerStartFailure);
            WWWForm form = new WWWForm();
            form.AddField("ip", "127.0.0.1");
            form.AddField("port", serverPort);
            form.AddField("maxPlayer", maxClientCount);
            string link = "server/On";
            StartCoroutine(NetworkManager.PostRequestToMasterServer<DescResponse>(link, form, Success, Failure));

        }
    }
    #endregion
    
    #region [ServerStop]

    void ServerStopSuccess(DescResponse response)
    {

        ProcessServerStop();
    

    }

    private void ProcessServerStop()
    {
        if (Server != null && Server.IsRunning)
        {
            Server.Stop();

        }
    }

    public void StopServer()
    {
        Action<DescResponse> Success = new Action<DescResponse>(ServerStopSuccess);
        Action Failure = new Action(delegate { ServerStopSuccess(null); });
        WWWForm form = new WWWForm();
        form.AddField("port", port);
        string link = "server/Off";
        StartCoroutine(NetworkManager.PostRequestToMasterServer<DescResponse>(link, form, Success, Failure));

    }
    #endregion

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (Server != null)
        {
            Server.Update();

            if (CurrentTick % 200 == 0)
            {
                SendSync();
            }

            CurrentTick++;

        }



    }

    private void SendSync()
    {
        Message message = Message.Create(MessageSendMode.Unreliable, (ushort)ServerToClientId.sync);
        message.Add(CurrentTick);
        Server.SendToAll(message);
    }

    private void SendTextNotif(string s)
    {
        Message message = Message.Create(MessageSendMode.Unreliable, (ushort)ServerToClientId.messageText);
        message.AddString(s);
        Server.SendToAll(message);
    }

    private void SendTime(string time)
    {
        Message message = Message.Create(MessageSendMode.Unreliable, (ushort)ServerToClientId.time);
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

    [MessageHandler((ushort)ClientToServerId.gameSceneLoaded)]
    private static void gameScene(ushort fromClientId, Message message)
    {
        if (Player.list.TryGetValue(fromClientId, out Player player))
        {
            player.HasLoadedGameScene = true;
            NetworkManager.Singleton.OnPlayerLoadedGameScene();
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
            if (timer <= 10)
            {
                SendTextNotif(truesec + "");
            }
        }
        lasttime = truesec;

        return t;
    }
    private void EndGame()
    {

        int maxamount = Server.ClientCount;
        foreach (KeyValuePair<ushort, Player> t in Player.list)
        {



        }


        //Send to Master Server Who Wins or loses
        //Kick Players
        foreach (KeyValuePair<ushort, Player> t in Player.list)
        {
            KickPlayer(t.Key);
        }

        RestartServer();
    }

    private void OnApplicationQuit()
    {
        StopServer();
    }

    


    private void RestartServer()
    {

        currentServerState = CurrentServerState.Open;
        currentLobbyType = CurrentLobbyType.Casual;
        timer = maxTime;
        UpdateServer();
    }

    public static IEnumerator PostRequestToMasterServer<T>(string link, WWWForm form, Action<T> success, Action failure)
    {
        UnityWebRequest request = UnityWebRequest.Post($"{mainAddress}{link}", form);
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
            T response = JsonUtility.FromJson<T>(request.downloadHandler.text);
            success(response);

        }
        else
        {
            failure();
        }
        yield return null;
    }


}
