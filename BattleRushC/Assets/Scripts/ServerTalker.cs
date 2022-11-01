using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using SimpleJSON;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class ServerTalker : MonoBehaviour
{


    [Header("Login UI")]
    [SerializeField] InputField usernameInput;
    [SerializeField] InputField passwordInput;
    [SerializeField] Button connectButton;
    [SerializeField] TextMeshProUGUI alertText;


    [Header("Register UI")]
    [SerializeField] InputField rusernameInput;
    [SerializeField] InputField remailInput;
    [SerializeField] InputField rpasswordInput;
    [SerializeField] Button signupButton;
    [SerializeField] TextMeshProUGUI ralertText;



    [Header("Debug")]
    [SerializeField] bool debug;
    

    [HideInInspector] public static string mainAddress = "http://127.0.0.1:5500/";

    private void Awake()
    {

        Application.targetFrameRate = 60;
    }



    public static IEnumerator PostRequestToMasterServer<T>(string link, WWWForm form, Action<T> success, Action failure)
    {
        UnityWebRequest request = UnityWebRequest.Post($"{mainAddress}{link}", form);
        var handler = request.SendWebRequest();
        float startTime = 0;
        while (!handler.isDone)
        {
            startTime += Time.deltaTime;
            if (startTime >= 10.0f)
            {
                break;
            }
            yield return null;
        }

        if (request.result == UnityWebRequest.Result.Success)
        {
            T response = JsonUtility.FromJson<T>(request.downloadHandler.text);
            success(response);

        }
        else
        {
            failure();
        }
        yield return null;
    }



    #region Login
    public void OnLoginClick()
    {
        ActivateButtons(false);
        if (debug)
        {

            string username = usernameInput.text;
            string password = passwordInput.text;

            if(username == "dddd" && password == "dddd")
            {

                PlayerAccount.Connected(new PlayerAccount(), "");
                LoadingScene.main.LoadScene("MainMenuScene");
            }
        }
        else
        {
            Action<LoginResponse> Success = new Action<LoginResponse>(LoginSuccess);
            Action Failure = new Action(LoginFailure);
            string username = usernameInput.text;
            string password = passwordInput.text;
            WWWForm form = new WWWForm();
            form.AddField("rPassword", password);
            form.AddField("rUsername", username);
            string link = "account/login";
            alertText.text = "Signing in ...";
            StartCoroutine(PostRequestToMasterServer<LoginResponse>(link, form, Success, Failure));
        }
    }


    void LoginSuccess(LoginResponse response)
    {

        switch (response.code)
        {
            case 0:
                ActivateButtons(false);
                alertText.text = $"Welcome {response.data.username}";
                PlayerAccount.Connected(response.data, response.token);
                LoadingScene.main.LoadScene("MainMenuScene");
                break;
            default:
                ActivateButtons(true);
                alertText.text = response.message;
                break;
        }
        

        
        ResetInputsLogin();
    }
    void LoginFailure()
    {
        alertText.text = "Error connecting to the server...";
        ResetInputsLogin();
        ActivateButtons(true);

    }
 

    #endregion


    #region Registration
    public void OnSignUpClick()
    {
        ActivateButtons(false);

        Action<LoginResponse> Success = new Action<LoginResponse>(RegisterSuccess);
        Action Failure = new Action(RegisterFailure);
        string username = rusernameInput.text;
        string password = rpasswordInput.text;
        string email = remailInput.text;
        WWWForm form = new WWWForm();
        form.AddField("rPassword", password);
        form.AddField("rUsername", username);
        form.AddField("rEmail", email);
        string link = "account/create";
        alertText.text = "Signing in ...";
        StartCoroutine(PostRequestToMasterServer<LoginResponse>(link, form, Success, Failure));

    }
    void RegisterSuccess(LoginResponse response)
    {
        switch (response.code)
        {
            case 0:

                ActivateButtons(true);
                ralertText.text = $"Account has been created!";
                break;
            case 1:
                ActivateButtons(true);
                ralertText.text = "Invalid Credentials...";
                break;
            case 2:
                ActivateButtons(true);
                ralertText.text = "Email is already in use.";
                break;
            case 3:
                ActivateButtons(true);
                ralertText.text = "Username is already in use";
                break;
            default:
                ralertText.text = "Corruption Detected...";
                ActivateButtons(false);
                PlayerAccount.Disconnected();
                break;
        }
    }

    void RegisterFailure()
    {

        ralertText.text = "Error connecting to the server...";
        ActivateButtons(true);
    }

    #endregion

    #region[Misc.]
    private void ResetInputsLogin()
    {
        usernameInput.text = "";
        passwordInput.text = "";
        rusernameInput.text = "";
        rpasswordInput.text = "";
        remailInput.text = "";
    }
    private void ActivateButtons(bool toggle)
    {

        signupButton.interactable = toggle;
        connectButton.interactable = toggle;
    }
    #endregion

    #region [Déconnexion]
    private void OnApplicationQuit()
    {
        TempDisconnect();
    }


    public void ConfirmDisconnection(Response t)
    {
        PlayerAccount.Disconnected();
    }
    public void TempDisconnect()
    {
        Action<Response> Success = new Action<Response>(ConfirmDisconnection);
        Action Failure = new Action(PlayerAccount.Disconnected);
        WWWForm form = new WWWForm();
        form.AddField("tokenid", PlayerAccount.connectionToken);
        string link = "deconnexion";
        StartCoroutine(ServerTalker.PostRequestToMasterServer<Response>(link, form, Success, Failure));


        //StartCoroutine(TryDisconnect());
    }

    #endregion
}
