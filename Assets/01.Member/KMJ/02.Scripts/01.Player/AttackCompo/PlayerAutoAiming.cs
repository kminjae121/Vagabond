using System;
using System.ComponentModel;
using _01.Member.KMJ._02.Scripts.Enemy;
using Code.Core.Debugs;
using Code.Entities;
using Code.Interfaces;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.UI;

namespace _01.Member.KMJ._02.Scripts._01.Player.AttackCompo
{
    public class PlayerAutoAiming : MonoBehaviour, IEntityComponent
    {
        [SerializeField] private LayerMask whatIsEnemy;

        [SerializeField] private GameObject aimUI;
        
        [SerializeField] private Image uiImage;

        private Player _player;
        private float currentAimmingTime = 0f;
        [field: SerializeField] public GameObject aimingObject { get; set; }

        private EnemyAimUI _aimUI;
        private Transform defaultTarget;
        private bool isLockedOn = false;

        public void Initialize(Entity entity)
        {
            _player = entity as Player;
            _aimUI = aimUI.GetComponent<EnemyAimUI>();
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
                if (!_player.atkComponent.isDashAttacking && hit.transform.gameObject != null)
                { 
                    uiImage.color = Color.white;
                    SetAIActive(true);
                    CheckIsTimeOver(hit);
                }
            }
            else if (aimingObject != null)
            {
                if (aimingObject.TryGetComponent(out EnemyAimed aimed))
                {
                    SetAIActive(false);
                    uiImage.color = Color.white;
                    SetEnemyNull();
                    aimed.StartCoroutineInScript();
                }
            }
            else
            {
                uiImage.color = Color.white;
                SetAIActive(false);
            }
        }

        private void CheckIsTimeOver(RaycastHit hit)
        {
            if (hit.transform.gameObject.TryGetComponent(out EnemyAimed aimed))
            {
                aimed.AimmingThis();

                if (aimed.isTarget)
                {
                    _aimUI._isBoosted = true;
                    uiImage.color = Color.red;
                    aimingObject = hit.collider.gameObject;
                }
            }
        }

        public void SetAIActive(bool isActive)
        {
            aimUI.SetActive(isActive);
        }

        public void SetEnemyNull()
        {
            aimingObject = null;
        }
    }
}
