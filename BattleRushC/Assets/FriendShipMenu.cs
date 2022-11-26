using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendShipMenu : MonoBehaviour
{
    [SerializeField] InputField UsernameInput;
    [SerializeField] Transform friendListContent;
    [SerializeField] GameObject FriendObject;
    [SerializeField] GameObject[] Loadings;


    [SerializeField] Transform friendRequestsListContent;
    [SerializeField] GameObject FriendRequestObject;
    [SerializeField] GameObject RequestMenu;
    [SerializeField] GameObject FriendListMenu;


    private void Start()
    {
        StartLoading(true);
    }

    private void StartLoading(bool v)
    {
        foreach(GameObject l in Loadings)
        {
            l.SetActive(v);
        }
    }

    public void ToggleRequest(bool yes)
    {
        RequestMenu.SetActive(yes);
        FriendListMenu.SetActive(!yes);
    }




    public void GetAllFriends()
    {
        ClearList();
        StartLoading(true);


        string ThisPlayerUsername = PlayerAccount.connectedUser.username;
        Action<DecoResponse> Success = new Action<DecoResponse>(GetFriendsSuccess);
        Action Failure = new Action(OnFailure);
        WWWForm form = new WWWForm();
        form.AddField("rUsername", PlayerAccount.connectedUser.username);
        string link = "user/friend";
        StartCoroutine(ServerTalker.PostRequestToMasterServer<DecoResponse>(link, form, Success, Failure));


    }

    private void GetFriendsSuccess(DecoResponse response)
    {
        switch (response.code)
        {
            case 0:
                string[] lists = JsonHelper.getJsonArray<string>(response.data);
                Debug.Log(lists);
                foreach (string request in lists)
                {
                    Debug.Log(request);
                    GameObject FriendRequest = Instantiate(FriendObject, friendListContent);
                    FriendRequest.GetComponent<FriendRequestObject>().Setup(request);
                }
                break;
            case 11:
                Debug.Log("Alreay existing request Requests");
                break;
            case 12:
                Debug.Log("Opposing request existed. Accepted request");
                break;
            case -9:
                PlayerAccount.Disconnected();
                break;
            case -43:
                Debug.Log("User doesn't exist");
                break;
        }

        StartLoading(false) ;
    }

    public void GetAllRequests()
    {
        ClearList();

        StartLoading(true);


        string ThisPlayerUsername = PlayerAccount.connectedUser.username; 
        Action<DecoResponse> Success = new Action<DecoResponse>(GetRequestSuccess);
        Action Failure = new Action(OnFailure);
        WWWForm form = new WWWForm();
        form.AddField("token", PlayerAccount.connectionToken);
        string link = "account/getRequests";
        StartCoroutine(ServerTalker.PostRequestToMasterServer<DecoResponse>(link, form, Success, Failure));
        
        
    }

    private void ClearList()
    {
        foreach (Transform child in friendListContent)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in friendRequestsListContent)
        {
            Destroy(child.gameObject);
        }
    }

    private void GetRequestSuccess(DecoResponse response)
    {
        switch (response.code)
        {
            case 0:
                FriendRequest[] lists = JsonHelper.getJsonArray<FriendRequest>(response.data);
                foreach(FriendRequest request in lists)
                {
                    GameObject FriendRequest = Instantiate(FriendRequestObject, friendRequestsListContent);
                    FriendRequest.GetComponent<FriendRequestObject>().Setup(request.userRequestID);
                   
                }
                break;
            case 11:
                Debug.Log("Already existing request Requests");
                break;
            case 12:
                GetAllRequests();
                Debug.Log("Opposing request existed. Accepted request");
                break;
            case -9:
                PlayerAccount.Disconnected();
                break;
            case -43:
                Debug.Log("User doesn't exist");
                break;
        }

        StartLoading(false);
    }

    public void SendRequest()
    {
        if(UsernameInput.text.Length < 1)
        {
            return;
        }
        Action<DecoResponse> Success = new Action<DecoResponse>(OnAcceptSuccess);
        Action Failure = new Action(OnFailure);
        WWWForm form = new WWWForm();
        form.AddField("token", PlayerAccount.connectionToken);
        form.AddField("friendname", UsernameInput.text);
        string link = "account/sendfriendrequest";
        StartCoroutine(ServerTalker.PostRequestToMasterServer<DecoResponse>(link, form, Success, Failure));

    }


    public void OnAcceptSuccess(DecoResponse response)
    {
        switch (response.code)
        {
            case 0:
                Debug.Log("Request was sent");
                break;
            case 11:
                Debug.Log("Alreay existing request Requests");
                break;
            case 12:
                Debug.Log("Opposing request existed. Accepted request");
                break;
            case -9:
                PlayerAccount.Disconnected();
                break;
            case -43:
                Debug.Log("User doesn't exist");
                break;
        }

        UsernameInput.text = "";
    }

    public void OnFailure()
    {
        PlayerAccount.Disconnected();
    }


}
