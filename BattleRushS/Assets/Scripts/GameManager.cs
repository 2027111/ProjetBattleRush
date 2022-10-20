using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _singleton;

    public static GameManager Singleton
    {
        get => _singleton;
        private set
        {
            if (_singleton == null)
            {

                _singleton = value;
            }
            else if (_singleton != value)
            {
                Debug.Log($"{nameof(GameManager)} instance already exists. Destroying duplicate!");
                Destroy(value.gameObject);
            }
        }
    }


    public GameObject PlayerPrefab => playerPrefab;
    public GameObject SpawnPoint => spawnPoint;


    [Header("Prefabs")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject spawnPoint;

    [Header("Debug")]
    [SerializeField] bool DebugServer;
    GameObject debugPlayer;
    List<GameObject> debugs = new List<GameObject>();

    private void Update()
    {
        if (DebugServer)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                if (debugPlayer)
                {
                    Destroy(debugPlayer);
                }
                else
                {
                    debugPlayer = Instantiate(PlayerPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
                    debugPlayer.AddComponent<DebugControls>();
                    debugPlayer.GetComponent<Player>().ChangerState(new EtatVoitureMouvement(debugPlayer));
                    debugPlayer.GetComponent<Player>().SetProfile("Debug");


                }
            }
            if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                GameObject t = Instantiate(PlayerPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
                debugs.Add(t);
                t.GetComponent<Player>().ChangerState(new EtatVoitureMouvement(t));
               t.GetComponent<Player>().SetProfile($"Bot{debugs.Count}");


            }
            if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                foreach(GameObject d in debugs)
                {
                    Destroy(d);
                }
            }
            if (debugPlayer)
            {
                Debug.Log(debugPlayer.GetComponent<Player>().rb.velocity.magnitude);
            }
        }
    }




    //[SerializeField] BattleUI bui;
    //bui.Setup(localPlayer, enemyplayer);



    private void Awake()
    {
        Singleton = this;
    }
}
