using _00.CORE._02.Scripts;
using _01.Member.KMJ._00.Core._01.Entity._05.Interface;
using UnityEngine;

namespace _01.Member.KMJ._02.Scripts._01.Player
{
    public class CharacterMovement : MonoBehaviour, IEntityComponent
    {
        [SerializeField] private StatSO moveSpeedStat;
        public void Initialize(Entity entity)
        {
            
        }
    }
}