using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextAnimator : MonoBehaviour
{
    [SerializeField] float scaleUpgradePerSecond;
    [SerializeField] Vector3 movementPerSecond;
    [SerializeField] float timeTillDestroy = 1;
    float timetillfade = 1;
    [SerializeField] Text text;

    bool destroy = true;

    private void Start()
    {
        timetillfade = timeTillDestroy / 2;
    }
    // Update is called once per frame
    void Update()
    {
        if(destroy)
        {

            timeTillDestroy -= Time.deltaTime;
            GetComponent<CanvasGroup>().alpha = (timeTillDestroy / timetillfade);
            if(timeTillDestroy <= 0)
            {
                Destroy(gameObject);
            }

            transform.localScale = transform.localScale +  (transform.localScale * scaleUpgradePerSecond * Time.deltaTime);

            transform.position = transform.position + (movementPerSecond * Time.deltaTime);

        }
    }
    public void SetText(string t)
    {
        text.text = t;
    }
}
