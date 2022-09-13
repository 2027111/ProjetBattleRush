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
        diff *= 0.15f;


        temp = diff;

    }

    public EtatVoitureFrapper(GameObject joueur, VoiturePhysique voiturePhysique) : this(joueur)
    {

        Vector3 diff = Voiture.transform.position - voiturePhysique.transform.position;
        diff.Normalize();
        diff *= 0.15f;
        diff += Vector3.up;


        temp = diff;
        
    }

    Vector3 Rotation = Vector3.zero;

    public override void Enter()
    {
        Voiture.attack.SetActive(false);
        Voiture.rb.AddForce(temp * 0.35f * Voiture.damage, ForceMode.Impulse);
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

        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        Vector3 vel = (Vector3.Normalize(Voiture.transform.forward * 4 * y + Voiture.transform.right * 4 * x));
        Voiture.rb.AddForce(vel);
    }
    
}