using Riptide;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public delegate void Evenement();
public class Player : MonoBehaviour

{

    public static Dictionary<ushort, Player> list = new Dictionary<ushort, Player>();
    public ushort Id { get; set; }

    public bool IsLocal { get; set; } = false;


    public string Username { get; set; }
    bool isReady = false;
    public bool[] inputs = new bool[7];//if necessary
    public GameObject modelCar
    {
        get
        {
            return gameObject.transform.Find("CarModel").gameObject;
        }
    }
    public GameObject attack
    {
        get
        {
            return gameObject.transform.Find("Attack").gameObject;
        }
    }
    public GameObject flightburst
    {
        get
        {
            return gameObject.transform.Find("FlightBurst").gameObject;
        }
    }
    public Camera camProxy
    {
        get
        {
            return GetComponentInChildren<Camera>();
        }
    }
    public GameObject camHolder
    {
        get
        {
            return gameObject.transform.Find("CamHolder").gameObject;
        }
    }
    public GameObject usernameCanvas
    {
        get
        {
            return gameObject.transform.Find("Username").gameObject;
        }
    }
    public CarGraphics carGraphics
    {
        get
        {
            return GetComponentInChildren<CarGraphics>();
        }
    }
    public CardGraphics cardGraphics
    {
        get
        {
            return GetComponentInChildren<CardGraphics>();
        }
    }
    public Interpolator interpolater
    {

        get
        {
            return GetComponent<Interpolator>();
        }
    }
    public Image carMap
    {
        get
        {
            return gameObject.transform.Find("Canvas").GetChild(0).GetComponent<Image>();
        }
    }
    public event Evenement EvenementHandler;
    public int points
    {
        get;
        private set;
    } = 0;

    [MessageHandler((ushort)ServerToClientId.playerisReady)]
    private static void serverPlayerReady(Message message)
    {
        if (list.TryGetValue(message.GetUShort(), out Player player))
        {
            player.isReady = message.GetBool();
            if (player.cardGraphics.readyText != null)
            {
                player.cardGraphics.readyText.text = player.isReady ? "Ready!" : "Not Ready!";
            }
        }
    }

    private void OnDestroy()
    {
        list.Remove(Id);
    }

    public void RotateCam()
    {

            //Instantiate();
            Vector3 rot = camHolder.transform.localRotation.eulerAngles;

            if (rot.y == 180)
            {
                rot.y = 0;
            }
            else
            {

                rot.y = 180;
            }
            camHolder.transform.localRotation = Quaternion.Euler(rot);
        
    }
    public static void SpawnCard(ushort id, string username)
    {
        Player player;
        player = Instantiate(GameLogic.Singleton.LobbyPlayerPrefab, GameObject.Find("CardContainer").transform).GetComponent<Player>();
        player.IsLocal = id == NetworkManager.Singleton.Client.Id;
        player.name = $"Player {id} {(string.IsNullOrEmpty(username) ? "Guest" : username)}";
        player.Id = id;
        player.Username = username;
        player.cardGraphics.usernameText.text = username;
        if (list.ContainsKey(id))
        {
            Destroy(list[id].gameObject);
        }

        list.Add(id, player);
    }


    public static void Spawn(ushort id, string username, Vector3 position, Quaternion rot, Vector3 colorBody, Vector3 colorEmi, Vector3 colorRims)
    {
        Player player;
        player = Instantiate(GameLogic.Singleton.PlayerPrefab, position, rot).GetComponent<Player>();
        if (id == NetworkManager.Singleton.Client.Id)
        {
            player.IsLocal = true;
            player.gameObject.AddComponent<PlayerController>();
            player.camProxy.transform.parent = null;
            FollowLocalPlayer.SetPlayer(player.gameObject);
            Destroy(player.usernameCanvas);
        }
        else
        {
            player.usernameCanvas.transform.GetChild(0).GetComponent<Text>().text = username;
            Destroy(player.camHolder);
        }
        player.name = $"Player {id} {(string.IsNullOrEmpty(username) ? "Guest" : username)}";
        player.Id = id;
        player.Username = username;
        Debug.Log(colorBody);
        Debug.Log(colorEmi);
        Debug.Log(colorRims);
        player.carGraphics.SetBody(colorEmi);
        player.carMap.color = new Color(colorEmi.x, colorEmi.y, colorEmi.z);
        player.carGraphics.SetEmissions(colorBody);
        player.carGraphics.SetRims(colorRims);

        if (list.ContainsKey(id))
        {
            Destroy(list[id].gameObject);
        }
        list.Add(id, player);
        UIManager.Singleton?.SetSBCard(player);
    }

    [MessageHandler((ushort)ServerToClientId.playerConnected)]
    private static void SpawnPlayerCard(Message message)
    {
        ushort playeid = message.GetUShort();
        SpawnCard(playeid, message.GetString());
    }

    [MessageHandler((ushort)ServerToClientId.playerMovement)]
    private static void PlayerMovement(Message message)
    {
        if (list.TryGetValue(message.GetUShort(), out Player player))
        {
            player.Move(message.GetUInt(), message.GetVector3(), message.GetQuaternion(), message.GetQuaternion()) ;
            if (player.IsLocal)
            {

                UIManager.Singleton.SetSpeed(message.GetVector3());
                UIManager.Singleton.SetBoost(message.GetFloat());
            }
            

        }
    }

    [MessageHandler((ushort)ServerToClientId.playerSpawned)]
    private static void SpawnPlayer(Message message)
    {
        ushort playeid = message.GetUShort();
        Spawn(playeid, message.GetString(), message.GetVector3(), message.GetQuaternion(), message.GetVector3(), message.GetVector3(), message.GetVector3());
    }
    [MessageHandler((ushort)ServerToClientId.stats)]
    private static void RecieveStats(Message message)
    {

        if (list.TryGetValue(message.GetUShort(), out Player player))
        {
            int points = message.GetInt();
            player.points = points;
            player.EvenementHandler();
        }
    
    }



    private void Move(uint tick, Vector3 newPosition, Quaternion carRot, Quaternion modelCarRot)
    {
        interpolater?.NewUpdate(tick, newPosition); //Configurer système d'interpolation de mouvement pour rendre les changements de positions plus fluide.
        Vector3 currentpos = transform.position;
        //transform.position = Vector3.LerpUnclamped(currentpos, newPosition, 0.025f);
        transform.position = newPosition;
        transform.rotation = carRot;
        modelCar.transform.rotation = modelCarRot;
    }

    
    
    // Start is called before the first frame update
    void Start()
    {

    }


}
