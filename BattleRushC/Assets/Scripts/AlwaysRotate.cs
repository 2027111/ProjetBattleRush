using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlwaysRotate : MonoBehaviour
{

    public Vector3 rotateps = new Vector3(0, 140, 0);

    void Update()
    {
        transform.Rotate(rotateps * Time.deltaTime); //rotates 50 degrees per second around z axis
    }

    public void SetToYRot(int i)
    {
        StartCoroutine(RotateTo(new Vector3(0, i, 0)));
    }
    public void SetToZRot(int i)
    {
        StartCoroutine(RotateTo(new Vector3(0, 0, i)));
    }
    public void SetToXRot(int i)
    {
        StartCoroutine(RotateTo(new Vector3(i, 0, 0)));
    }

    public void Lockit(bool l)
    {
        enabled = l;
    }
    public void Setactive()
    {
        this.gameObject.SetActive(true);
    }




    IEnumerator RotateTo(Vector3 v)
    {
        Vector3 t = gameObject.transform.rotation.eulerAngles;

        float l = 0;
        while(l < 1)
        {
            Vector3.Slerp(t, v, l);
            l += Time.deltaTime;
        }
        




        yield return null;
    }
}
