using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EtatVoitureDebutPartie : EtatVoiture
{
    public EtatVoitureDebutPartie(GameObject joueur) : base(joueur)
    {
    }


    public EtatVoitureDebutPartie(GameObject joueur, Player Player) : this(joueur)
    {

        
    }


    public override void Enter()
    {
        Voiture.attack.SetActive(false);
        Voiture.attack.SetActive(false);
    }

    public override void Exit()
    {

    }


    public override void Handle()
    {
    }
    
}