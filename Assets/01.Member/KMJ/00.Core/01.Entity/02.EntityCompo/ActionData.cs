using _01.Member.KMJ._00.Core._01.Entity._05.Interface;
using UnityEngine;

namespace _01.Member.KMJ._00.Core._01.Entity._02.EntityCompo
{
    public class ActionData : MonoBehaviour, IEntityComponent
    {
        public Vector3 HitPoint { get; set; }
        public Vector3 HitNormal { get; set; }
        public bool HitByPowerAttack { get; set; }
        public DamageData LastDamageData { get; set; } 

        private global::Entity _entity;
        public void Initialize(global::Entity entity)
        {
            _entity = entity;
        }
        
    }
}