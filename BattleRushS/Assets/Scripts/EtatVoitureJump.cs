using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EtatVoitureJump : EtatVoiture
{
    float time = 0.3f;
    public EtatVoitureJump(GameObject joueur) : base(joueur)
    {
    }




    public override void Enter()
    {
        Voiture.attack.SetActive(false);
        Voiture.flightburst.SetActive(false);
    }

    public override void Exit()
    {
        Voiture.flightburst.SetActive(false);
        Voiture.attack.SetActive(false);

    }


    public override void Handle()
    {
        time -= Time.deltaTime;
        if (Physics.BoxCast(Voiture.transform.position,Voiture.GetComponent<BoxCollider>().size/2, Vector3.down, Voiture.transform.rotation,  0.51f, Voiture.lm) && time <= 0)
        {
            Voiture.modelCar.transform.forward = Voiture.gameObject.transform.forward;
            Voiture.ChangerState(new EtatVoitureMouvement(Voiture.gameObject));
        }

    }
    
}