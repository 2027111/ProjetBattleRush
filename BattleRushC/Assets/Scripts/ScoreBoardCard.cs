using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBoardCard : MonoBehaviour
{

    [SerializeField] Text usernameText;
    [SerializeField] Text pointsText;
    [SerializeField] Text positionText;

    [SerializeField] public Player playerInfo;
    bool playerExisted = false;

    private void Start()
    {

        transform.parent.GetComponent<ScoreBoardOrder>().OrderChildren();
    }
    private void Update()
    {
        if (!playerInfo)
        {
            if (playerExisted)
            {
                pointsText.text = "Disconnected";
            }
            else if(!playerExisted)
            {
                Destroy(gameObject);
            }
        }
    }

    public void Affiliate(Player thisplayer)
    {
        playerInfo = thisplayer;
        usernameText.text = playerInfo.Username;
        playerExisted = true;
        if(playerInfo.Id == NetworkManager.Singleton.Client.Id)
        {
            gameObject.GetComponent<Image>().color = new Color32(255, 162, 0, 255);
        }
        playerInfo.EvenementHandler += ReplacePoints;
    }

    public int returnPoints()
    {
        return playerInfo.points;
    }
    public void ReplacePoints()
    {
        pointsText.text = "" + playerInfo.points;
        transform.parent.GetComponent<ScoreBoardOrder>().OrderChildren();
    }
    public void SetPosition()
    {
        positionText.text = ""+(transform.GetSiblingIndex());
    }
}
