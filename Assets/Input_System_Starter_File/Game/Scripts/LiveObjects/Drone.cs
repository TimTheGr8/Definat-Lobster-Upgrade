using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Game.Scripts.UI;

namespace Game.Scripts.LiveObjects
{
    public class Drone : MonoBehaviour
    {
        private enum Tilt
        {
            NoTilt, Forward, Back, Left, Right
        }

        [SerializeField]
        private Rigidbody _rigidbody;
        [SerializeField]
        private float _speed = 5f;
        private bool _inFlightMode = false;
        [SerializeField]
        private Animator _propAnim;
        [SerializeField]
        private CinemachineVirtualCamera _droneCam;
        [SerializeField]
        private InteractableZone _interactableZone;
        [SerializeField]
        private InputManager _inputManager;
        

        public static event Action OnEnterFlightMode;
        public static event Action onExitFlightmode;

        private float _verticalDirection = 0;

        private void OnEnable()
        {
            InteractableZone.onZoneInteractionComplete += EnterFlightMode;
        }

        private void EnterFlightMode(InteractableZone zone)
        {
            if (_inFlightMode != true && zone.GetZoneID() == 4) // drone Scene
            {
                _propAnim.SetTrigger("StartProps");
                _droneCam.Priority = 11;
                _inFlightMode = true;
                OnEnterFlightMode?.Invoke();
                UIManager.Instance.DroneView(true);
                _interactableZone.CompleteTask(4);
                _inputManager.AssignDrone(this);
                _inputManager.InitializeDroneInputs();
            }
        }

        private void ExitFlightMode()
        {            
            _droneCam.Priority = 9;
            _inFlightMode = false;
            UIManager.Instance.DroneView(false);            
        }

        private void FixedUpdate()
        {
            _rigidbody.AddForce(transform.up * (9.81f), ForceMode.Acceleration);
            if (_inFlightMode)
                CalculateMovementFixedUpdate(_verticalDirection);
        }

        public void RotateDrone (float input)
        {
            var tempRot = transform.localRotation.eulerAngles;
            if(input < 0)
                tempRot.y -= _speed / 3;
            if (input > 0)
                tempRot.y += _speed / 3;
            transform.localRotation = Quaternion.Euler(tempRot);
        }

        public void SetVerticalDirection(float direction)
        {
            _verticalDirection = direction;
        }

        private void CalculateMovementFixedUpdate(float direction)
        {
            _rigidbody.AddForce((transform.up * direction) * _speed, ForceMode.Acceleration);
        }

        public void TiltDrone(float value)
        {
            if(value < 0)
            {
                transform.rotation = Quaternion.Euler(30, transform.localRotation.eulerAngles.y, 0);
            }
            else if(value > 0)
            {
                transform.rotation = Quaternion.Euler(-30, transform.localRotation.eulerAngles.y, 0);
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, transform.localRotation.eulerAngles.y, 0);
            }
        }

        public void DisableDrone()
        {
            _inFlightMode = false;
            onExitFlightmode?.Invoke();
            ExitFlightMode();
        }

        private void OnDisable()
        {
            InteractableZone.onZoneInteractionComplete -= EnterFlightMode;
        }
    }
}
