using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowLocalPlayer : MonoBehaviour
{
    public static GameObject FollowedPlayer;


    public static void SetPlayer(GameObject p)
    {
        FollowedPlayer = p;
    }


    private void Update()
    {
        if (FollowedPlayer)
        {
            transform.position = new Vector3(FollowedPlayer.transform.position.x, transform.position.y, FollowedPlayer.transform.position.z);
        }
    }

}
