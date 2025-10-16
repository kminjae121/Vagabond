using System;
using System.ComponentModel;
using _01.Member.KMJ._02.Scripts.Enemy;
using Code.Core.Debugs;
using Code.Entities;
using Code.Interfaces;
using UnityEngine;
using Unity.Cinemachine;

namespace _01.Member.KMJ._02.Scripts._01.Player.AttackCompo
{
    public class PlayerAutoAiming : MonoBehaviour, IEntityComponent
    {
        [SerializeField] private LayerMask whatIsEnemy;

        private Player _player;
        private float currentAimmingTime = 0f;
        [field: SerializeField] public GameObject aimingObject { get; set; }
        
        private Transform defaultTarget;
        private bool isLockedOn = false;

        public void Initialize(Entity entity)
        {
            _player = entity as Player;
        }

        private void Update()
        {
        }

        public void ShootRayForCheckEnemy()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) && ((1 << hit.collider.gameObject.layer) & whatIsEnemy) != 0)
            {
                UnityLogger.Log("앙기모띠"); 
                if (!_player.atkComponent.isDashAttacking)
                {
                    CheckIsTimeOver(hit);
                }
            }
            else if(aimingObject != null)
            {
                if (aimingObject.TryGetComponent(out EnemyAimed aimed))
                {
                    SetEnemyNull();
                    aimed.StartCoroutineInScript();
                }
            }
        }

        private void CheckIsTimeOver(RaycastHit hit)
        {
            if (hit.transform.gameObject.TryGetComponent(out EnemyAimed aimed))
            {
                aimed.AimmingThis();
                
                if(aimed.isTarget)
                    aimingObject = hit.collider.gameObject;
            }
        }

        public void SetEnemyNull()
        {
            aimingObject = null;
        }
    }
}
