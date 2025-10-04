using System.Globalization;
using _00.CORE._02.Scripts;
using _00.CORE._02.Scripts.GameEventChannel;
using _01.Member.KMJ._00.Core._01.Entity._02.EntityCompo;
using _01.Member.KMJ._00.Core._01.Entity._05.Interface;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

namespace _01.Member.KMJ._02.Scripts._01.Player.AttackCompo
{
    public class EntityHealth : MonoBehaviour, IEntityComponent, IDamageable, IAfterInitialize
    {
        private Entity _entity;
        private ActionData _actionData;
        private EntityStatCompo _statCompo;

        [SerializeField] private StatSO hpStat;
        [SerializeField] private float maxHealth;
        [SerializeField] private float currentHealth;

        public float CurrentHealth => currentHealth;
        public float MaxHealth => maxHealth;
        
        
        public delegate void OnHealthChanged(float current, float max);

        public event OnHealthChanged OnHealthChangedEvent;

        public void Initialize(Entity entity)
        {
            _entity = entity;
            _actionData = entity.GetCompo<ActionData>();
            _statCompo = entity.GetCompo<EntityStatCompo>();
        }
        
        public void AfterInitialize()
        {
            maxHealth = currentHealth = _statCompo.SubscribeStat(
                hpStat, HandleMaxHPChanged, 10f);
        }

        private void OnDestroy()
        {
            _statCompo.UnSubscribeStat(hpStat, HandleMaxHPChanged);
        }

        private void HandleMaxHPChanged(StatSO stat, float currentvalue, float previousvalue)
        {
            float changed = currentvalue - previousvalue; //얼마만큼 변했는지를 측정
            maxHealth = currentvalue;
            if (changed > 0)
                currentHealth = Mathf.Clamp(currentHealth + changed, 0, maxHealth);
            else
                currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        }

        public void ApplyDamage(DamageData damageData, Vector3 hitPoint, Vector3 hitNormal, AttackDataSO attackData, Entity dealer)
        {
            _actionData.HitNormal = hitNormal;
            _actionData.HitPoint = hitPoint;
            _actionData.HitByPowerAttack = attackData.isPowerAttack;
            _actionData.LastDamageData = damageData; 

            currentHealth = Mathf.Clamp(currentHealth - damageData.damage, 0, maxHealth);

            OnHealthChangedEvent?.Invoke(currentHealth, maxHealth);
            
            //int typeHash = damageData.isCritical ? criticalText.nameHash : normalText.nameHash;
            Vector3 position = hitPoint + new Vector3(0, 1.5f);

            

            if (currentHealth <= 0)
            {
                _entity.OnDeathEvent?.Invoke();
            }
            
            _entity.OnHitEvent?.Invoke();
        }
    }
}