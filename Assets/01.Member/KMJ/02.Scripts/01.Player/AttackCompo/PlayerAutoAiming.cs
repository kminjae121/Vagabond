using System;
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
        [SerializeField] private float aimmingFullTime;
        
        [SerializeField] private CinemachineCamera playerCam;
        
        [SerializeField] private CinemachinePanTilt panTilt;

        private Player _player;
        private float currentAimmingTime = 0f;
        private GameObject aimingObject;
        
        private Transform defaultTarget;
        private bool isLockedOn = false;

        public void Initialize(Entity entity)
        {
            _player = entity as Player;
        }

        private void Update()
        {
            
        }

        private void FixedUpdate()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
        
            if (Physics.Raycast(ray, out hit))
            {
                if (((1 << hit.collider.gameObject.layer) & whatIsEnemy) != 0)
                {
                    UnityLogger.Log("시작됨");
                    currentAimmingTime += Time.deltaTime;
                }
            }
            else
            {
                aimingObject = null;
                currentAimmingTime = 0;
                UnlockCamera(); 
            }
                
            if (currentAimmingTime >= aimmingFullTime)
            {
                if (Physics.Raycast(ray, out hit))
                {
                    if (((1 << hit.collider.gameObject.layer) & whatIsEnemy) != 0 && aimingObject == null)
                    {
                        aimingObject = hit.collider.gameObject;
                    }
                }
            }
            
            if (aimingObject != null)
            {
                LockCamera(aimingObject.transform);
                
            }

            if (aimingObject == null)
            {
                UnlockCamera();
            }
        }

        private void LockCamera(Transform target)
        {
            if (playerCam == null || isLockedOn) return;

            if (panTilt != null)
                panTilt.enabled = false;

            playerCam.transform.LookAt(target);
            //isLockedOn = true;
        }
        
       
        private void UnlockCamera()
        {
            if (playerCam == null || !isLockedOn) return;

            playerCam.LookAt = defaultTarget;

            if (panTilt != null)
                panTilt.enabled = true; 

            isLockedOn = false;
        }
    }
}
