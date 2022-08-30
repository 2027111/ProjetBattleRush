using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EtatVoitureMouvement : EtatVoiture
{
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

        Voiture.rb.velocity = (Vector3.Normalize(Voiture.direction) * Voiture.speed);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Voiture.ChangerState(new EtatVoitureFrapper(Voiture.gameObject));
        }
    }

}
