using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EtatVoitureMort : EtatVoiture
{
    public EtatVoitureMort(GameObject joueur) : base(joueur)
    {

    }


    public override void Enter()
    {
        Voiture.gameObject.layer = 9; 
        Voiture.StartCoroutine(changeView());
        Voiture.transform.position = GameObject.Find("Spawn").transform.position;
        Voiture.ForceChangeDir(GameObject.Find("Spawn").transform.forward);

    }

    public override void Exit()
    {
        Voiture.gameObject.layer = 0;
    }


    public override void Handle()
    {

    }

    public IEnumerator changeView()
    {
        for (int i = 0; i <7; i++)
        {

            yield return new WaitForSecondsRealtime(0.25f);
            Voiture.modelCar.SetActive(!Voiture.modelCar.activeSelf);
        }
        Voiture.modelCar.SetActive(true);
        Voiture.ChangerState(new EtatVoitureMouvement(Voiture.gameObject));
        yield break;
        
    }

}