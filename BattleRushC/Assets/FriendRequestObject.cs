using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendRequestObject : MonoBehaviour
{


    [SerializeField] Text usernameText;
    string Username;



    public void Setup(string u)
    {
        Username = u;
        usernameText.text = u;
    }





    public void AcceptRequest()
    {
        RequestResponse(true);
    }

    public void RefuseRequest()
    {
        RequestResponse(false);
    }


    public void RequestResponse(bool yesno)
    {
        Action<DecoResponse> Success = new Action<DecoResponse>(OnAcceptSuccess);
        Action Failure = new Action(OnFailure);
        WWWForm form = new WWWForm();
        form.AddField("token", PlayerAccount.connectionToken);
        form.AddField("friendname", Username);
        form.AddField("accepted", yesno ? "YES":"NO") ;
        string link = "account/acceptRequest";
        StartCoroutine(ServerTalker.PostRequestToMasterServer<DecoResponse>(link, form, Success, Failure));

    }


    public void OnAcceptSuccess(DecoResponse response)
    {

        FindObjectOfType<FriendShipMenu>().GetAllRequests();
    }

    public void OnFailure()
    {
        
    }

}
