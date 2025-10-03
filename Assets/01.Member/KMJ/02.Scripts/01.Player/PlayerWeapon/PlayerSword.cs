using _00.CORE._02.Scripts;
using UnityEngine;

namespace _01.Member.KMJ._02.Scripts._01.Player.PlayerWeapon
{
    public class PlayerSword : MonoBehaviour
    {
        [Header("Stat")]
        [SerializeField] private StatSO _atkDamageStat;
        [SerializeField] private StatSO _atkSpeedStat;
        
        private float atkDamage;
        private float atkSpeed;
        
        [Space(5)]
        [Header("StatCompo")]
        [SerializeField] private EntityStatCompo _statCompo;
        
        [Space(5)]
        [Header("EnemyLayer")]
        [SerializeField] private LayerMask _whatIsEnemy;
        
        
    }
}