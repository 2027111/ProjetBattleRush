using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EtatVoitureMouvement : EtatVoiture
{


    public bool accelerating = false;
    public bool ralenting = false;
    public EtatVoitureMouvement(GameObject joueur) : base(joueur)
    {
    }

    public override void Enter()
    {
        Voiture.gameObject.transform.forward = Voiture.direction;

    }

    public override void Exit()
    {

    }

    


    public override void Handle()
    {
        float accel = 1;
        float x = 0;
        if (Voiture.control)
        {


        if (Input.GetKey(KeyCode.W))
        {
            accel = 1.4f;
            accelerating = true; ralenting = false;

        }
        else if (Input.GetKey(KeyCode.S)){
            accel = 0.5f;
            accelerating = false; ralenting = true;
        }
        else
        {
            accel = 1;
            accelerating = false; ralenting = false;
        }


        x = Input.GetAxis("Horizontal");


        }
        Voiture.rb.velocity = (Vector3.Normalize(Voiture.direction) * Voiture.speed * accel) + (Voiture.transform.right * 3 * x);


    }

}
