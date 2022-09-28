using RiptideNetworking;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] public Interpolator interpolater;

    int points = 0;



    private void OnDestroy()
    {
        list.Remove(Id);
    }


    public static void Spawn(ushort id, string username, Vector3 position, Quaternion rot)
    {
        Player player;
        player = Instantiate(GameLogic.Singleton.PlayerPrefab, position, rot).GetComponent<Player>();
        if (id == NetworkManager.Singleton.Client.Id)
        {
            player.IsLocal = true;
            player.gameObject.AddComponent<PlayerController>();
            FollowLocalPlayer.SetPlayer(player.gameObject);
        }
        else
        {
            Destroy(player.camProxy);
        }
        player.name = $"Player {id} {(string.IsNullOrEmpty(username) ? "Guest" : username)}";
        player.Id = id;
        player.Username = username;
        list.Add(id, player);
    }

    [MessageHandler((ushort)ServerToClientId.playerMovement)]
    private static void PlayerMovement(Message message)
    {
        if (list.TryGetValue(message.GetUShort(), out Player player))
        {
            player.Move(message.GetUInt(), message.GetVector3(), message.GetQuaternion(), message.GetQuaternion()) ;

        }
    }

    [MessageHandler((ushort)ServerToClientId.playerSpawned)]
    private static void SpawnPlayer(Message message)
    {
        ushort playeid = message.GetUShort();
        Spawn(playeid, message.GetString(), message.GetVector3(), message.GetQuaternion());
    }

 
    private Message AddSpawnData(Message message)
    {

        message.AddUShort(Id);
        //

        message.AddString(Username);
        message.AddVector3(transform.position);
        return message;
    }

    private void Move(uint tick, Vector3 newPosition, Quaternion carRot, Quaternion modelCarRot)
    {
        //interpolater.NewUpdate(tick, newPosition); //Configurer système d'interpolation de mouvement pour rendre les changements de positions plus fluide.
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


    // Update is called once per frame
    void Update()
    {

     
    }
}
