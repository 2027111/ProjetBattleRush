using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugControls : MonoBehaviour
{

    private bool[] inputs;//WASDSPACES
    private bool[] down = { false, false, false, false, true, false };
    private KeyCode[] inp = { KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.Space, KeyCode.LeftShift };
    


    // Start is called before the first frame update
    void Start()
    {
        inputs = new bool[inp.Length];
        //   hold[8] = false;
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < inputs.Length; i++)
        {

            inputs[i] = down[i] ? Input.GetKeyDown(inp[i]) : Input.GetKey(inp[i]);

        }


        if (inputs[4])
        {
        }

        /*
        else
        {


            Vector2 dir = js.Direction;

            if (dir.y > 0.5f)
            {
                inputs[0] = true;
            }
            if (dir.x < -0.5f)
            {

                inputs[1] = true;
            }
            if (dir.y < 0)
            {

                inputs[2] = true;
            }
            if (dir.x > 0.5f)
            {

                inputs[3] = true;
            }

            if (Light.buttonPressed)
            {
                inputs[4] = true;

            }

            if (Heavy.buttonPressed)
            {


                inputs[5] = true;
            }

            if (Special.buttonPressed)
            {

                inputs[6] = true;
            }

        }*/

        GetComponent<Player>().SetInput(inputs);

    }




}
