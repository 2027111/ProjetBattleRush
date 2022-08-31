using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiturePhysique : MonoBehaviour
{
    [SerializeField] public float speed = 10;
    [SerializeField] public Vector3 direction = new Vector3(0, 0, 1);
    [SerializeField] public GameObject modelCar;

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


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<VoiturePhysique>())
        {
            VoiturePhysique touchedCar = collision.gameObject.GetComponent<VoiturePhysique>();


            if(touchedCar != this)
            {
                if(touchedCar.etatActuel.GetType() == typeof(EtatVoitureMouvement))
                {

                if(etatActuel.GetType() == typeof(EtatVoitureMouvement))
                {
                    if(!(etatActuel as EtatVoitureMouvement).ralenting)
                    {
                        touchedCar.Strike(transform.position, (etatActuel as EtatVoitureMouvement).accelerating);

                    }

                    
                    
                }

                }
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
        etatActuel.Handle();
    }
}
