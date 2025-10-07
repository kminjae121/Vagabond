using _00.CORE._02.Scripts;
using _01.Member.KMJ._00.Core._01.Entity._02.EntityCompo;
using _01.Member.KMJ._00.Core._01.Entity._05.Interface;
using _01.Member.KMJ._02.Scripts._01.Player.AttackCompo;
using UnityEngine;

namespace _01.Member.KMJ._02.Scripts._01.Player.PlayerWeapon
{
    public class SwordCompo : MonoBehaviour, IEntityComponent
    {
        [Space(5)]
        [Header("EnemyLayer")]
        [SerializeField] private LayerMask whatIsEnemy;
        
        [Space(5)]
        [Header("AttackData")]
        [SerializeField] private AttackDataSO weaponAtkData;
            
        [Header("Stat")]
        [SerializeField] private StatSO atkDamageStat;
        
        private float _atkDamage;

        private Entity _owner;
        
        private DamageData damageData;
        

        public void Initialize(Entity entity)
        {
            _owner = entity;
            
            
            damageData = new DamageData();

            damageData.damage = _atkDamage;
            damageData.damageType = DamageType.MELEE;
        }
        
        private void HandleAttackDamageChange(StatSO stat, float currentvalue, float previousvalue)
        {
            _atkDamage = currentvalue;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (((1 << other.gameObject.layer) & whatIsEnemy) != 0)
            {
                if (other.TryGetComponent(out IDamageable damageable))
                {
                    damageable.ApplyDamage(damageData, other.transform.position, _owner.transform.forward, weaponAtkData, _owner);
                }
            }
        }
    }
}
