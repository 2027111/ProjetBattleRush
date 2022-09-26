using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour

{

    public static Dictionary<ushort, Player> list = new Dictionary<ushort, Player>();
    public ushort Id { get; set; }

    public bool IsLocal { get; set; }


    public string Username { get; set; }
    public bool[] inputs = new bool[7];//if necessary
    [SerializeField] public GameObject modelCar;
    [SerializeField] public GameObject attack;
    [SerializeField] public GameObject flightburst;
    [SerializeField] public Camera camProxy;
    [SerializeField] public GameObject camHolder;

    int points = 0;





    // Start is called before the first frame update
    void Start()
    { 
      
    }


    // Update is called once per frame
    void Update()
    {

     
    }
}
