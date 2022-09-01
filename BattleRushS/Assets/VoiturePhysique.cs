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
    [SerializeField] public Camera camProxy;
    [SerializeField] public LayerMask lm;
    [SerializeField] public bool control = false;

    public Rigidbody rb;

    EtatVoiture etatActuel;

    // Start is called before the first frame update
    void Start()
    { 
        rb = GetComponent<Rigidbody>();
        ChangerState(new EtatVoitureMouvement(this.gameObject));
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

            if(attacked != attack)
            {
                Strike(attackerCar.transform.position, (attackerCar.etatActuel as EtatVoitureMouvement).accelerating);
            }
        }
    }

    private void Strike(Vector3 position, bool accelerating)
    {
        Debug.Log(this.name + " was hit!");
        ChangerState(new EtatVoitureFrapper(this.gameObject, position, accelerating));
    }

    // Update is called once per frame
    void Update()
    {
        if (control)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                //Instantiate();
            }
        }
        etatActuel.Handle();
    }
}
