using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreBoardOrder : MonoBehaviour
{
   public void OrderChildren()
    {
        List<GameObject> tempList = new List<GameObject>();


        foreach(Transform child in transform)
        {
            tempList.Add(child.gameObject);
        }


        tempList.Sort((p1, p2) => p1.GetComponent<ScoreBoardCard>().returnPoints().CompareTo(p2.GetComponent<ScoreBoardCard>().returnPoints()));

        foreach(GameObject c in tempList)
        {
            c.transform.SetSiblingIndex(tempList.IndexOf(c));
        }

        Debug.Log("Sorting ScoreBoard");


    }
}
