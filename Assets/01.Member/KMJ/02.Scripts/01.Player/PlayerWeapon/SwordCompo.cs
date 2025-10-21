using System;
using _01.Member.KMJ._00.Core._01.Entity._02.EntityCompo;
using _01.Member.KMJ._02.Scripts._01.Player.AttackCompo;
using Code.Core.Debugs;
using Code.Core.Stats;
using Code.Entities;
using Code.Interfaces;
using DynamicMeshCutter;
using UnityEngine;

namespace _01.Member.KMJ._02.Scripts._01.Player.PlayerWeapon
{
    public class SwordCompo : MonoBehaviour, IEntityComponent
    {
        [Space(5)]
        [Header("EnemyLayer")]
        [SerializeField] private LayerMask whatIsEnemy;
        [SerializeField] private LayerMask baldoWeaponLayer;
        
        [Space(5)]
        [Header("AttackData")]
        [SerializeField] private AttackDataSO weaponAtkData;
            
        [Header("Stat")]
        [SerializeField] private StatSO atkDamageStat;

        [SerializeField] private EntityStatCompo statCompo;

        [SerializeField] private Player player;
        private float _atkDamage;

        private Entity _owner;
        
        private DamageData damageData;

        public void Initialize(Entity entity)
        {
            _owner = entity;
            
            
        }

        private void Start()
        {
            damageData = new DamageData();

            damageData.damage = statCompo.GetStat(atkDamageStat).Value;
            
            damageData.damageType = DamageType.MELEE;
            
        }

        private void HandleAttackDamageChange(StatSO stat, float currentvalue, float previousvalue)
        {
            _atkDamage = currentvalue;
        }
        
        private void OnTriggerEnter(Collider other)
        {
           if (((1 << other.gameObject.layer) & whatIsEnemy) != 0 && ((1 << gameObject.layer) & baldoWeaponLayer) != 0 && player.aimmingComponent.aimingObject != null)
            {
                if (other.gameObject == player.aimmingComponent.aimingObject)
                {
                    if (other.TryGetComponent(out IDamageable damageable))
                    {
                        
                        DamageData data = new DamageData();
                        data.damage = 99999;
                        data.damageType = DamageType.MELEE;

                        damageable.ApplyDamage(data, other.transform.position, _owner.transform.forward, weaponAtkData,
                            _owner);
                        player.aimmingComponent.SetAIActive(false);
                        player.bloodSystemCompo.AddFlower(1);
                        player.aimmingComponent.SetEnemyNull();
                    }
                }
            }
            else if (((1 << other.gameObject.layer) & whatIsEnemy) != 0 && ((1 << gameObject.layer) & baldoWeaponLayer) != 0)
            {
                if (other.TryGetComponent(out IDamageable damageable))
                {
                    DamageData data = new DamageData();
                    var cutter = GetComponent<PlaneBehaviour>();

                    MeshTarget meshTarget = other.GetComponentInChildren<MeshTarget>();
                    
                    data.damage = 99999;
                    data.damageType = DamageType.MELEE;
                    
                    damageable.ApplyDamage(data, other.transform.position, _owner.transform.forward, weaponAtkData,_owner);
                    player.aimmingComponent.SetAIActive(false);
                    
                    
                    
                    player.bloodSystemCompo.AddFlower(1);       
                    
                    cutter.Cut(meshTarget,other.transform.position, _owner.transform.forward);
                }
            }
            else if (((1 << other.gameObject.layer) & whatIsEnemy) != 0)
            {
                if (other.TryGetComponent(out IDamageable damageable))
                {
                    damageable.ApplyDamage(damageData, other.transform.position, _owner.transform.forward, weaponAtkData,_owner);
                }
            }
        }
    }
}
