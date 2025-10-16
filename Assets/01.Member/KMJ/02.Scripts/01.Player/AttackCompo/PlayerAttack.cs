using System;
using System.Collections;
using _00.CORE._02.Scripts.Input;
using Code.Core.Debugs;
using Code.Entities;
using Code.Interfaces;
using UnityEngine;

namespace _01.Member.KMJ._02.Scripts._01.Player.AttackCompo
{
    public class PlayerAttack : MonoBehaviour, IEntityComponent
    {
        [SerializeField] private InputReader _inputReader;
        private Player _player;

        public float chargingTime { get; set; } = 0;
        [field: SerializeField] public float maxchargingTime { get; set; } = 5;

        [field: SerializeField] private float chargeAttackSec = 3f;

        [SerializeField] private float dashSpeed = 5;
        [SerializeField] private float maxtimer = 1.2f;
        [SerializeField] private float flyingSpeed = 3;

        public bool isDashAttacking { get; set; } = false;

        public float _timer { get; set; } = 0;

        private Rigidbody _rbComponent;
        
        private Coroutine _timerCoroutine;

        public void Initialize(Entity entity)
        {
            _inputReader.AttackEvent += HandleAttack;
            _inputReader.ChargingEvent += HandleCharge;
            _inputReader.ChargingAttackEvent += HandleChargeAttack;
            _inputReader.AttackEndEvent += HandleAttackEnd;
            _player = entity as Player;
            _rbComponent = _player.GetComponent<Rigidbody>();
        }

        private void OnDestroy()
        {
            _inputReader.AttackEvent -= HandleAttack;
            _inputReader.ChargingEvent -= HandleCharge;
            _inputReader.ChargingAttackEvent -= HandleChargeAttack;
            _inputReader.AttackEndEvent -= HandleAttackEnd;
        }

        private void HandleCharge()
        {
            _player.ChangeState("CHARGE");
        }

        private void HandleAttack()
        {
            if (_player.bloodSystemCompo.isFallingFlower)
            {
                HandleCharge();
            }
            else
            {
                _player.swordCompo.Attack();
            }
        }
        public void StartChargingTimer()
        {
            if (_timerCoroutine == null)
            {
                _timerCoroutine = StartCoroutine(ChargeAttackSec());
            }
        }

        public void StopChargingTimer()
        {
            if (_timerCoroutine != null)
            {
                StopCoroutine(_timerCoroutine);
                _timerCoroutine = null;
            }
        }

        private void HandleChargeAttack()
        {
            StopChargingTimer();
            if (_player.aimmingComponent.aimingObject != null)
            {
                _player.ChangeState("GUIDEATTACK");
            }
            else if (chargingTime >= chargeAttackSec)
            {
                _player.ChangeState("CHARGEATTACK");
            }
            else
            {
                _player.ChangeState("IDLE");
            }

            chargingTime = 0;
        }
        
        private void HandleAttackEnd()
        {
            if (_player.bloodSystemCompo.isFallingFlower)
            {
                StopChargingTimer();
                if (chargingTime >= chargeAttackSec)
                {
                    _player.ChangeState("CHARGEATTACK");
                }
                else
                {
                    _player.ChangeState("IDLE");
                }

                chargingTime = 0;
            }
            else return;
        }

        
        public void ChargingAttack()
        {
            if (_player == null) return;
            
            Vector3 dashDirection = _player.transform.forward;
            
            float dashSpeed = 50f;
            
            float dashDuration = 0.3f;
            
        }

        public void GuidedAttack()
        {
            if (_player.aimmingComponent.aimingObject == null)
            {
                _player.ChangeState("IDLE");
            }
            else
            {
                _player.transform.position = Vector3.MoveTowards(_player.transform.position,
                    _player.aimmingComponent.aimingObject.transform.position, Time.deltaTime * flyingSpeed);
            }
        }

        //private IEnumerator DashRoutine(Vector3 direction, float speed, float duration)
        //{
        //    float elapsed = 0f;
        //    while (elapsed < duration)
        //    {
        //        _player.transform.position += direction * speed * Time.deltaTime;
        //        elapsed += Time.deltaTime;
        //        yield return null;
        //    }
        //    _player.ChangeState("IDLE");
        //}

        public void Dash()
        {
            _timer += Time.fixedDeltaTime;

            _rbComponent.AddForce(transform.forward * (dashSpeed - _timer) , ForceMode.Impulse);
                
            if (_timer >= maxtimer)
            {
                _player.ChangeState("IDLE");
            }
        }
        
        public IEnumerator ChargeAttackSec()
        {
            while (chargingTime < maxchargingTime)
            {
                chargingTime += 1;
                
                yield return new WaitForSeconds(1f);
            }
        }
    }
}