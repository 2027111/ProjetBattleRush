using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EtatVoiture
{


    protected Player Voiture
    {
        get;
        set;
    }

    protected Animator Animateur
    {
        set;
        get;
    }

    public System.Type GetRealState()
    {
        return this.GetType();
    }
    public EtatVoiture(GameObject joueur)
    {
        Voiture = joueur.GetComponent<Player>();
        Animateur = joueur.GetComponent<Animator>();
    }

    public abstract void Enter();
    public abstract void Handle();
    public abstract void Exit();
}
