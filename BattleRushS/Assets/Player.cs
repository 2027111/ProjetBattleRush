using RiptideNetworking;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour

{



    public static Dictionary<ushort, Player> list = new Dictionary<ushort, Player>();
    public ushort Id { get; private set; }
    public bool IsLocal { get; private set; }

    [SerializeField] public float speed = 10;
    [SerializeField] public Vector3 direction = new Vector3(0, 0, 1);
    [SerializeField] public GameObject modelCar;
    [SerializeField] public GameObject attack;
    [SerializeField] public GameObject flightburst;
    [SerializeField] public Camera camProxy;
    [SerializeField] public GameObject camHolder;
    [SerializeField] public LayerMask lm;
    [SerializeField] public bool control = false;
    public float damage = 0;
    public Player lastHit = null;
    public float boostamount = 100;
    int points = 0;

    public bool[] inputs = new bool[6];



    public string[] inputchar = { "W", "A", "S", "D", "SPACE", "SHIFT"};

    public Rigidbody rb;

    EtatVoiture etatActuel;


    public string Username { get; private set; }

    private void OnDestroy()
    {
        list.Remove(Id);
    }

    public bool GetAccel()
    {
        return (etatActuel as EtatVoitureMouvement).accelerating;
    }


    // Start is called before the first frame update
    void Start()
    { 
        rb = GetComponent<Rigidbody>();
        ChangerState(new EtatVoitureDebutPartie(this.gameObject));
        direction = transform.forward;
    }
    public void ChangerState(EtatVoiture ev)
    {
        if(etatActuel != null)
        {
            etatActuel.Exit();
        }
        etatActuel = ev;
        ev.Enter();
    }


    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.GetComponent<AttackZone>())
        {
            AttackZone attacked = collision.gameObject.GetComponent<AttackZone>();
            Player attackerCar = attacked.gameObject.transform.parent.gameObject.GetComponent<Player>();
                if (attacked != attack)
                {
                Strike(attackerCar);
                }
            
        }else if (collision.gameObject.tag == "DirectionZone")
        {
            ChangeDirection(collision.gameObject.transform.forward);
        }else if (collision.gameObject.GetComponent<DeathZone>())
        {
            if (lastHit)
            {
                lastHit.points += collision.gameObject.GetComponent<DeathZone>().pointsByKills;
                lastHit.SendStats();

            }
            else
            {

                points -= collision.gameObject.GetComponent<DeathZone>().suicidePenality;
                SendStats();
            }

            transform.rotation = Quaternion.Euler(Vector3.zero);
            rb.velocity = Vector3.zero;
            transform.position = GameObject.Find("Spawn").transform.position;
            lastHit = null;
            damage = 0;
        }
    }


    private void Strike(Player attackerCar)
    {
        Debug.Log(this.name + " was hit!");
        lastHit = attackerCar;
        ChangerState(new EtatVoitureFrapper(this.gameObject, attackerCar));
    }
    public void ChangeDirection(Vector3 dir)
    {
        if(direction == dir)
        {
            return;
        }
        else
        {
            Debug.Log("Changing direction to " + dir);
            direction = dir;
            StartCoroutine(changeDir());
        }
    }
    IEnumerator changeDir()
    {
        float t = 0;
        Vector3 temp = transform.forward;
        temp.y = 0;
        temp.Normalize();
        while (t < 1)
        {
            transform.forward = Vector3.Slerp(temp, direction, t);
            t+= 3 * Time.deltaTime;
            yield return null;
        }



        yield return null;
    }

    // Update is called once per frame
    void Update()
    {

     
        if (control)
        {
            if (inputs[4])
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

        }
        etatActuel.Handle();
        SendMovement();
    }


    [MessageHandler((ushort)ClientToServerId.input)]
    private static void Input(ushort fromClientId, Message message)
    {
        if (list.TryGetValue(fromClientId, out Player player))
        {
            player.SetInput(message.GetBools(6));
        }
    }
    [MessageHandler((ushort)ClientToServerId.name)]
    private static void Name(ushort fromClientId, Message message)
    {
        Spawn(fromClientId, message.GetString());

    }

    public void SendStats()
    {
        Message message = Message.Create(MessageSendMode.unreliable, ServerToClientId.stats);
        message.AddUShort(Id);
        message.AddInt(points);
        NetworkManager.Singleton.Server.SendToAll(message);

    }

    private void SetInput(bool[] vs)
    {
        inputs = vs;
    }
    private Message AddSpawnData(Message message)
    {

        message.AddUShort(Id);
        message.AddString(Username);
        message.AddVector3(transform.position);
        message.AddQuaternion(GameManager.Singleton.SpawnPoint.transform.rotation);
        return message;
    }
    private void SendSpawned()
    {

        NetworkManager.Singleton.Server.SendToAll(AddSpawnData(Message.Create(MessageSendMode.reliable, ServerToClientId.playerSpawned)));
    }

    private void SendSpawned(ushort toClientid)
    {

        NetworkManager.Singleton.Server.Send(AddSpawnData(Message.Create(MessageSendMode.reliable, ServerToClientId.playerSpawned)), toClientid);
    }
    private static void Spawn(ushort id, string username)
    {
        foreach (Player otherPlayer in list.Values)
        {
            otherPlayer.SendSpawned(id);
        }

        Vector3 pos = GameManager.Singleton.SpawnPoint.transform.position;
        pos += GameManager.Singleton.SpawnPoint.transform.right * 2 * (NetworkManager.Singleton.Server.ClientCount - 1);
        Player player = Instantiate(GameManager.Singleton.PlayerPrefab, pos, GameManager.Singleton.SpawnPoint.transform.rotation).GetComponent<Player>();
       
        player.name = $"Player {id} {(string.IsNullOrEmpty(username) ? "Guest" : username)}";
        player.Id = id;
        player.Username = string.IsNullOrEmpty(username) ? $"Guest {id}" : username;
        player.SendSpawned();
        list.Add(id, player);
    }
    private void SendMovement()
    {
        if(NetworkManager.Singleton.CurrentTick % 2 != 0)
        {
            return;
        }
        Message message = Message.Create(MessageSendMode.unreliable, ServerToClientId.playerMovement);
        message.AddUShort(Id);
        message.AddUInt(NetworkManager.Singleton.CurrentTick);
        message.AddVector3(transform.position);
        message.AddQuaternion(transform.rotation);
        message.AddQuaternion(modelCar.transform.rotation);
        NetworkManager.Singleton.Server.SendToAll(message);
    }
}
