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


    // Update is called once per frame
    void Update()
    {
        etatActuel.Handle();
    }
}
