using System;
using _00.CORE._02.Scripts;
using _01.Member.KMJ._00.Core._01.Entity._02.EntityCompo;
using _01.Member.KMJ._00.Core._01.Entity._05.Interface;
using _01.Member.KMJ._02.Scripts._01.Player.AttackCompo;
using UnityEngine;

namespace _01.Member.KMJ._02.Scripts._01.Player.PlayerWeapon
{
    public class PlayerSword : MonoBehaviour, IEntityComponent
    {
        [Header("Stat")]
        [SerializeField] private StatSO _atkDamageStat;
        [SerializeField] private StatSO _atkSpeedStat;
        
        
        [Space(5)]
        [Header("StatCompo")]
        [SerializeField] private EntityStatCompo _statCompo;
        
        [Space(5)]
        [Header("EnemyLayer")]
        [SerializeField] private LayerMask _whatIsEnemy;

        [Space(5)]
        [Header("AttackData")]
        [SerializeField] private AttackDataSO _weaponAtkData;
        
        private float _atkDamage;
        private float _atkSpeed;
        
        private Collider _weaponCollider;

        private Entity _owner;

        private DamageData damageData;
        
        public void Initialize(Entity entity)
        {
            _owner = entity;
            
            _weaponCollider.enabled = false;
            
            _atkDamage = _statCompo.SubscribeStat(_atkDamageStat, HandleAttackDamageChange, 4f);
            
            _atkSpeed = _statCompo.SubscribeStat(_atkSpeedStat, HandleAttackSpeedChange, 3f);

            damageData = new DamageData();

            damageData.damage = _atkDamage;
            damageData.damageType = DamageType.MELEE;
        }
        
        private void HandleAttackDamageChange(StatSO stat, float currentvalue, float previousvalue)
        {
            _atkDamage = currentvalue;
        }

        private void HandleAttackSpeedChange(StatSO stat, float currentvalue, float previousvalue)
        {
            _atkSpeed = currentvalue;
        }

        public void StartAttack()
        {
            _weaponCollider.enabled = true;
        }

        public void StopAttack()
        {
            _weaponCollider.enabled = false;
        }


        private void OnTriggerEnter(Collider other)
        {
            if (((1 << other.gameObject.layer) & _whatIsEnemy) != 0)
            {
                if (other.TryGetComponent(out IDamageable damageable))
                {
                    damageable.ApplyDamage(damageData, other.transform.position, _owner.transform.forward, _weaponAtkData, _owner);
                }
            }
        }

    }
}