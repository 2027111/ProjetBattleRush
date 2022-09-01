using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EtatVoitureFrapper : EtatVoiture
{
    Vector3 temp = new Vector3(0, 0, 0);

    float time = 0.3f;
    public EtatVoitureFrapper(GameObject joueur) : base(joueur)
    {
    }

    public EtatVoitureFrapper(GameObject joueur, Vector3 position, bool acceler) : this(joueur)
    {

        Vector3 diff = Voiture.transform.position - position;
        diff.Normalize();
        diff += Vector3.up;
        diff *= 0.65f;
        if (acceler)
        {
            diff *= 1.1f;
        }


        temp = diff;


    }

    Vector3 Rotation = Vector3.zero;

    public override void Enter()
    {
        Voiture.attack.SetActive(false);
        Voiture.rb.constraints = RigidbodyConstraints.None;
        Voiture.rb.AddForce(temp * 10, ForceMode.Impulse);
        Rotation = new Vector3(Random.Range(0.5f, 2), Random.Range(0.5f, 2), Random.Range(0.5f, 2));
    }

    public override void Exit()
    {
        Voiture.rb.constraints = RigidbodyConstraints.FreezeRotation;
        Voiture.gameObject.transform.forward = Voiture.direction;

    }


    public override void Handle()
    {
        time -= Time.deltaTime;
        if(Physics.Raycast(Voiture.transform.position,Vector3.down, out RaycastHit hit,  0.51f, Voiture.lm) && time <= 0)
        {
            Voiture.modelCar.transform.forward = Voiture.gameObject.transform.forward;
            Voiture.ChangerState(new EtatVoitureMouvement(Voiture.gameObject));
        }
        else
        {
            Voiture.modelCar.transform.Rotate(Rotation * 10);
        }
    }
    
}