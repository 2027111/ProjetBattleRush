using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;
using RiptideNetworking.Utils;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

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

    public Client Client
    {
        get; private set;
    }




    public void ConnectTo(Server s)
    {
        this.ip = s.ip;
        this.port = s.port;
        StartCoroutine(Connection());
    }

    [SerializeField] private string ip;
    [SerializeField] private string port;



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
    }




    public void Connect()
    {
        Client.Connect($"{ip}:{port}");
    }

    public void Connect(string ip, string port)
    {
        this.ip = ip;
        this.port = port;

        StartCoroutine(Connection());
    }





    private IEnumerator Connection()
    {
        string playscene = "GameScene";

        SceneManager.LoadScene(playscene);
        while (SceneManager.GetActiveScene().name != playscene)
        {
            Debug.Log("Loading Scene");
            yield return null;
        }


        Debug.Log($"Connection to {ip}:{port}");
        Connect();


    }

    private void DidConnect(object sender, EventArgs e)
    {
        Debug.Log("Sending Name");
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
        foreach (Player player in Player.list.Values)
        {
            Destroy(player.gameObject);
        }
    }

    private void OnFailedToConnect(object sender, EventArgs e)
    {
    }

    private void DidDisconnect(object sender, EventArgs e)
    {
        foreach (Player player in Player.list.Values)
        {
            Destroy(player.gameObject);
        }

        SceneManager.LoadScene("MenuScene");
    }

    public void SendName()
    {
        Message message = Message.Create(MessageSendMode.reliable, ClientToServerId.name);
        message.AddString(PlayerAccount.connectedUser.username);
        Client.Send(message);
        //
    }






    private void FixedUpdate()
    {
      
        Client.Tick();
    }

    private void OnApplicationQuit()
    {
        if (Client.IsConnected)
        {
            Client.Disconnect();
        }
        AccountDisconnect();
    }




    public void AccountDisconnect()
    {
        StartCoroutine(TryDisconnect());
    }

    IEnumerator TryDisconnect()
    {
        WWWForm form = new WWWForm();
        form.AddField("tokenid", PlayerAccount.connectionToken);
        UnityWebRequest request = UnityWebRequest.Post($"{ServerTalker.mainAddress}deconnexion", form);
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

            PlayerAccount.Disconnected();
        }
        else
        {
            Debug.Log("Disconnection failed");
        }
        yield return null;
    }






}
