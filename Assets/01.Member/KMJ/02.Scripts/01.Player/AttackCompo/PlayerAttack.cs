using System;
using System.Collections;
using _00.CORE._02.Scripts.Input;
using _01.Member.KMJ._00.Core._01.Entity._05.Interface;
using UnityEngine;

namespace _01.Member.KMJ._02.Scripts._01.Player.AttackCompo
{
    public class PlayerAttack : MonoBehaviour, IEntityComponent
    {
        [SerializeField] private InputReader _inputReader;
        private Player _player;
        
        public float chargingTime { get; set; }
        public float maxchargingTime { get; set; }

        private float chargeAttackSec;

        private Coroutine _timerCoroutine;

        public void Initialize(Entity entity)
        {
            _inputReader.AttackEvent += HandleAttack;
            _inputReader.ChargingEvent += HandleCharge;
            _inputReader.ChargingAttackEvent += HandleChargeAttack;   
            _player = entity as Player;
        }

        private void OnDestroy()
        {
            _inputReader.AttackEvent -= HandleAttack;
            _inputReader.ChargingEvent -= HandleCharge;
            _inputReader.ChargingAttackEvent -= HandleChargeAttack;
        }

        private void HandleCharge()
        {
            _player.ChangeState("CHARGE");
        }

        private void HandleAttack()
        {
            Debug.Log("공격");
        }

        private void HandleChargeAttack()
        {
            if (chargingTime >= chargeAttackSec)
            {
                _player.ChangeState("CHARGEATTACK");
            }
        }

        public void StartChargingTimer()
        {
            if (_timerCoroutine != null)
            {
                _timerCoroutine = StartCoroutine(ChargeAttackSec());
            }
        }
        
        public IEnumerator ChargeAttackSec()
        {
            while (chargingTime >= maxchargingTime)
            {
                chargingTime += 1;
                
                yield return new WaitForSeconds(1f);
            }
        }
    }
}