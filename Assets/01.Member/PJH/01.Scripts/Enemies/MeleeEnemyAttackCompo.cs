using _01.Member.KMJ._00.Core._01.Entity._02.EntityCompo;
using _01.Member.KMJ._02.Scripts._01.Player.AttackCompo;
using Code.Core.Debugs;
using Code.Core.Stats;
using Code.Entities;
using Code.Entities.Combat;
using Code.Interfaces;
using UnityEngine;

namespace Code.Enemies
{
    public class MeleeEnemyAttackCompo : MonoBehaviour, IEntityComponent, IAfterInitialize
    {
        [SerializeField] private AttackDataSO attackData;
        [SerializeField] private StatSO meleeDamageStat;
        [SerializeField] private OverlapDamageCaster[] casters;
        
        private Entity _entity;
        private EntityStatCompo _statCompo;
        private EntityAnimatorTrigger _trigger;
        private DamageData _currentDamageData;
        private bool _isActive;
        
        public void Initialize(Entity entity)
        {
            _entity = entity;
            _statCompo = entity.GetCompo<EntityStatCompo>();
            _trigger = entity.GetCompo<EntityAnimatorTrigger>();

            casters = entity.GetComponentsInChildren<OverlapDamageCaster>(true);

            foreach (var caster in casters)
                caster.InitCaster(entity);
        }

        public void AfterInitialize()
        {
            meleeDamageStat = _statCompo.GetStat(meleeDamageStat);
            _trigger.OnDamageToggleTrigger += SetDamageCaster;
            
            UnityLogger.Log("melee enemy attack compo Init 완료");
        }

        private void OnDestroy()
        {
            _trigger.OnDamageToggleTrigger -= SetDamageCaster;
        }

        private void SetDamageCaster(bool isActive)
        {
            _isActive = isActive;

            if (!isActive)
                return;
            
            foreach (var caster in casters)
                caster.StartCasting();

            _currentDamageData = new DamageData
            {
                damage = meleeDamageStat.Value,
                damageType = attackData.damageType
            };
        }

        private void FixedUpdate()
        {
            if (!_isActive)
                return;
            
            UnityLogger.Log("fixed 들어감");
            


            foreach (var caster in casters)
            {
                caster.CastDamage(_currentDamageData, transform.position,
                    transform.forward, attackData);
            }
        }
    }
}