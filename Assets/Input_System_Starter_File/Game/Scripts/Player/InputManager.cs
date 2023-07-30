using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Game.Scripts.Player;
using Game.Scripts.LiveObjects;

public class InputManager : MonoBehaviour
{

    [SerializeField]
    private Player _player;

    private PlayerInputs _input;
    private InteractableZone _interactableZone;
    private Laptop _laptop;

    // Start is called before the first frame update
    void Start()
    {
        InitializePlayerInputs();
    }

    // Update is called once per frame
    void Update()
    {
        var move = _input.Player.Movement.ReadValue<Vector2>();
        _player.CalcutateMovement(move);
    }

    // Main Player Controls
    private void InitializePlayerInputs()
    {
        _input = new PlayerInputs();
        _input.Player.Enable();

        _input.Player.Interact.performed += Interact_performed;
        _input.Player.Interact.started += Interact_started;
        _input.Player.Interact.canceled += Interact_canceled;
    }

    private void Interact_canceled(InputAction.CallbackContext context)
    {
        if (_interactableZone != null)
        {
            _interactableZone.InteractionCanceled();
        }
    }

    private void Interact_started(InputAction.CallbackContext context)
    {
        if (_interactableZone != null)
        {
            _interactableZone.InteractionStarted();
        }
    }

    private void Interact_performed(InputAction.CallbackContext context)
    {
        if(_interactableZone != null)
        {
            _interactableZone.Interact();
        }
    }

    public void AssignInteracable(InteractableZone interactable)
    {
        _interactableZone = interactable;
    }

    // Security Camera Controls
    public void InitializeCameraInputs()
    {
        _input.Player.Disable();
        _input.SecurityCameras.Enable();

        _input.SecurityCameras.SwitchCameras.performed += SwitchCameras_performed;
        _input.SecurityCameras.DisableCameras.performed += DisableCameras_performed;
    }

    public void AssignLaptop(Laptop laptop)
    {
        _laptop = laptop;
    }

    private void DisableCameras_performed(InputAction.CallbackContext context)
    {
        _laptop.DisableCameras();
        InitializePlayerInputs();
    }

    private void SwitchCameras_performed(InputAction.CallbackContext context)
    {
        _laptop.SwitchCamera();
    }
}

