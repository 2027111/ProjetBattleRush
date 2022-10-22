using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayGif : MonoBehaviour
{

    [SerializeField] Sprite[] sprites;
    float t = 0;
    int index = 0;
    [SerializeField] float ImagePerSeconds = 1;
    float imageRate = 1;
    // Start is called before the first frame update
    void Start()
    {

        GetComponent<Image>().sprite = sprites[index];
        imageRate = 1 / ImagePerSeconds;
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime; 
        if(t >= imageRate)
        {
            t = -0;
            GetComponent<Image>().sprite = nextSprite();
        }
    }

    private Sprite nextSprite()
    {
        index++;
        if(index >= sprites.Length)
        {
            index = 0;
            
        }
        return sprites[index];
    }
}
