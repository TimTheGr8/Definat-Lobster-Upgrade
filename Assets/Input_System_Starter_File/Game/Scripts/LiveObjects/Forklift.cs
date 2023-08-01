using System;
using UnityEngine;
using Cinemachine;

namespace Game.Scripts.LiveObjects
{
    public class Forklift : MonoBehaviour
    {
        [SerializeField]
        private GameObject _lift, _steeringWheel, _leftWheel, _rightWheel, _rearWheels;
        [SerializeField]
        private Vector3 _liftLowerLimit, _liftUpperLimit;
        [SerializeField]
        private float _speed = 5f, _liftSpeed = 1f;
        [SerializeField]
        private CinemachineVirtualCamera _forkliftCam;
        [SerializeField]
        private GameObject _driverModel;
        private bool _inDriveMode = false;
        [SerializeField]
        private InteractableZone _interactableZone;
        [SerializeField]
        private InputManager _inputManager;

        public static event Action onDriveModeEntered;
        public static event Action onDriveModeExited;

        private bool _isMastMoving = false;

        private void OnEnable()
        {
            InteractableZone.onZoneInteractionComplete += EnterDriveMode;
        }

        private void EnterDriveMode(InteractableZone zone)
        {
            if (_inDriveMode !=true && zone.GetZoneID() == 5) //Enter ForkLift
            {
                _inDriveMode = true;
                _inputManager.AssignForklift(this);
                _inputManager.InitializeForkliftInputs();
                _forkliftCam.Priority = 11;
                onDriveModeEntered?.Invoke();
                _driverModel.SetActive(true);
                _interactableZone.CompleteTask(5);
            }
        }

        public void Move(Vector2 input)
        {
            var direction = new Vector3(0, 0, input.y); 
            var velocity = direction * _speed;

            transform.Translate(velocity * Time.deltaTime);

            if (Mathf.Abs(input.y) > 0)
            {
                var tempRot = transform.rotation.eulerAngles;
                tempRot.y += input.x * _speed / 2;
                transform.rotation = Quaternion.Euler(tempRot);
            }
        }

        public void SetMastMoving(bool isMastMoving)
        {
            _isMastMoving = isMastMoving;
        }

        public void LiftRoutine(float input)
        {
            Vector3 tempPos = _lift.transform.localPosition;
            if (input < 0 & _lift.transform.localPosition.y >= _liftLowerLimit.y)
                tempPos.y -= Time.deltaTime * _liftSpeed;
            if (input > 0 & _lift.transform.localPosition.y <= _liftUpperLimit.y)
                tempPos.y += Time.deltaTime * _liftSpeed;

            _lift.transform.localPosition = new Vector3(tempPos.x, tempPos.y, tempPos.z);
        }

        public void DisableForkiftInputs()
        {
            _inDriveMode = false;
            _forkliftCam.Priority = 9;
            _driverModel.SetActive(false);
            onDriveModeExited?.Invoke();
        }

        private void OnDisable()
        {
            InteractableZone.onZoneInteractionComplete -= EnterDriveMode;
        }

    }
}