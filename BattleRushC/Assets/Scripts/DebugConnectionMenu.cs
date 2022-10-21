using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugConnectionMenu : MonoBehaviour
{
    [SerializeField] InputField ip;
    [SerializeField] InputField port;
    [SerializeField] GameObject menu;


    // Start is called before the first frame update
    void Start()
    {
        ip.text = "127.0.0.1";
        port.text = "63577";
    }

    public void ConnectToPort()
    {
        if (ip.text.Trim().Length == 0 || port.text.Trim().Length == 0)
        {
            return;
        }
        NetworkManager.Singleton.ConnectTo(ip.text, port.text);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Insert) && Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.LeftControl))
        {
            menu.SetActive(!menu.activeSelf);
        }
    }
}
