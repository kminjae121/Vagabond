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
        public float maxchargingTime { get; set; } = 5;

        private float chargeAttackSec = 3f;

        private Coroutine _timerCoroutine;

        public void Initialize(Entity entity)
        {
            _inputReader.AttackEvent += HandleAttack;
            _inputReader.ChargingEvent += HandleCharge;
            _inputReader.ChargingAttackEvent += HandleChargeAttack;
            _inputReader.AttackEndEvent += HandleAttackEnd;
            _player = entity as Player;
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
            
            StartCoroutine(DashRoutine(dashDirection, dashSpeed, dashDuration));

            Debug.Log("차징공격 발동!");
        }

        private IEnumerator DashRoutine(Vector3 direction, float speed, float duration)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                _player.transform.position += direction * speed * Time.deltaTime;
                elapsed += Time.deltaTime;
                yield return null;
            }
            _player.ChangeState("IDLE");
        }
        
        public IEnumerator ChargeAttackSec()
        {
            while (chargingTime < maxchargingTime)
            {
                chargingTime += 1;
                
                Debug.Log($"플레이어 차징중!{chargingTime}");
                
                yield return new WaitForSeconds(1f);
            }
        }
    }
}