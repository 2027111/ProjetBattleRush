using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugMasterServerModifyier : MonoBehaviour
{

    [Header("Debug UI")]
    [SerializeField] InputField ipInput;
    [SerializeField] InputField portInput;
    [SerializeField] Text ipcurrent;
   
    // Start is called before the first frame update
    void Start()
    {

        ipInput.text = "127.0.0.1";
        portInput.text = "5500";
        SetMasterAddress();
    }

    public void SetMasterAddress()
    {

        ServerTalker.mainAddress = $"http://{ipInput.text}:{portInput.text}/";
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
