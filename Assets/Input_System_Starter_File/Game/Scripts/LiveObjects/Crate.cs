using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.LiveObjects
{
    public class Crate : MonoBehaviour
    {
        [SerializeField] private float _punchDelay;
        [SerializeField] private GameObject _wholeCrate, _brokenCrate;
        [SerializeField] private Rigidbody[] _pieces;
        [SerializeField] private BoxCollider _crateCollider;
        [SerializeField] private InteractableZone _interactableZone;
        private bool _isReadyToBreak = false;

        private List<Rigidbody> _brakeOff = new List<Rigidbody>();
        private bool _holding = false;
        private float _holdTime = 0;

        private void OnEnable()
        {
            InteractableZone.onHoldStarted += InteractableZone_onHoldStarted;
            InteractableZone.onHoldEnded += InteractableZone_onHoldEnded;
        }

        private void InteractableZone_onHoldEnded(int zoneId)
        {
            if (zoneId == 6)
            {
                Debug.Log("End  Hold");
                _holding = false;
                Debug.Log(_holdTime);
                StopAllCoroutines();
                if(_holdTime < 0.25f)
                {
                    PerformBreak();
                }
                else if(_holdTime >= 0.25f)
                {
                    BreakMultiplePieces(_holdTime);
                }
                _holdTime = 0;
            }
        }

        private void InteractableZone_onHoldStarted(int zoneId)
        {
            if (zoneId == 6)
            {
                Debug.Log("Start Hold");
                _holding = true;
                StartCoroutine(StartHoldTimer());
            }
        }

        private void PerformBreak()
        {
            if (_isReadyToBreak == false && _brakeOff.Count > 0)
            {
                _wholeCrate.SetActive(false);
                _brokenCrate.SetActive(true);
                _isReadyToBreak = true;
            }

            if (_isReadyToBreak) //Crate zone            
            {
                if (_brakeOff.Count > 0)
                {
                    BreakPart();
                    StartCoroutine(PunchDelay());
                }
                else if (_brakeOff.Count == 0)
                {
                    _isReadyToBreak = false;
                    _crateCollider.enabled = false;
                    _interactableZone.CompleteTask(6);
                    Debug.Log("Completely Busted");
                }
            }
        }

        private void Start()
        {
            _brakeOff.AddRange(_pieces);
        }

        public void BreakPart()
        {
            int rng = Random.Range(0, _brakeOff.Count);
            _brakeOff[rng].constraints = RigidbodyConstraints.None;
            _brakeOff[rng].AddForce(new Vector3(1f, 1f, 1f), ForceMode.Force);
            _brakeOff.Remove(_brakeOff[rng]);            
        }

        private void BreakMultiplePieces(float holdTime)
        {
            int loops = 0;
            if (holdTime == 0.25)
                loops = 2;
            else if (holdTime > 0.25 && holdTime <= 0.5)
                loops = 3;
            else if (holdTime > 0.5 && holdTime <= 1)
                loops = 4;
            else if (holdTime > 1)
                loops = 5;
            for (int i = 1; i < loops; i++)
            {
                if (_brakeOff.Count >= 0)
                    PerformBreak();
                else
                    break;
            }
        }

        IEnumerator PunchDelay()
        {
            float delayTimer = 0;
            while (delayTimer < _punchDelay)
            {
                yield return new WaitForEndOfFrame();
                delayTimer += Time.deltaTime;
            }

            _interactableZone.ResetAction(6);
        }

        IEnumerator StartHoldTimer()
        {
            while (_holding)
            {
                yield return new WaitForSeconds(0.25f);
                _holdTime += 0.25f;
            }
        }

        private void OnDisable()
        {
            InteractableZone.onHoldStarted -= InteractableZone_onHoldStarted;
            InteractableZone.onHoldEnded -= InteractableZone_onHoldEnded;
        }
    }
}
