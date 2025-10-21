using System;
using _00.CORE._02.Scripts.Input;
using Code.Core.Stats;
using Code.Entities;
using Code.Interfaces;
using UnityEngine;

namespace _01.Member.KMJ._02.Scripts._01.Player.PlayerWeapon
{
    public class PlayerSword : MonoBehaviour, IEntityComponent
    {
        #region  SerializeField
        [Header("Animator")]
        [SerializeField] private Animator animCompo;
        
        [Header("InputComponent")]
        [SerializeField] private InputReader _inputReader;
        
        [Space(5)]
        [Header("StatCompo")]
        [SerializeField] private EntityStatCompo statCompo;
        [SerializeField] private StatSO atkSpeedStat;

        [SerializeField] private LayerMask baseWeaponMask;
        [SerializeField] private LayerMask baldoWeaponMask;
        #endregion
        
        
        private readonly int _cntAttackHash = Animator.StringToHash("ATK_COUNT");
        private readonly int _normalAttackHash = Animator.StringToHash("NORMAL_ATTACK");
        private readonly int _baldoAttackHash = Animator.StringToHash("BALDO_ATTACK");
        private readonly int _nabdoAttackHash = Animator.StringToHash("NABDO_ATTACK");
        private readonly int _swordIdleHash = Animator.StringToHash("SWORD_IDLE");
        private readonly int _attackSpeedHash = Animator.StringToHash("ATTACK_SPEED");
        
        private int _atkCnt = 0;
        
        private float _atkSpeed;
        
        public float AttackSpeed
        {
            get => _atkSpeed;
            set
            {
                _atkSpeed = value;
                animCompo.SetFloat(_attackSpeedHash, _atkSpeed);
            }
        }
        
        [SerializeField] private Collider _weaponCollider;
        

        private bool _isAttacking = false;
        
        public void Initialize(Entity entity)
        {
            animCompo.SetBool(_swordIdleHash, true);
            
        }

        private void Start()
        {
            _weaponCollider.enabled = false;
            
            StatSO target = statCompo.GetStat(atkSpeedStat);
            Debug.Assert(target != null, $"{atkSpeedStat.statName} does not exist");
            target.OnValueChanged += HandleAttackSpeedChange;
            _atkSpeed = target.Value;
            
        }

        private void HandleAttackSpeedChange(StatSO stat, float currentvalue, float previousvalue)
        {
            AttackSpeed = currentvalue;
        }
        

        public void Attack()
        {
            if (_isAttacking)
                return;
            _weaponCollider.enabled = true;
            _isAttacking = true;
            animCompo.SetBool(_swordIdleHash, false);
            animCompo.SetInteger(_cntAttackHash, _atkCnt);
            animCompo.SetBool(_normalAttackHash, true);

            if (_atkCnt >= 2)
            {
                _atkCnt = 0;
            }
            else
             _atkCnt++;
        }

        public void StopAttack()
        {
            _isAttacking = false;
            _weaponCollider.enabled = false;
            animCompo.SetBool(_swordIdleHash, true); 
            animCompo.SetBool(_normalAttackHash, false);
        }

        public void NabDo()
        {
            if (_isAttacking)
                return;

            _isAttacking = true;
            animCompo.SetBool(_swordIdleHash, false);
            animCompo.SetBool(_nabdoAttackHash,true);
        }

        public void BalDo()
        {
            if (_isAttacking)
                return;

            gameObject.layer = Mathf.RoundToInt(Mathf.Log(baldoWeaponMask.value, 2));
            _isAttacking = true;
            _weaponCollider.enabled = true;
            animCompo.SetBool(_baldoAttackHash, true);
            animCompo.SetBool(_swordIdleHash, false);
        }

        public void SetNormalSword()
        {
            gameObject.layer = Mathf.RoundToInt(Mathf.Log(baseWeaponMask.value, 2));
            _weaponCollider.enabled = false;
        }

        public void StopBalDo()
        {
            _isAttacking = false;
            animCompo.SetBool(_swordIdleHash, true); 
            animCompo.SetBool(_baldoAttackHash, false);
        }

        public void StopNabDo()
        {
            _isAttacking = false;
            animCompo.SetBool(_swordIdleHash, true); 
            animCompo.SetBool(_nabdoAttackHash, false);
        }

    }
}