using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarteJoueur : IComparer<CarteJoueur>
{
    public string Username;
    public int Points;

    public CarteJoueur(string username, int points)
    {
        Username = username;
        Points = points;
    }

    public static int Sort(CarteJoueur p1, CarteJoueur p2)
    {
        return p1.Points.CompareTo(p2.Points);
    }

    public int Compare(CarteJoueur p1, CarteJoueur p2)
    {
        return p1.Points.CompareTo(p2.Points);
    }
}
