using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlwaysRotate : MonoBehaviour
{
    

    public Vector3 rotateps = new Vector3(0, 140, 0);
    bool rotate = true;

   

    void Update()
    {
        if (rotate)
        {
            transform.Rotate(rotateps * Time.deltaTime); //rotates 50 degrees per second around z axis
        }
    }
    void Start()
    {

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
        rotate = l;
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
            l += Time.deltaTime;
            transform.rotation = Quaternion.Euler(Vector3.Lerp(t, v, l));
            yield return null;
        }
        




        yield return null;
    }
}
