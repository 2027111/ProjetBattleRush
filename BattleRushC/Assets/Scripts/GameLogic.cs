using Riptide;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    private static GameLogic _singleton;

    public static GameLogic Singleton
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
                Debug.Log($"{nameof(GameLogic)} instance already exists. Destroying duplicate!");
                Destroy(value.gameObject);
            }
        }
    }


    public GameObject PlayerPrefab => playerPrefab;

    [Header("Prefabs")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] public GameObject message;
    [SerializeField] GameObject[] particleList;



    public void SpawnParticle(Vector3 pos, int index)
    {
        Debug.Log("Spawned " + index);
        Instantiate(particleList[index], pos, Quaternion.identity);
    }

    private void Awake()
    {
        Singleton = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [MessageHandler((ushort)ServerToClientId.part)]
    public static void HandleSpawnPart(Message message)
    {

        int index = message.GetInt();
        Vector3 pos = message.GetVector3();
        Singleton.SpawnParticle(pos, index);


    }
}
