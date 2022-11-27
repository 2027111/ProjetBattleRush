using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugStateChanger : MonoBehaviour
{


    Player car;
    Type[] types = { typeof(EtatVoitureMouvement), typeof(EtatVoitureFrapper), typeof(EtatVoitureJump) };

    // Start is called before the first frame update
    void Start()
    {
        car = GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnGUI()
    {
        GUI.Box(new Rect(10, 170, 150, 280), "DebugControls");
        
        if(GUI.Button(new Rect(24, 210, 126, 25), "MouvementState"))
        {
            car.ChangerState(new EtatVoitureMouvement(car.gameObject));
        }
        if (GUI.Button(new Rect(24, 240, 126, 25), "FrapperState"))
        {
            car.ChangerState(new EtatVoitureFrapper(car.gameObject));
        }
        if (GUI.Button(new Rect(24, 270, 126, 25), "JumpState"))
        {
            car.ChangerState(new EtatVoitureJump(car.gameObject));
        }
        if (GUI.Button(new Rect(24, 300, 126, 25), "DebutState"))
        {
            car.ChangerState(new EtatVoitureDebutPartie(car.gameObject));
        }
        if (GUI.Button(new Rect(24, 330, 126, 25), "MortState"))
        {
            car.ChangerState(new EtatVoitureMort(car.gameObject));
        }


        for (int i = 0; i < 3; i++)
        {
            for(int j = 0; j<3; j++)
            {

                if (GUI.Button(new Rect(20 + (j * 45), 360 + (i * 30), 45, 25), ""+ ((i*3)+j)))
                {
                    car.speed = (i * 3) + j;
                }
            }
        }

    }


}
