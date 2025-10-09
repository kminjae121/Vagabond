using _01.Member.KMJ._00.Core._01.Entity._02.EntityCompo;
using _01.Member.KMJ._02.Scripts._01.Player.AttackCompo;
using Code.Core.Debugs;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Code.Entities.Test
{
    public class EnemyHitTest : MonoBehaviour
    {
        [SerializeField] private EntityHealth enemy;

        private void Update()
        {
            if (Keyboard.current.hKey.wasPressedThisFrame)
            {
                UnityLogger.Log("hit test");
                
                DamageData data = new DamageData
                {
                    damage = 1,
                    isCritical = false,
                    damageType = DamageType.MELEE
                };

                AttackDataSO atkSO = ScriptableObject.CreateInstance<AttackDataSO>();
                atkSO.isPowerAttack = false;
                
                enemy.ApplyDamage(data, Vector3.zero, Vector3.zero, atkSO, null);
            }
        }
    }
}