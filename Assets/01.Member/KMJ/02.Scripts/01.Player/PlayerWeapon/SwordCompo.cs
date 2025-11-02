
using System.Collections;
using _01.Member.KMJ._00.Core._01.Entity._02.EntityCompo;
using _01.Member.KMJ._02.Scripts._01.Player.AttackCompo;
using Code.Core._02.Sound;
using Code.Core.GameEvent;
using Code.Core.Stats;
using Code.Entities;
using Code.Interfaces;
using GameEvents;
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

        [SerializeField] private float bloodGageAmount = 60f;
        
        
        [SerializeField] private GameEventChannelSO _soundChannel;
        [SerializeField] private SoundSO bloodSound;
        private float _atkDamage;

        private Entity _owner;

        private Coroutine _timeScaleCoroutine;
        
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
                        var sfxEvt = SoundEvents.PlaySFXEvent.Initializer(transform.position,bloodSound);
                        _soundChannel.RaiseEvent(sfxEvt);
                        DamageData data = new DamageData();
                        data.damage = 99999;
                        data.damageType = DamageType.MELEE;

                        damageable.ApplyDamage(data, other.transform.position, _owner.transform.forward, weaponAtkData,
                            _owner);

                        StartCoroutine(TimeScale());
                        //player.aimmingComponent.SetUIActive(false);
                        player.bloodSystemCompo.AddFlower((int)bloodGageAmount);
                        player.aimmingComponent.SetEnemyNull();
                    }
                }
            }
            else if (((1 << other.gameObject.layer) & whatIsEnemy) != 0 && ((1 << gameObject.layer) & baldoWeaponLayer) != 0)
            {
                if (other.TryGetComponent(out IDamageable damageable))
                {
                    var sfxEvt = SoundEvents.PlaySFXEvent.Initializer(transform.position,bloodSound);
                    _soundChannel.RaiseEvent(sfxEvt);
                    DamageData data = new DamageData();
                    
                    data.damage = 99999;
                    data.damageType = DamageType.MELEE;
                    
                    damageable.ApplyDamage(data, other.transform.position, _owner.transform.forward, weaponAtkData,_owner);
                    player.aimmingComponent.SetUIActive(false);
                    
                    StartCoroutine(TimeScale());
                    player.bloodSystemCompo.AddFlower((int)bloodGageAmount);       
                    
                    
                    
                    //cutter.Cut(meshTarget,other.transform.position, _owner.transform.forward);
                }
            }
        }

        private IEnumerator TimeScale()
        {
            Time.timeScale = 0.4f;
            yield return new WaitForSeconds(0.08f);
            Time.timeScale = 1;
        }
    }
}
