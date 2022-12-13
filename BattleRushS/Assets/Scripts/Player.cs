using Riptide;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour

{
    public Check lastCheck;
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
    public bool isReady = false;
    public bool HasLoadedGameScene = false;
    public int points = 0;


    public Rigidbody rb;

    public bool Jump;
    public bool Switch;
    public Vector2 forceDirection = Vector2.zero;

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
        NetworkManager.Singleton.OnPlayerReady();
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
        lastCheck = new Check(transform.position, transform.forward);
    }

    public void SpawnEveryone()
    {
        SendConnected(ServerToClientId.playerSpawned);
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
    [MessageHandler((ushort)ClientToServerId.ready)]
    private static void Ready(ushort fromClientId, Message message)
    {
        if (list.TryGetValue(fromClientId, out Player player))
        {
            player.isReady = !player.isReady;
            NetworkManager.Singleton.OnPlayerReady();
        }
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
                        
                        //Debug.Log(attacker.Username + " has collided with " + Username);
                        //Debug.Log(attacker.Username + " : " + Mathf.Round(attacker.rb.velocity.magnitude) + " | " + Username + " : " + Mathf.Round(rb.velocity.magnitude)  + " | " + "Difference : " + Mathf.Abs(Mathf.Round(attacker.rb.velocity.magnitude - rb.velocity.magnitude)));
  ;                      //Debug.Log(attacker.Username + " : " + attacker.etatActuel.GetType() + " | " + Username + " : " + etatActuel.GetType());
                        if (difference > 3)
                        {
                            if(attacker.etatActuel.GetType() == typeof(EtatVoitureMouvement))
                            {
                                Strike(attacker);
                            }
                        }
                    }
                }
            
        }
        else if (collision.gameObject.tag == "DirectionZone")
        {
            ChangeDirection(collision.gameObject);
        }
        else if (collision.gameObject.GetComponent<DeathZone>())
        {
            if (lastHit)
            {
                lastHit.AddPoints(collision.gameObject.GetComponent<DeathZone>().pointsByKills);
                lastHit.SendStats();

            }
            else
            {

                AddPoints(-collision.gameObject.GetComponent<DeathZone>().suicidePenality);
                SendStats();
            }
            //RespawnMethod();
            transform.rotation = Quaternion.Euler(Vector3.zero);
            rb.velocity = Vector3.zero;
            ChangerState(new EtatVoitureMort(gameObject));
            lastHit = null;
            damage = 0;
        }
    }


    public void AddPoints(int amount)
    {
        points += amount;
        NetworkManager.Singleton?.SetJoueurPoint(Username, points);
    }


    private void Strike(Player attackerCar)
    {
        lastHit = attackerCar;
        ChangerState(new EtatVoitureFrapper(this.gameObject, attackerCar));
    }
    public void ChangeDirection(GameObject dir)
    {
        if(direction == dir.transform.forward)
        {
            return;
        }
        else
        {
            direction = dir.transform.forward;
            StartCoroutine(changeDir(dir));
        }
    }

    public void ForceChangeDir(Vector3 forward)
    {
        direction = forward;
        transform.forward = forward;
    }

    IEnumerator changeDir(GameObject check)
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
        lastCheck = new Check(check.transform.position, check.transform.forward);


        yield return null;
    }

    // Update is called once per frame
    void Update()
    {

        etatActuel.Handle();
    }


    [MessageHandler((ushort)ClientToServerId.input)]
    private static void Input(ushort fromClientId, Message message)
    {
        if (list.TryGetValue(fromClientId, out Player player))
        {
            player.SetInput(message.GetVector2(), message.GetBool(), message.GetBool());
        }
    }

    private void SetInput(Vector2 direction, bool jump, bool dirswitch)
    {
        forceDirection = direction;
        Jump = jump;
        Switch = dirswitch;
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

    private void SendConnected(ServerToClientId type)
    {
        Debug.Log("Spawn");
        NetworkManager.Singleton?.Server.SendToAll(AddSpawnData(Message.Create(MessageSendMode.Reliable, type)));
    }

    private void SendConnected(ushort toClientid, ServerToClientId type)
    {

        NetworkManager.Singleton?.Server.Send(AddSpawnData(Message.Create(MessageSendMode.Reliable, type)), toClientid);
    }
    private static void Spawn(ushort id, string username, Vector3 colorBody, Vector3 colorEmi, Vector3 colorRims)
    {
        foreach (Player otherPlayer in list.Values)
        {
            otherPlayer.SendConnected(id, ServerToClientId.playerConnected);
        }

        Vector3 pos = GameManager.Singleton.SpawnPoint.transform.position;
        pos += GameManager.Singleton.SpawnPoint.transform.right * 2 * (NetworkManager.Singleton.Server.ClientCount - 1);
        Player player = Instantiate(GameManager.Singleton.PlayerPrefab, pos, GameManager.Singleton.SpawnPoint.transform.rotation).GetComponent<Player>();
       
        player.name = $"Player {id} {(string.IsNullOrEmpty(username) ? "Guest" : username)}";
        player.Id = id;
        player.Username = string.IsNullOrEmpty(username) ? $"Guest {id}" : username;
        player.ChangerState(new EtatVoitureCarte(player.gameObject));
        player.GetComponent<CarGraphics>().Set(colorBody, colorEmi, colorRims);
        player.SendConnected(ServerToClientId.playerConnected);
        list.Add(id, player);
        NetworkManager.Singleton.OnPlayerReady();
        NetworkManager.Singleton?.ConfirmAccountConnection(player);
       
    }
    public void SendReady()
    {

        Message message = Message.Create(MessageSendMode.Reliable, ServerToClientId.playerisReady);
        message.AddUShort(Id);
        message.AddBool(isReady);
        Debug.Log(isReady);
        NetworkManager.Singleton?.Server.SendToAll(message);
    }

    public void SendMovement()
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

    public void carRespawn()
    {
        ForceChangeDir(lastCheck.orientation);
        transform.position = lastCheck.position;
        //modelCar.transform.forward = lastCheck.orientation;
    }

  
}
