using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;


public class MenuPreset : MonoBehaviour
{

    [SerializeField] public Transform PosOnActive;
    [SerializeField] public Transform PosOffActive;
    public bool slideorappear = false; //false = slide, true = appear;
    public bool outte = false;
    public Transform targetpos;
    public Transform startpos;
    Vector2 targetSize;
    Vector2 startSize;
    Vector2 baseSize =new Vector2(1600, 900);
    float t;

    // Start is called before the first frame update
    void Start()
    {

        if (slideorappear)
        {

            baseSize = gameObject.GetComponent<RectTransform>().sizeDelta;
            if (!outte)
        {
                startSize = Vector2.zero;
                targetSize = Vector2.zero;
        }
        else
        {

                startSize = baseSize;
                targetSize = baseSize;
        }

        }
        else
        {
            if (!outte)
            {

                startpos = PosOffActive;
                targetpos = PosOffActive;
            }
            else
            {

                startpos = PosOnActive;
                targetpos = PosOnActive;
            }

        }


    }




    // Update is called once per frame
    void Update()
    {
        if (slideorappear)
        {
            if(targetSize != gameObject.GetComponent<RectTransform>().sizeDelta)
            {

                t += Time.deltaTime / 0.3f;
                gameObject.GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(startSize, targetSize, t);
            }
        }
        else
        {
        if (targetpos != null)
        {
            if (transform.position != targetpos.position)
            {
                t += Time.deltaTime / 0.3f;
                transform.position = Vector2.Lerp(startpos.position, targetpos.position, t);
                
            }
        }

        }

    }


    public virtual void OnHoverExit()
    {
        if (slideorappear)
        {

            if (outte == false)
            {
                return;
            }
                t = 0;
                startSize = baseSize;
                targetSize = Vector2.zero;
                outte = false;
           
        }
        else
        {
            if (outte == false)
            {
                return;
            }
            t = 0;
            startpos = PosOnActive;
            targetpos = PosOffActive;
            outte = false;
        }

    }

    public virtual void Activate()
    {
       

        t = 0;
        if (slideorappear)
        {

            if (outte == true)
            {
                OnHoverExit();

            }
            else
            {
                startSize = Vector2.zero;
                targetSize = baseSize;
                outte = true;
            }
        }
        else
        {
            if (outte == true)
            {
                OnHoverExit();

            }
            else
            {
                startpos = PosOffActive;
                targetpos = PosOnActive;
                outte = true;
            }
        }
    }
}
