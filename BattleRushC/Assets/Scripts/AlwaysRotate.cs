using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlwaysRotate : MonoBehaviour
{

    public float rotateps = 140;


    void Update()
    {
        transform.Rotate(0, 0, rotateps * Time.deltaTime); //rotates 50 degrees per second around z axis
    }


    public void Setactive()
    {
        this.gameObject.SetActive(true);
    }
}
