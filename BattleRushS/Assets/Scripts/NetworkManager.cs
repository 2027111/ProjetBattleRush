using Riptide;
using Riptide.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
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
    colorsforend,

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

    public List<CarteJoueur> joueurPoint = new List<CarteJoueur>();

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
    [SerializeField] private string ip = "127.0.0.1";
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
        foreach(KeyValuePair<ushort, Player> player in Player.list)
        {
            joueurPoint.Add(new CarteJoueur(player.Value.Username, player.Value.points));
        }
        SendChangeSceneToGame();
    }

    private void SendChangeSceneToGame()
    {
        Message message = Message.Create(MessageSendMode.Reliable, ServerToClientId.scene);
        NetworkManager.Singleton.Server.SendToAll(message);
    }


    public string GetLocalIPv4()
    {
        /*return Dns.GetHostEntry(Dns.GetHostName())
            .AddressList.First(
                f => f.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            .ToString();*/



        foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (ni.OperationalStatus == OperationalStatus.Up)
            {
                if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                    {
                        if (ip != null && ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {

                            Debug.Log(ip.Address.ToString());
                            return ip.Address.ToString();
                        }
                            
                    }
                }
            }
        }
        return null;
    
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
            if (currentServerState == CurrentServerState.Battle)
            {
                foreach (CarteJoueur j in joueurPoint)
                {
                    if(j.Username == player.Username)
                    {
                        j.Points -= 9999;
                    }
                }
            }
            Destroy(player.gameObject);
        }
        if (debug)
        {
            return;
        }
        if (Server.ClientCount == 0)
        {
            EndGame();
        }
        else
        {

            UpdateServer();
        }

    }

    public void SetJoueurPoint(string username, int points)
    {
        foreach (CarteJoueur j in joueurPoint)
        {
            if (j.Username == username)
            {
                j.Points = points;
            }
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
        form.AddField("ip", ip);
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
            if(GetLocalIPv4() != null)
            {
                ip = GetLocalIPv4();
            }
            Action<DescResponse> Success = new Action<DescResponse>(ServerStartSuccess);
            Action Failure = new Action(ServerStartFailure);
            WWWForm form = new WWWForm();
            form.AddField("ip", ip);
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


        joueurPoint.Sort();
        for(int i = 0; i < joueurPoint.Count; i++)
        {

            SendPlayerScoreRequest(joueurPoint[i].Username, i+1, joueurPoint.Count);
            bool found = false;
            foreach (KeyValuePair<ushort, Player> t in Player.list)
            {
                if (t.Value.Username.Equals(joueurPoint[i].Username))
                {
                    found = true;
                    SendAllNames(joueurPoint.ToArray());
                    
                }
            }

        }



        //Send to Master Server Who Wins or loses
        //Kick Players
        foreach (KeyValuePair<ushort, Player> t in Player.list)
        {
            KickPlayer(t.Key);
        }

        RestartServer();
    }
    internal void SendAllNames(CarteJoueur[] lol)
    {
        Message message = Message.Create(MessageSendMode.Unreliable, ServerToClientId.colorsforend);
        message.Add(Server.ClientCount);
        foreach (CarteJoueur lo in lol)
        {
            message.AddString(lo.Username);
        }
        NetworkManager.Singleton?.Server.SendToAll(message);
    }
    private void SendPlayerScoreRequest(string username, int position, int count)
    {
        Action<DescResponse> Success = new Action<DescResponse>(ConfirmConnectionSuccess);
        Action Failure = new Action(delegate { UpdateSuccess(false); });
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("position", position);
        form.AddField("serverCount", count);
        string link = "server/winlose";
        StartCoroutine(NetworkManager.PostRequestToMasterServer<DescResponse>(link, form, Success, Failure));


    }

    public void Success(DescResponse response)
    {

    }
    public void Failure()
    {

    }

    private void WinLoseRequest(string username, int position, int maxcount)
    {

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
