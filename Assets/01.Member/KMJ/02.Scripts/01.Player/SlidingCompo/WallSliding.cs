using System;
using _01.Member.KMJ._00.Core._01.Entity._05.Interface;
using UnityEngine;

namespace _01.Member.KMJ._02.Scripts._01.Player.SlidingCompo
{
    public class WallSliding : MonoBehaviour, IEntityComponent
    {
        [Header("WallLayerMask")]
        [SerializeField] private LayerMask _whatIsWall;

        [Space(10)]
        [Header("Detected Pos")]
        
        [SerializeField] private Transform leftPos;
        [SerializeField] private Transform rightPos;

        [Header("Ray")]
        [SerializeField] private Vector3 checkSize; 

        public bool _isWallSliding { get; set; }
        
        private Rigidbody _rbCompo;

        private CharacterMovement _movementCompo;
        
        public void Initialize(Entity entity)
        {
            _movementCompo = entity.GetCompo<CharacterMovement>();
            _rbCompo = GetComponent<Rigidbody>();
        }
        

        public string CanSlidingWall()
        {
            Collider[] leftHit = Physics.OverlapBox(leftPos.position, checkSize, Quaternion.identity, _whatIsWall);
            
            Collider[] rightHit = Physics.OverlapBox(rightPos.position, checkSize, Quaternion.identity, _whatIsWall);

            if (leftHit.Length > 0)
            {
                return "Left";
            }
            if(rightHit.Length > 0)
            {
                return "Right";
            }
            else
            {
                return "None";
            }
        }

        public void StartWallSlide()
        {
            _isWallSliding = true;
            _rbCompo.useGravity = false;
            _movementCompo.StopMoving();    
        }

        public void WallSlide()
        {
            Vector3 forwardDir = transform.forward;
            
            _rbCompo.linearVelocity = new Vector3(forwardDir.x * _movementCompo.moveSpeed,
                _rbCompo.linearVelocity.y, forwardDir.z * _movementCompo.moveSpeed
            );
        }


        public void EndWallSlide()
        {
            _isWallSliding = false;
            _rbCompo.useGravity = true;
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(leftPos.position, checkSize);
            Gizmos.DrawWireCube(rightPos.position,checkSize);
            Gizmos.color = Color.white;
        }
    }
}