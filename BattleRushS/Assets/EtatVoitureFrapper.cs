using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EtatVoitureFrapper : EtatVoiture
{
    Vector3 temp = new Vector3(0, 0, 0);

    Vector3 Rotation = Vector3.zero;
    float time = 0.3f;
    public EtatVoitureFrapper(GameObject joueur) : base(joueur)
    {
    }

    public EtatVoitureFrapper(GameObject joueur, Vector3 position, bool acceler) : this(joueur)
    {

        Vector3 diff = Voiture.transform.position - position;
        diff.Normalize();
        diff += Vector3.up;
        diff *= 0.15f;


        temp = diff;

    }

    public EtatVoitureFrapper(GameObject joueur, Player Player) : this(joueur)
    {

        Vector3 diff = Voiture.transform.position - Player.transform.position;
        diff.Normalize();
        diff += Vector3.up * 3;
        temp = diff;
        
    }


    public override void Enter()
    {
        Voiture.attack.SetActive(false);
        Voiture.flightburst.SetActive(true);
        Voiture.rb.AddForce(temp * 3, ForceMode.Impulse);
        if(temp == Vector3.zero)
        {
            Rotation = Vector3.one;
        }
        else
        {

            Rotation = new Vector3(Random.Range(0.1f, 1), Random.Range(0.1f, 1), Random.Range(0.1f, 1));
        }
    }

    public override void Exit()
    {
        Voiture.rb.constraints = RigidbodyConstraints.FreezeRotation;
        Voiture.gameObject.transform.forward = Voiture.direction;
        Voiture.flightburst.SetActive(false);
        Voiture.attack.SetActive(false);

    }


    public override void Handle()
    {
        time -= Time.deltaTime;
        if (Physics.BoxCast(Voiture.transform.position, Voiture.GetComponent<BoxCollider>().size / 2, Vector3.down, Voiture.transform.rotation, 0.51f, Voiture.lm) && time <= 0)
        {
            Voiture.modelCar.transform.forward = Voiture.gameObject.transform.forward;
            Voiture.ChangerState(new EtatVoitureMouvement(Voiture.gameObject));
        }
        else
        {
            Voiture.modelCar.transform.Rotate(Rotation * 10);
        }

        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        Vector3 vel = (Vector3.Normalize(Voiture.transform.forward * 4 * y + Voiture.transform.right * 4 * x));
        Voiture.rb.AddForce(vel);
    }
    
}