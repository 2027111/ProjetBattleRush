using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLocks : MonoBehaviour
{
    [SerializeField] Vector3[] positions;

    public void LockIt(int i)
    {
        StartCoroutine(RotateTo(positions[i]));
    }

    IEnumerator RotateTo(Vector3 v)
    {
        Vector3 t = gameObject.transform.rotation.eulerAngles;

        float l = 0;
        while (l < 1)
        {
            l += Time.deltaTime;
            transform.rotation = Quaternion.Euler(Vector3.Lerp(t, v, l));
            yield return null;
        }





        yield return null;
    }
}
