using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarteJoueur : IComparable<CarteJoueur>
{
    public string Username;
    public int Points;

    public CarteJoueur(string username, int points)
    {
        Username = username;
        Points = points;
    }

   

    public int CompareTo(CarteJoueur p2)
    {
        return this.Points.CompareTo(p2.Points);
    }
}
