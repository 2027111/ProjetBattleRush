using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiturePhysique : MonoBehaviour

{
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
    public VoiturePhysique lastHit = null;
    public float boostamount = 100;

    int points = 0;


    public Rigidbody rb;

    EtatVoiture etatActuel;
    public static Dictionary<ushort, VoiturePhysique> list = new Dictionary<ushort, VoiturePhysique>();

    public bool GetAccel()
    {
        return (etatActuel as EtatVoitureMouvement).accelerating;
    }


    // Start is called before the first frame update
    void Start()
    { 
        rb = GetComponent<Rigidbody>();
        ChangerState(new EtatVoitureMouvement(this.gameObject));
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
            VoiturePhysique attackerCar = attacked.gameObject.transform.parent.gameObject.GetComponent<VoiturePhysique>();
                if (attacked != attack)
                {
                float d = UnityEngine.Random.Range(4, 9);
                damage += d;
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
            }
            else
            {

                points -= collision.gameObject.GetComponent<DeathZone>().suicidePenality;
            }

            transform.rotation = Quaternion.Euler(Vector3.zero);
            rb.velocity = Vector3.zero;
            transform.position = GameObject.Find("Spawn").transform.position;
            damage = 0;
        }
    }


    private void Strike(VoiturePhysique attackerCar)
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
            if (Input.GetKeyDown(KeyCode.Space))
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
    }
}
