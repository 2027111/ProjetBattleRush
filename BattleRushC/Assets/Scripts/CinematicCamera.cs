using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicCamera : MonoBehaviour
{

    [SerializeField] Transform[] pos;
    bool active = true;
    float t = 0;
    int index = 0;
    [SerializeField] float shotPanSpeed = 2;
    [SerializeField] float ShotsPerSeconds = 1;
    Vector3 dir;
    [SerializeField] static Camera cam;
    float shotRate = 1;
    public static bool trans = false;

    // Start is called before the first frame update
    void Start()
    {
        cam = this.GetComponent<Camera>();
        trans = false;
        shotRate = 1 / ShotsPerSeconds;
        cam.transform.position = pos[index].position;
        cam.transform.forward = pos[index].forward;

    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {

        t += Time.deltaTime;
        if (t >= shotRate)
        {
            t = -0;
            nextPos();
        }

        cam.transform.position = cam.transform.position += cam.transform.right * shotPanSpeed * Time.deltaTime;

        }

    }

    private void nextPos()
    {
        index++;
        if (index >= pos.Length)
        {
            index = 0;

        }
        cam.transform.position = pos[index].position;
        cam.transform.forward = pos[index].forward;
    }

    public static void Transition()
    {
        if (!trans)
        {
            trans = true; 
            cam.GetComponent<CinematicCamera>().StartCoroutine(cam.GetComponent<CinematicCamera>().TransitionIntoMain());

        }
    }

    IEnumerator TransitionIntoMain()
    {
        active = false;
        float time = 0;
        Vector3 departPos = cam.transform.position;
        Quaternion departRot = cam.transform.rotation;
        while (time < 1)
        {
            cam.transform.position = Vector3.Slerp(departPos, Camera.main.transform.position, time);
            cam.transform.rotation = Quaternion.Slerp(departRot, Camera.main.transform.rotation, time);
            time += Time.deltaTime;
            yield return null;
        }
        cam.depth = -9;

    }
}
