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



    [HideInInspector] public static string mainAddress = "http://127.0.0.1:5500/";
    // Start is called before the first frame update

    private void Awake()
    {

    }
    void Start()
    {
    }



    #region Login
    public void OnLoginClick()
    {
        ActivateButtons(false);
        StartCoroutine(TryLogin());
    }
    IEnumerator TryLogin()
    {
        alertText.text = "Signing in ...";
        string username = usernameInput.text;
        string password = passwordInput.text;
        WWWForm form = new WWWForm();
        form.AddField("rPassword", password);
        form.AddField("rUsername", username);
        UnityWebRequest request = UnityWebRequest.Post($"{mainAddress}account/login", form);
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
            LoginResponse response = JsonUtility.FromJson<LoginResponse>(request.downloadHandler.text);
            switch (response.code)
            {
                case 0:

                    ActivateButtons(false);
                    alertText.text = $"Welcome {response.data.username}";
                    PlayerAccount.Connected(response.data, response.token);
                    SceneManager.LoadScene("GameScene");
                    break;
                case 1:
                    ActivateButtons(true);
                    alertText.text = "Invalid Credentials...";
                    break;
                default:
                    PlayerAccount.Disconnected();
                    alertText.text = "Corruption Detected...";
                    ActivateButtons(false);
                    break;
            }
            ResetInputsLogin();

        }
        else
        {
            alertText.text = "Error connecting to the server...";
            ResetInputsLogin();
            Debug.LogError(request.error);
            ActivateButtons(true);
        }
        yield return null;
    }

    private void ResetInputsLogin()
    {
        usernameInput.text = "";
        passwordInput.text = "";
    }
    #endregion


    #region Registration
    public void OnSignUpClick()
    {
        ActivateButtons(false);
        StartCoroutine(TryRegister());
    }


    IEnumerator TryRegister()
    {
        ralertText.text = "Signing in ...";
        string username = rusernameInput.text;
        string password = rpasswordInput.text;
        string email = remailInput.text;
        WWWForm form = new WWWForm();
        form.AddField("rPassword", password);
        form.AddField("rUsername", username);
        form.AddField("rEmail", email);
        UnityWebRequest request = UnityWebRequest.Post($"{mainAddress}account/create", form);
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
            LoginResponse response = JsonUtility.FromJson<LoginResponse>(request.downloadHandler.text);
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
            ResetInputsSignup();
        }
        else
        {
            ralertText.text = "Error connecting to the server...";
            Debug.LogError(request.error);
            ActivateButtons(true);
        }
        yield return null;
    }
    private void ResetInputsSignup()
    {
        rusernameInput.text = "";
        rpasswordInput.text = "";
        remailInput.text = "";
    }
    #endregion


    private void ActivateButtons(bool toggle)
    {

        signupButton.interactable = toggle;
        connectButton.interactable = toggle;
    }

    private void OnApplicationQuit()
    {
        TempDisconnect();
    }



    public void TempDisconnect()
    {
        StartCoroutine(TryDisconnect());
    }

    IEnumerator TryDisconnect()
    {
        WWWForm form = new WWWForm();
        form.AddField("tokenid", PlayerAccount.connectionToken);
        UnityWebRequest request = UnityWebRequest.Post($"{mainAddress}deconnexion", form);
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

            PlayerAccount.Disconnected();
        }
        else
        {
            PlayerAccount.Disconnected();
        }
        yield return null;
    }

}
