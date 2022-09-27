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
    public GameObject LocalPlayerPrefab => localplayerPrefab;

    [Header("Prefabs")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject localplayerPrefab;





    private void Awake()
    {
        Singleton = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
