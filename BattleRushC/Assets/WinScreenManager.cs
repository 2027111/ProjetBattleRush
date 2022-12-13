using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinScreenManager : MonoBehaviour
{

    private static WinScreenManager _singleton;


    public static WinScreenManager Singleton
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
                Debug.Log($"{nameof(WinScreenManager)} instance already exists. Destroying duplicate!");
                Destroy(value.gameObject);
            }
        }
    }
    public bool[] stillConnected = { true, true, true };
    public GameObject[] GraphicCars = new GameObject[3];
    public string[] carNames = new string[3];
    public Color[,] ColorCars = new Color[3,3];
    public int playerCount = 3;

    // Start is called before the first frame update
    private void Awake()
    {
        Singleton = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public void GetAllCars()
    {
        for (int i = 0; i < GraphicCars.Length; i++)
        {
            if (i >= playerCount)
            {
                return;
            }
            GraphicCars[i] = GameObject.Find("CarDisplay" + i);
            if (stillConnected[i])
            {
                GraphicCars[i].GetComponent<CarGraphics>().SetBody(ColorCars[i, 0]);
                GraphicCars[i].GetComponent<CarGraphics>().SetEmissions(ColorCars[i, 1]);
                GraphicCars[i].GetComponent<CarGraphics>().SetRims(ColorCars[i, 2]);
                GraphicCars[i].GetComponent<UsernameContainer>().SetText(carNames[i]);
            }
            else
            {
                Destroy(GraphicCars[i]);
            }
        }


        Destroy(this.gameObject);

    }


    public void LeaveScene()
    {
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
