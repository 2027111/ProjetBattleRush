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
    public GameObject CamPrefab => camObject;
    public GameObject SpawnPoint => spawnPoint;


    [Header("Prefabs")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject spawnPoint;


    [Header("Debug")]
    [SerializeField] bool DebugServer;
    [SerializeField] private GameObject camObject;






    private void Awake()
    {
        Singleton = this;
    }

    private void Start()
    {
        if (DebugServer)
        {
            gameObject.AddComponent<DebugManager>();
        }
    }
}
