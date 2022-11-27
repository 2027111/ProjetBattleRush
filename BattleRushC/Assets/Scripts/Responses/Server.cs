using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Server
{
    public string ip;
    public string port;
    public string lobbyType;
    public string lobbyStatus;
    public int pConnected;
    public int pMax;


    public void Setup(Server serv)
    {
        this.ip = serv.ip;
        this.port = serv.port;
        this.pConnected = serv.pConnected;
        this.pMax = serv.pMax;
        this.lobbyStatus = serv.lobbyStatus;
        this.lobbyType = serv.lobbyType;
    }
}
