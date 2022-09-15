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
        Voiture.flightburst.SetActive(false);
        Voiture.attack.SetActive(false);

    }

    public override void Exit()
    {
        Voiture.flightburst.SetActive(false);
        Voiture.attack.SetActive(false);

    }

    


    public override void Handle()
    {
        if (!Physics.BoxCast(Voiture.transform.position, Voiture.GetComponent<BoxCollider>().size / 2, Vector3.down, Voiture.transform.rotation, 0.51f, Voiture.lm))
        {
            Voiture.attack.SetActive(false);
            Voiture.ChangerState(new EtatVoitureFrapper(Voiture.gameObject));
        }
        float accel = 1;
        float x = 0;
        if (Voiture.control)
        {


        if (Input.GetKey(KeyCode.W) && Voiture.boostamount > 0)
        {

                Voiture.boostamount -=  30 * Time.deltaTime;
                accel = 1.4f;
            accelerating = true; ralenting = false;

        }
        else if (Input.GetKey(KeyCode.S)){
            accel = 0.5f;
            accelerating = false; ralenting = true;
        }
        else
        {
            Voiture.boostamount += 20 * Time.deltaTime;
            accel = 1;
            accelerating = false; ralenting = false;
        }


        x = Input.GetAxis("Horizontal");


            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Voiture.ChangerState(new EtatVoitureJump(Voiture.gameObject));
            }
        }
        else
        {

            accel = 1.4f;
            accelerating = true; ralenting = false;
        }


        Vector3 vel = (Vector3.Normalize(Voiture.transform.forward) * Voiture.speed * accel) + (Voiture.transform.right * 3 * (Voiture.speed / 4) * accel * x);

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
            float temp = Voiture.camProxy.fieldOfView;
            if (temp < 80)
            {
                temp += Time.deltaTime * 25;
                Voiture.camProxy.fieldOfView = temp;
            }
            else
            {
                Voiture.camProxy.fieldOfView = 80;

            }

        }
        else
        {
            Voiture.attack.SetActive(false);
            float temp = Voiture.camProxy.fieldOfView;
            if (temp > 60)
            {
                temp -= Time.deltaTime * 25;
                Voiture.camProxy.fieldOfView = temp;
            }
            else
            {

                Voiture.camProxy.fieldOfView = 60;
            }
        }

    }

}
