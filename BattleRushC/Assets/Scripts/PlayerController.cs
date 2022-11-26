using Riptide;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{


    private PlayerInput pi;
    private InputAction move;
    private bool Jump;
    private bool Switch;
    private Vector2 forceDirection = Vector2.zero;

    private void Awake()
    {
        pi = new PlayerInput();
    }

    private void OnEnable()
    {

        pi.Player.Jump.started += DoJump;
        pi.Player.CamSwitch.started += DoCamSwitch;
        pi.Player.Menu.started += DoMenu;
        move = pi.Player.Move;
        pi.Player.Enable();

    }

    private void DoMenu(InputAction.CallbackContext obj)
    {
        UIManager.Singleton?.ActivateEscapeUI();

    }

    private void OnDisable()
    {
        pi.Player.Jump.started -= DoJump;
        pi.Player.CamSwitch.started -= DoCamSwitch;
        pi.Player.Disable();

    }
    void Update()
    {

        forceDirection.x = move.ReadValue<Vector2>().x;
        forceDirection.y = move.ReadValue<Vector2>().y;

    }

    private void DoJump(InputAction.CallbackContext obj)
    {
        Jump = true;
    }
    private void DoCamSwitch(InputAction.CallbackContext obj)
    {
        Switch = true;
    }
    private void FixedUpdate()
    {
        SendInput();
        Jump = false;
        Switch = false;
    }


    #region Messages

    private void SendInput()
    {
        Message message = Message.Create(MessageSendMode.Unreliable, ClientToServerId.input);
        message.AddVector2(forceDirection);
        message.AddBool(Jump);
        message.AddBool(Switch);
        NetworkManager.Singleton.Client.Send(message);
    }

    #endregion
}
