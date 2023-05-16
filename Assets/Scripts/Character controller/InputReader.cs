using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputReader : MonoBehaviour, Controls.IPlayerActions, Controls.ICameraActions
{
    public Vector2 Movement { get; private set; }
    public event Action onInteracted;
    public event Action onUseItem;
    public bool Sprint { get; private set; }
    public bool Autosprint { get; private set; }
    public bool PlayerMapEnabled => controls.Player.enabled;

    public Vector2 CameraRotate { get; private set; }
    public float Zoom { get; private set; }
    public event Action onLockOn;
    public bool CameraMapEnabled => controls.Camera.enabled;

    private Controls controls;

    private void OnEnable()
    {
        if(controls == null)
        {
            controls = new Controls();
        }
        controls.Player.SetCallbacks(this);
        controls.Player.Enable();
        controls.Camera.SetCallbacks(this);
        controls.Camera.Enable();
        controls.UI.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Disable();
        controls.Camera.Disable();
        controls.UI.Disable();
    }

    public void SwitchUI(bool value)
    {
        if (value)
        {
            controls.UI.Enable();
            controls.Player.Disable();

            Movement = Vector2.zero;
        }
        else
        {
            controls.UI.Disable();
            controls.Player.Enable();
        }

    }

    public void PlayerControls(bool value)
    {
        if (!value)
        {
            controls.Player.Disable();
            Movement = Vector2.zero;
            return;
        }
        controls.Player.Enable();
    }

    //IPlayerActions
    public void OnMove(InputAction.CallbackContext context)
    {
        Movement = context.ReadValue<Vector2>();
    }

    public void OnAction(InputAction.CallbackContext context)
    {
        if(context.canceled)
        {
            onInteracted?.Invoke();
        }
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        Sprint = context.ReadValueAsButton();
    }

    public void OnAutosprint(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            Autosprint = !Autosprint;
        }
    }

    public void OnUseItem(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            onUseItem?.Invoke();
        }
    }

    //ICameraActions
    public void OnRotate(InputAction.CallbackContext context)
    {
        CameraRotate = context.ReadValue<Vector2>();
    }

    public void OnLockOn(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            onLockOn?.Invoke();
        }
    }

    public void OnZoom(InputAction.CallbackContext context)
    {
        Zoom = context.ReadValue<float>();
    }
}
