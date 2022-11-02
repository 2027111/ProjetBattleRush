using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class LoadingText : MonoBehaviour
{

    [SerializeField] string[] strings;
    float t = 0;
    int index = 0;
    [SerializeField] float TextPerSeconds = 1;
    float textRate = 1;

    // Start is called before the first frame update
    void Start()
    {
        textRate = 1 / TextPerSeconds;
        GetComponent<Text>().text = strings[index];

    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;
        if (t >= textRate)
        {
            t = -0;
            GetComponent<Text>().text = nextString();
        }
    }

    private string nextString()
    {
        index++;
        if (index >= strings.Length)
        {
            index = 0;

        }
        return strings[index];
    }
}
