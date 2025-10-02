using System;
using _00.CORE._02.Scripts.Input;
using UnityEngine;

namespace _01.Member.KMJ._02.Scripts._01.Player.AttackCompo
{
    public class PlayerAttack : MonoBehaviour
    {
        private InputReader _inputReader;

        private void Awake()
        {
            _inputReader.AttackEvent += HandleAttack;
            _inputReader.ChargingEvent += HandleCharge;
            _inputReader.ChargingAttackEvent += HandleChargeAttack;
        }

        private void OnDestroy()
        {
            _inputReader.AttackEvent -= HandleAttack;
            _inputReader.ChargingEvent -= HandleCharge;
            _inputReader.ChargingAttackEvent -= HandleChargeAttack;
        }

        private void HandleCharge()
        {
            Debug.Log("차징 공격중");
        }

        private void HandleAttack()
        {
            Debug.Log("공격");
        }

        private void HandleChargeAttack()
        {
            Debug.Log("발도!" +
                      "" +
                      "");
        }
    }
}