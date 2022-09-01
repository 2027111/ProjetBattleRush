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
        Voiture.attack.SetActive(false);

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
        Voiture.rb.velocity = (Vector3.Normalize(Voiture.direction) * Voiture.speed * accel) + (Voiture.transform.right * 3 * (Voiture.speed/4) * accel *x);

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
                temp += Time.deltaTime * 15;
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
                temp -= Time.deltaTime * 15;
                Voiture.camProxy.fieldOfView = temp;
            }
            else
            {

                Voiture.camProxy.fieldOfView = 60;
            }
        }

    }

}
