using Riptide;
using Riptide.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager _singleton;

    public static UIManager Singleton
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
                Debug.Log($"{nameof(UIManager)} instance already exists. Destroying duplicate!");
                Destroy(value);
            }
        }
    }

    [SerializeField] private MenuPreset[] menus;
    [SerializeField] private Text usernameText;
    [SerializeField] private Text goldAmountText;
    [SerializeField] private Transform scoreBoardParent;
    [SerializeField] public GameObject scoreBoardCard;
    [SerializeField] public GameObject disconnect;
    [SerializeField] public Text SpeedometerAmount;
    [SerializeField] public Timer timer;
    [SerializeField] public Image boostMeterFill;
    bool GameScene = false;

    private void Awake()
    {
        Singleton = this;

    }

    private void Update()

    { 
        if(SceneManager.GetActiveScene().name == "GameScene")
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                disconnect.SetActive(!disconnect.activeSelf);
            }

        }
    }

    private void Start()
    {
        UpdateParameters();
    }


    public void SetSBCard(Player player)
    {

        GameObject sbc = Instantiate(scoreBoardCard, scoreBoardParent);
        sbc.GetComponent<ScoreBoardCard>().Affiliate(player);
    }

    public void LeaveGame()
    {
        Application.Quit();
    }

    public void SetTimerAmount(string t)
    {
        timer.Amountis(t);
    }

    public void SetSpeed(Vector3 v)
    {
        
        float speed = v.sqrMagnitude;
        SpeedometerAmount.text = ((int)speed) + "km/h";
    }

    public void SetBoost(float fillamount)
    {
        boostMeterFill.fillAmount = fillamount / 100;
    }
    public void UpdateParameters()
    {

        if (PlayerAccount.IsConnected())
        {
            usernameText.text = PlayerAccount.connectedUser.username;

            goldAmountText.text = PlayerAccount.connectedUser.goldcoins.ToString();
        }
    }



    public void ServerDIsconnect()
    {
        NetworkManager.Singleton?.Disconnect();
    }
}
