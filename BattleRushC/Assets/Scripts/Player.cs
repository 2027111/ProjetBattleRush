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
    public bool[] inputs = new bool[7];//if necessary
    [SerializeField] public GameObject modelCar;
    [SerializeField] public GameObject attack;
    [SerializeField] public GameObject flightburst;
    [SerializeField] public Camera camProxy;
    [SerializeField] public GameObject camHolder;
    [SerializeField] public GameObject usernameCanvas;
    [SerializeField] public CarGraphics carGraphics;
    [SerializeField] public Interpolator interpolater;
    [SerializeField] public Image carMap;
    public event Evenement EvenementHandler;
    public int points
    {

        get;
        private set;
    } = 0;


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
        player.carGraphics.SetBody(colorBody);
        player.carMap.color = new Color(colorBody.x, colorBody.y, colorBody.z);
        player.carGraphics.SetEmissions(colorEmi);
        player.carGraphics.SetRims(colorRims);

        list.Add(id, player);
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
        interpolater.NewUpdate(tick, newPosition); //Configurer syst�me d'interpolation de mouvement pour rendre les changements de positions plus fluide.
        Vector3 currentpos = transform.position;
        //transform.position = Vector3.LerpUnclamped(currentpos, newPosition, 0.025f);
        transform.position = newPosition;
        transform.rotation = carRot;
        modelCar.transform.rotation = modelCarRot;
    }

    

    // Start is called before the first frame update
    void Start()
    {

        UIManager.Singleton.SetSBCard(this);
    }


}
