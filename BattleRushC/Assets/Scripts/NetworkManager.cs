using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Riptide;
using Riptide.Utils;
using System;
using UnityEngine.SceneManagement;
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


public class NetworkManager : MonoBehaviour
{


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


    public bool isHosting = false;
    public bool started = false;

    private uint _serverTick;
    public uint ServerTick
    {
        get => _serverTick;
        private set
        {
            _serverTick = value;
            InterpolationTick = (value - TicksBetweenPositionUpdates);
        }
    }


    public uint InterpolationTick { get; private set; }
    public uint _ticksBetweenPositionUpdates = 2;
    public uint TicksBetweenPositionUpdates
    {
        get => _ticksBetweenPositionUpdates;
        private set
        {
            _ticksBetweenPositionUpdates = value;
            InterpolationTick = (ushort)(ServerTick - value);
        }
    }

    [Space(10)]
    [SerializeField] private ushort tickDivergenceTolerance = 1;
    public Client Client
    {
        get; private set;
    }



    [SerializeField] public List<string> MapLists;

    [SerializeField] private string ip;
    [SerializeField] private string port;

    private bool GameOver = false;


    [MessageHandler((ushort)ServerToClientId.timeTillStart)]
    private static void TimeTillStart(Message message)
    {
        //InGameUI.instance.Start(message.GetFloat());
    }

    private void Awake()
    {
        Singleton = this;
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);
        Client = new Client();
        Client.Connected += DidConnect;
        Client.ConnectionFailed += OnFailedToConnect;
        Client.ClientDisconnected += PlayerLeft;
        Client.Disconnected += DidDisconnect;
        ServerTick = 2;
    }



    public void SendReady()
    {

        Message message = Message.Create(MessageSendMode.Reliable, ClientToServerId.ready);
        NetworkManager.Singleton.Client.Send(message);
    }

    public void ConnectTo(Server s)
    {
        this.ip = s.ip;
        this.port = s.port;
        StartCoroutine(Connection());
    }

    public void Connect()
    {
        Client.Connect($"{ip}:{port}");
    }

    public void ConnectTo(string ip, string port)
    {
        this.ip = ip;
        this.port = port;

        StartCoroutine(Connection());
    }





    private IEnumerator Connection()
    {
        string playscene = "Lobby";

        LoadingScene.main.LoadScene(playscene);
        while (SceneManager.GetActiveScene().name != playscene)
        {
            yield return null;
        }


        Debug.Log($"Connection to {ip}:{port}");
        Connect();


    }

    private void DidConnect(object sender, EventArgs e)
    {
        DontDestroyOnLoad(this.gameObject);
        SendName();
        //UIManager.Singleton.SendName();
    }

    private void PlayerLeft(object sender, ClientDisconnectedEventArgs e)
    {
        if (Player.list.TryGetValue(e.Id, out Player player))
        {

            Destroy(player.gameObject);
        }
    }

    public void Disconnect()
    {
        Client.Disconnect();
        OnServerDeco();
    }

    public void OnServerDeco()
    {

        foreach (Player player in Player.list.Values)
        {
            Destroy(player.gameObject);
        }
        
        LoadingScene.main.LoadScene("MainMenuScene");
    }
    private void OnFailedToConnect(object sender, EventArgs e)
    {
        LoadingScene.main.LoadScene("MainMenuScene");
    }

    private void DidDisconnect(object sender, EventArgs e)
    {
        if (GameOver)
        {
            foreach (Player player in Player.list.Values)
            {
                Destroy(player.gameObject);
            }

            LoadingScene.main.LoadScene("EndGameScene", new Action(WinScreenManager.Singleton.GetAllCars));

        }
        else
        {

            OnServerDeco();
        }

    }

    public void SendName()
    {
        Message message = Message.Create(MessageSendMode.Reliable, ClientToServerId.name);
        message.AddString(PlayerAccount.connectedUser.username);
        message.AddColors(CustomizeManager.ColorToVector(0), CustomizeManager.ColorToVector(1), CustomizeManager.ColorToVector(2));
        //message.AddVector3(CustomizeManager.ColorToVector(0));
        //message.AddVector3(CustomizeManager.ColorToVector(1));
        //message.AddVector3(CustomizeManager.ColorToVector(2));
        Client.Send(message);
        
    }






    private void FixedUpdate()
    {
      
        Client.Update();
        ServerTick++;
    }

    private void OnApplicationQuit()
    {
        if (Client.IsConnected)
        {
            Client.Disconnect();
        }
        AccountDisconnect();
    }


    public void ConfirmDisconnection(Response t)
    {
        PlayerAccount.Disconnected();
    }


    public void AccountDisconnect()
    {
        Action<DecoResponse> Success = new Action<DecoResponse>(ConfirmDisconnection);
        Action Failure = new Action(PlayerAccount.Disconnected);
        WWWForm form = new WWWForm();
        form.AddField("tokenid", PlayerAccount.connectionToken);
        string link = "deconnexion";
        StartCoroutine(ServerTalker.PostRequestToMasterServer<DecoResponse>(link, form, Success, Failure));

    }


    private void SetTick(ushort serverTick)
    {
        if(Mathf.Abs(ServerTick - serverTick) > tickDivergenceTolerance)
        {
            Debug.Log($"Client tick : {ServerTick} -> {serverTick}");
            ServerTick = serverTick;
        }
    }


    [MessageHandler((ushort)ServerToClientId.scene)]
    private static void GameScene(Message message)
    {
        ConnectionMenuDebug.singleton.gameObject.SetActive(false);
        Debug.Log("Enter Game");
        Action success = new Action(Singleton.SendGameSceneLoaded);
        LoadingScene.main.LoadScene(Singleton.MapLists[0], success);
    }


    public void SendGameSceneLoaded()
    {
        Debug.Log("Finished Loading");
        Message message = Message.Create(MessageSendMode.Reliable, ClientToServerId.gameSceneLoaded);
        Client.Send(message);

    }
    [MessageHandler((ushort)ServerToClientId.colorsforend)]
    public static void EndGameName(Message message)
    {
        Singleton.GameOver = true;
        WinScreenManager.Singleton.playerCount = message.GetInt();
        if (Singleton.MapLists.Contains(SceneManager.GetActiveScene().name))
        {
            for (int i = 0; i < WinScreenManager.Singleton.GraphicCars.Length; i++)
            {
                string username = message.GetString();
                bool found = false;
                foreach (KeyValuePair<ushort, Player> player in Player.list)
                {
                    if (player.Value.Username.Equals(username))
                    {
                        found = true;
                        WinScreenManager.Singleton.ColorCars[i, 0] = player.Value.carGraphics.GetBody();
                        WinScreenManager.Singleton.ColorCars[i, 0] = player.Value.carGraphics.GetLights();
                        WinScreenManager.Singleton.ColorCars[i, 0] = player.Value.carGraphics.GetRims();
                        WinScreenManager.Singleton.carNames[i] = username;
                    }
                }
                if (!found)
                {
                    WinScreenManager.Singleton.stillConnected[i] = false;
                }
            }
        }
    }

    [MessageHandler((ushort)ServerToClientId.sync)]
    public static void Sync(Message message)
    {
        Singleton.SetTick(message.GetUShort());
    }
    [MessageHandler((ushort)ServerToClientId.time)]
    public static void setTime(Message message)
    {
        Singleton.UpdateTime(message.GetString());
    }

    private void UpdateTime(string timer)
    {
        UIManager.Singleton.SetTimerAmount(timer);
    }

    [MessageHandler((ushort)ServerToClientId.messageText)]
    public static void Text(Message message)
    {
        string m = message.GetString();
        GameObject messageObject = Instantiate(GameLogic.Singleton.message, GameObject.Find("Canvas").transform);
        messageObject.GetComponent<TextAnimator>().SetText(m);
        Debug.Log(m);
        if (UIManager.Singleton.gameObject.GetComponent<CanvasGroup>().alpha == 0)
        {

            UIManager.Singleton.StartCoroutine(UIManager.Singleton.ActivateUI());
        }
        if (m == "3")
        {
            CinematicCamera.Transition();
        }

    }

   
}
