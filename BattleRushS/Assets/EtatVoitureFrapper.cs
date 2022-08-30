using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EtatVoitureFrapper : EtatVoiture
{
    public EtatVoitureFrapper(GameObject joueur) : base(joueur)
    {
    }

    Vector3 Rotation = Vector3.zero;
    public override void Enter()
    {
        Vector3 temp = new Vector3(Random.Range(-2, 2), 1, Random.Range(0, 2));
        Voiture.rb.AddForce(temp * 10, ForceMode.Impulse);
        Rotation = new Vector3(Random.Range(0.5f, 2), Random.Range(0.5f, 2), Random.Range(0.5f, 2));
    }

    public override void Exit()
    {
        Voiture.gameObject.transform.forward = Voiture.direction;

    }


    public override void Handle()
    {
        if(Physics.Raycast(Voiture.transform.position,Vector3.down, out RaycastHit hit,  0.51f, Voiture.lm))
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