using System;
using System.Collections;
using Code.Core.Debugs;
using Code.Entities;
using Code.Interfaces;
using UnityEngine;

namespace _01.Member.KMJ._02.Scripts._01.Player.AttackCompo
{
    public class PlayerAttack : MonoBehaviour, IEntityComponent
    {
        [Header("Player Reference")]
        private Player _player;
        
        [Header("Charging Settings")]
        public float chargingTime { get; set; } = 0;
        [SerializeField] public float maxchargingTime = 5f;
        [SerializeField] private float chargeAttackSec = 3f;
        
        [Header("Dash Attack Settings")]
        [SerializeField] private float dashSpeed = 35f;
        [SerializeField] private float dashDuration = 0.25f;
        public bool isDashAttacking { get; set; } = false;
        public float _timer { get; set; } = 0;
        
        [Header("Guided Attack Settings")]
        [SerializeField] private float guidedSpeed = 25f;
        [SerializeField] private float guidedStopDistance = 2.5f;
        
        private CharacterMovement _movementCompo;
        private Coroutine _timerCoroutine;

        public void Initialize(Entity entity)
        {
            _player = entity as Player;
            
            if (_player == null)
            {
                UnityLogger.LogError("PlayerAttack는 Player 엔티티에만 사용할 수 있습니다.");
                return;
            }
            
            _movementCompo = _player.GetCompo<CharacterMovement>();
            
            if (_movementCompo == null)
            {
                UnityLogger.LogError("CharacterMovement 컴포넌트를 찾을 수 없습니다.");
            }
            
            if (_player.inputReader != null)
            {
                _player.inputReader.AttackEvent += HandleAttack;
                _player.inputReader.ChargingEvent += HandleCharge;
                _player.inputReader.ChargingAttackEvent += HandleChargeAttack;
                _player.inputReader.AttackEndEvent += HandleAttackEnd;
            }
            else
            {
                UnityLogger.LogError("InputReader가 할당되지 않았습니다.");
            }
        }

        private void OnDestroy()
        {
            if (_player != null && _player.inputReader != null)
            {
                _player.inputReader.AttackEvent -= HandleAttack;
                _player.inputReader.ChargingEvent -= HandleCharge;
                _player.inputReader.ChargingAttackEvent -= HandleChargeAttack;
                _player.inputReader.AttackEndEvent -= HandleAttackEnd;
            }
        }

        private void HandleCharge()
        {
            _player.ChangeState("CHARGE");
        }

        private void HandleAttack()
        {
            if (_player.bloodSystemCompo != null && _player.bloodSystemCompo.isFallingFlower)
            {
                HandleCharge();
            }
            else
            {
                if (_player.swordCompo != null)
                {
                    _player.swordCompo.Attack();
                }
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
            
            if (_player.aimmingComponent != null && _player.aimmingComponent.aimingObject != null)
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
            if (_player.bloodSystemCompo != null && _player.bloodSystemCompo.isFallingFlower)
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
        }

        public void GuidedAttack()
        {
            if (_movementCompo == null)
            {
                UnityLogger.LogError("CharacterMovement가 없어 GuidedAttack을 실행할 수 없습니다.");
                _player.ChangeState("IDLE");
                return;
            }
            
            if (_player.aimmingComponent == null || _player.aimmingComponent.aimingObject == null)
            {
                _movementCompo.StopGuidedMovement();
                _player.ChangeState("IDLE");
                return;
            }
            
            if (!_movementCompo.isGuidedMovement)
            {
                _movementCompo.SetGuidedMovement(
                    _player.aimmingComponent.aimingObject.transform, 
                    guidedSpeed, 
                    guidedStopDistance
                );
            }
            
            if (_movementCompo.IsGuidedMovementComplete())
            {
                _movementCompo.StopGuidedMovement();
                _player.ChangeState("IDLE");
            }
        }

        public void Dash()
        {
            if (_movementCompo == null)
            {
                UnityLogger.LogError("CharacterMovement가 없어 Dash를 실행할 수 없습니다.");
                _player.ChangeState("IDLE");
                return;
            }
            
            if (!_movementCompo.isImpulseActive)
            {
                Vector3 dashDirection = _player.transform.forward;
                _movementCompo.ApplyImpulse(dashDirection, dashSpeed, dashDuration);
                isDashAttacking = true;
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