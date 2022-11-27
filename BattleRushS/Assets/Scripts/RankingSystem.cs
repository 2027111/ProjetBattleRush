using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RankingSystem
{ 

    public static float GetProbabilityWinning(float ratingP1, float ratingP2)
    {

        return 1f / (1f + Mathf.Pow(10f, (ratingP2 - ratingP1)) / 400f);

    }



    public static Vector2 UpdatePlayerRanking(float ratingPlayer1, float ratingPlayer2, float multiplier, bool isPlayer1Winner)
    {


        float probabilitywinplayer1 = GetProbabilityWinning(ratingPlayer1, ratingPlayer2);
        Debug.Log(probabilitywinplayer1.ToString());

        if (isPlayer1Winner)
        {
            Debug.Log("Player 1 modifyier : " + multiplier * (1 - probabilitywinplayer1));
            ratingPlayer1 += multiplier * (1 - probabilitywinplayer1);

            Debug.Log("Player 2 modifyier : " + multiplier * (probabilitywinplayer1 - 1));
            ratingPlayer2 += multiplier * (probabilitywinplayer1 - 1);

        }
        else
        {
            Debug.Log("Player 1 modifyier : " + multiplier * (-probabilitywinplayer1));
            ratingPlayer1 += multiplier * (-probabilitywinplayer1);


            Debug.Log("Player 2 modifyier : " + multiplier * probabilitywinplayer1);
            ratingPlayer2 += multiplier * probabilitywinplayer1;
        }


        return new Vector2(ratingPlayer1, ratingPlayer2);

    }


}
