using Code.Core.Debugs;
using Code.Entities;
using Code.Entities.Combat;
using Code.Interfaces;
using UnityEngine;

namespace Code.Enemies
{
    public class ArcherAttackCompo : MonoBehaviour, IEntityComponent
    {
        [SerializeField] private GameObject arrowPrefab;
        [SerializeField] private float arrowSpeed = 5f;
        
        private Entity _entity;
        
        public void Initialize(Entity entity)
        {
            _entity = entity;
        }

        public void ShootArrow(Vector3 dir)
        {
            Arrow arrow = Instantiate(arrowPrefab, transform.position, Quaternion.LookRotation(dir)).GetComponent<Arrow>();
            arrow.Initialize(dir, arrowSpeed);
            UnityLogger.Log("arrow shoot");
        }
    }
}