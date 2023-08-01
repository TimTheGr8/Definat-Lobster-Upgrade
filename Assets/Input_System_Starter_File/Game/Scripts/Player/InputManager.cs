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
    [SerializeField]
    private Drone _drone;

    private PlayerInputs _input;
    private InteractableZone _interactableZone;
    private Laptop _laptop;
    private Forklift _forklift;

    // Start is called before the first frame update
    void Start()
    {
        InitializePlayerInputs();
    }

    // Update is called once per frame
    void Update()
    {
        var playerMove = _input.Player.Movement.ReadValue<Vector2>();
        _player.CalcutateMovement(playerMove);

        if (_drone != null)
        {
            var droneMove = _input.Drone.Movement.ReadValue<Vector2>();
            _drone.TiltDrone(droneMove.y);
            _drone.RotateDrone(droneMove.x);
        }

        if(_forklift != null)
        {
            var forkliftMove = _input.Forklift.Movement.ReadValue<Vector2>();
            _forklift.Move(forkliftMove);
            var mastMovement = _input.Forklift.Forks.ReadValue<float>();
            _forklift.LiftRoutine(mastMovement);
        }
    }

    private void FixedUpdate()
    {
        if (_drone != null)
        {
            // TODO: use this for the forklift mast
            var droneVertical = _input.Drone.Thrust.ReadValue<float>();
            _drone.SetVerticalDirection(droneVertical);
        }
    }

    // Main Player Inputs
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

    // Security Camera Inputs
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
        _input.SecurityCameras.Disable();
        InitializePlayerInputs();
    }

    private void SwitchCameras_performed(InputAction.CallbackContext context)
    {
        _laptop.SwitchCamera();
    }

    // Drone Inputs
    public void InitializeDroneInputs()
    {
        _input.Player.Disable();
        _input.Drone.Enable();

        _input.Drone.DiableDroneInputs.performed += DiableDroneInputs_performed;
    }

    public void AssignDrone(Drone drone)
    {
        _drone = drone;
    }

    private void DiableDroneInputs_performed(InputAction.CallbackContext context)
    {
        _drone.DisableDrone();
        _input.Drone.Disable();
        _drone = null;
        InitializePlayerInputs();
    }

    // Forklift Inputs
    public void InitializeForkliftInputs()
    {
        _input.Player.Disable();
        _input.Forklift.Enable();

        _input.Forklift.DisableForkliftInputs.performed += DisableForkliftInputs_performed;
    }

    private void DisableForkliftInputs_performed(InputAction.CallbackContext obj)
    {
        _input.Forklift.Disable();
        _forklift.DisableForkiftInputs();
        _forklift = null;
        InitializePlayerInputs();
    }

    public void AssignForklift(Forklift forklift)
    {
        _forklift = forklift;
    }


}

