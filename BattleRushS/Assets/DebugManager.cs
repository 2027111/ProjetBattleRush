using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugManager : MonoBehaviour
{
    [Header("Debug")]
    GameObject debugPlayer;
    List<GameObject> debugs = new List<GameObject>();

    private void OnGUI()
    {
            GUI.Box(new Rect(10, 10, 150, 150), "DebugControls");


            if (GUI.Button(new Rect(24, 40, 126, 25), "ToggleDebugCar"))
            {
                if (debugPlayer)
                {
                    Destroy(debugPlayer);
                }
                else
                {
                    debugPlayer = Instantiate(GameManager.Singleton.PlayerPrefab, GameManager.Singleton.SpawnPoint.transform.position, GameManager.Singleton.SpawnPoint.transform.rotation);
                    debugPlayer.AddComponent<DebugControls>();
                    debugPlayer.GetComponent<Player>().ChangerState(new EtatVoitureMouvement(debugPlayer));
                    debugPlayer.GetComponent<Player>().SetProfile("Debug");
                    Instantiate(GameManager.Singleton.CamPrefab, debugPlayer.transform);
                    debugPlayer.AddComponent<DebugStateChanger>();


                }
            }
            if (GUI.Button(new Rect(24, 70, 126, 25), "SpawnBotCar"))
            {
                GameObject t = Instantiate(GameManager.Singleton.PlayerPrefab, GameManager.Singleton.SpawnPoint.transform.position, GameManager.Singleton.SpawnPoint.transform.rotation);
                debugs.Add(t);
                t.GetComponent<Player>().ChangerState(new EtatVoitureMouvement(t));
                t.GetComponent<Player>().SetProfile($"Bot {debugs.Count}");
            }
            if (GUI.Button(new Rect(24, 100, 126, 25), "DestroyAllBotCar"))
            {
                foreach (GameObject d in debugs)
                {
                    Destroy(d);
                }
            }

    }
}
