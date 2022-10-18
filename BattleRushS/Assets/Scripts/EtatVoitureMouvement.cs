using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EtatVoitureMouvement : EtatVoiture
{


    public bool accelerating = false;
    public bool ralenting = false;
    float accel = 1;
    float tacceltime = 0.25f;
    float timepassed = 0;
    public EtatVoitureMouvement(GameObject joueur) : base(joueur)
    {
    }

    public override void Enter()
    {
        Voiture.lastHit = null;
        Voiture.flightburst.SetActive(false);
        Voiture.attack.SetActive(false);
        timepassed = 0;


    }

    public override void Exit()
    {
        Voiture.flightburst.SetActive(false);
        Voiture.attack.SetActive(false);

    }

    


    public override void Handle()
    {
        timepassed += Time.deltaTime;
        if (!Physics.BoxCast(Voiture.transform.position, Voiture.GetComponent<BoxCollider>().size / 2, Vector3.down, Voiture.transform.rotation, 0.51f, Voiture.lm))
        {
            Voiture.attack.SetActive(false);
            Voiture.ChangerState(new EtatVoitureJump(Voiture.gameObject));

        }
        float x = 0;
        if (Voiture.control)
        {


        if (Voiture.inputs[0] && Voiture.boostamount > 0 && Voiture.canboost)
        {

                Voiture.boostamount -=  30 * Time.deltaTime;
                if(Voiture.boostamount<= 0)
                {
                    Voiture.boostamount = 0.001f;
                    Voiture.canboost = false;
                }
                if(accel < 2)
                {
                    accel += Time.deltaTime;
                }
                else
                {

                    accel = 2;
                }
            accelerating = true; ralenting = false;

        }
        else if (Voiture.inputs[2])
            {
            if(accel > 0.5f)
                {
                    accel -= Time.deltaTime;
                }
                else
                {
                    accel = 0.5f;
                }
            accelerating = false; ralenting = true;


        }
        else
        {

                if (accel != 1)
                {
                    accel += Mathf.Sign(1 - accel) * Time.deltaTime;
                }
               
                accelerating = false; ralenting = false;
        }

            if (Voiture.boostamount < 100 && !accelerating)
            {

                Voiture.boostamount += 20 * Time.deltaTime;
            }else if(Voiture.boostamount > 100)
            {
                Voiture.boostamount = 100;
                Voiture.canboost = true;
            }


            x = 0;

            if (Voiture.inputs[1])
            {
                x -= 0.4f;
            }
            if(Voiture.inputs[3]){
                x += 0.4f;
            }


            if (Voiture.inputs[5])
            {
                Voiture.ChangerState(new EtatVoitureJump(Voiture.gameObject));
                Voiture.rb.AddForce(Voiture.transform.up * 7, ForceMode.Impulse);
            }
        }
        else
        {

            accel = 1.4f;
            accelerating = true; ralenting = false;
        }


        Vector3 vel = (Vector3.Normalize(Voiture.transform.forward) * Voiture.speed * accel) + (Voiture.transform.right * Voiture.speed/2 * accel * x);
        if(timepassed/tacceltime < 1)
        {
            vel *= timepassed / tacceltime;
        }

  
        Voiture.rb.velocity = new Vector3(vel.x, Voiture.rb.velocity.y, vel.z);

        Vector3 dir = Voiture.rb.velocity;
        dir.y = 0;
        dir.Normalize();
        float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        Voiture.modelCar.transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);

        Vector3 rotation = Voiture.modelCar.transform.rotation.eulerAngles;
        Voiture.attack.transform.rotation = Quaternion.Euler(90, 0, -rotation.y);

        if (accelerating)
        {
            Voiture.attack.SetActive(true);

            

        }
        else
        {
            Voiture.attack.SetActive(false);
            
        }

    }

}
