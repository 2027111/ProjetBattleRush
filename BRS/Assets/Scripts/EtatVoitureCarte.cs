using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EtatVoitureCarte : EtatVoiture
{
    public EtatVoitureCarte(GameObject joueur) : base(joueur)
    {
    }


    public EtatVoitureCarte(GameObject joueur, Player Player) : this(joueur)
    {

        
    }


    public override void Enter()
    {
    }

    public override void Exit()
    {

    }


    public override void Handle()
    {
    }
    
}