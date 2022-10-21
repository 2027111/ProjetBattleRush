using Riptide;
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
    [SerializeField] public LayerMask lm;
    [SerializeField] public bool control = false;
    public PlayerAccount thisaaccounttemp;
    public float damage = 0;
    public Player lastHit = null;
    public float boostamount = 100;
    public bool canboost = true;
    int points = 0;

    public bool[] inputs = new bool[6];
    public string[] inputchar = { "W", "A", "S", "D", "SPACE", "SHIFT"};

    public Rigidbody rb;

    EtatVoiture etatActuel;
    private Vector3 color;

    protected static void WriteAt(string s, int x, int y)
    {
        try
        {
            Console.SetCursorPosition(0 + x, 14 - y);
            Console.Write(s);
        }
        catch (ArgumentOutOfRangeException e)
        {
            Console.WriteLine(e.Message);
        }
    }
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

    private void OnCollisionEnter(Collision collision)
    {

        Vector3 positionofcontact = collision.GetContact(0).point;
        SendParticle(0, positionofcontact);
    }

    public void SetProfile(string v)
    {
        Username = v;
    }



    private void SendParticle(int v, Vector3 positionofcontact)
    {

        Message message = Message.Create(MessageSendMode.Unreliable, ServerToClientId.part);
        message.AddInt(v);
        message.AddVector3(positionofcontact);
        NetworkManager.Singleton?.Server.SendToAll(message);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "HurtBox")
        {
            Player attacker = collision.gameObject.transform.parent.gameObject.GetComponent<Player>();
                if (attacker != this)
                {
                    if(attacker.rb.velocity.magnitude > this.rb.velocity.magnitude)
                    {
                        int difference = Mathf.Abs(Mathf.RoundToInt(attacker.rb.velocity.magnitude - rb.velocity.magnitude));
                        
                        Debug.Log(attacker.Username + " has collided with " + Username);
                        Debug.Log(attacker.Username + " : " + Mathf.Round(attacker.rb.velocity.magnitude) + " | " + Username + " : " + Mathf.Round(rb.velocity.magnitude)  + " | " + "Difference : " + Mathf.Abs(Mathf.Round(attacker.rb.velocity.magnitude - rb.velocity.magnitude)));
                        Debug.Log(attacker.Username + " : " + attacker.etatActuel.GetType() + " | " + Username + " : " + etatActuel.GetType());
                        if (difference > 3)
                        {
                            if(attacker.etatActuel.GetType() == typeof(EtatVoitureMouvement))
                            {
                                Debug.Log("STRIKE!");
                                Strike(attacker);
                            }
                        }
                    }
                }
            
        }
        else if (collision.gameObject.tag == "DirectionZone")
        {
            ChangeDirection(collision.gameObject.transform.forward);
        }
        else if (collision.gameObject.GetComponent<DeathZone>())
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
            //RespawnMethod();
            transform.rotation = Quaternion.Euler(Vector3.zero);
            rb.velocity = Vector3.zero;
            transform.position = GameObject.Find("Spawn").transform.position;
            ForceChangeDir(GameObject.Find("Spawn").transform.forward);
            lastHit = null;
            damage = 0;
        }
    }


    private void Strike(Player attackerCar)
    {
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
            direction = dir;
            StartCoroutine(changeDir());
        }
    }

    void ForceChangeDir(Vector3 forward)
    {
        direction = forward;
        transform.forward = forward;
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
        Spawn(fromClientId, message.GetString(), message.GetVector3(), message.GetVector3(), message.GetVector3());
    }

    public void SendStats()
    {
        Message message = Message.Create(MessageSendMode.Unreliable, ServerToClientId.stats);
        message.AddUShort(Id);
        message.AddInt(points);
        NetworkManager.Singleton?.Server.SendToAll(message);

    }

    public void SetInput(bool[] vs)
    {
        inputs = vs;
        if(thisaaccounttemp.accounttype == "Dev")
        {
            string t = "";
            for(int i = 0; i < inputs.Length; i++)
            {
                if (inputs[i])
                {
                    t += inputchar[i];
                    t += " ";
                }
                else
                {

                    t += "N";
                    t += " ";
                }
            }
            WriteAt($"User : {Username} INPUTS : {t}", 0, Id);
        }
    }
    private Message AddSpawnData(Message message)
    {
        message.AddUShort(Id);
        message.AddString(Username);
        message.AddVector3(transform.position); 
        message.AddQuaternion(GameManager.Singleton.SpawnPoint.transform.rotation);
        message.AddVector3(GetComponent<CarGraphics>().CarroserieColor);
        message.AddVector3(GetComponent<CarGraphics>().EmissionColor);
        message.AddVector3(GetComponent<CarGraphics>().RimsColor);
        return message;
    }
    private void SendSpawned()
    {

        NetworkManager.Singleton?.Server.SendToAll(AddSpawnData(Message.Create(MessageSendMode.Reliable, ServerToClientId.playerSpawned)));
    }

    private void SendSpawned(ushort toClientid)
    {

        NetworkManager.Singleton?.Server.Send(AddSpawnData(Message.Create(MessageSendMode.Reliable, ServerToClientId.playerSpawned)), toClientid);
    }
    private static void Spawn(ushort id, string username, Vector3 colorBody, Vector3 colorEmi, Vector3 colorRims)
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
        player.ChangerState(new EtatVoitureDebutPartie(player.gameObject));
        player.GetComponent<CarGraphics>().Set(colorBody, colorEmi, colorRims);
        player.SendSpawned();
        list.Add(id, player);
            NetworkManager.Singleton?.ConfirmAccountConnection(player);
       
    }
    private void SendMovement()
    {
        if(NetworkManager.Singleton?.CurrentTick % 2 != 0)
        {
            return;
        }
        Message message = Message.Create(MessageSendMode.Unreliable, ServerToClientId.playerMovement);
        message.AddUShort(Id);
        message.AddUInt(NetworkManager.Singleton.CurrentTick);
        message.AddVector3(transform.position);
        message.AddQuaternion(transform.rotation);
        message.AddQuaternion(modelCar.transform.rotation);
        message.AddVector3(rb.velocity);
        message.AddFloat(boostamount);
        NetworkManager.Singleton?.Server.SendToAll(message);
    }
}
