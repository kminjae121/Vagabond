using Code.Core.Debugs;
using Code.Entities;
using Code.Interfaces;
using UnityEngine;

namespace _01.Member.KMJ._02.Scripts._01.Player.Climing
{
    public class WallClimbingCompo : MonoBehaviour, IEntityComponent
    {
        [Header("Detection Settings")]
        [SerializeField] private Vector3 _climingSize;
        [SerializeField] private Vector3 _detectedOutSize;
        [SerializeField] private Transform _detectedTrm;
        [SerializeField] private Transform _endTrm;
        [SerializeField] private LayerMask _detectedLayer;

        [Header("Bloodthief Style - Climbing Settings")]
        [SerializeField] private float speed = 12f;
        [SerializeField] private float _waitTime = 0.06f;

        [Header("Bloodthief Style - Vault Settings")]
        [SerializeField] private float vaultUpForce = 10f;
        [SerializeField] private float vaultForwardForce = 15f;
        [SerializeField] private float vaultDuration = 0.3f;
    
        private CharacterController _controller;
        private CharacterMovement _movement;
        private float _currentTime = 0;
        private Player _player;
        private Vector3 climbVelocity = Vector3.zero;
    
        public void Initialize(Entity entity)
        {
            _player = entity as Player;
            _controller = entity.GetComponent<CharacterController>();
            _movement = entity.GetCompo<CharacterMovement>();
        
            if (_controller == null)
            {
                UnityLogger.LogError("CharacterController를 찾을 수 없습니다.");
            }
        
            if (_movement == null)
            {
                UnityLogger.LogError("CharacterMovement를 찾을 수 없습니다.");
            }
        }

        public bool CanClimbWall()
        {
            Collider[] hits = Physics.OverlapBox(_detectedTrm.position, _climingSize, Quaternion.identity, _detectedLayer);
            return hits.Length > 0;
        }

        public void ClimingWall()
        {
            Collider[] hits = Physics.OverlapBox(_detectedTrm.position, _climingSize, Quaternion.identity, _detectedLayer);
        
            if (hits.Length > 0)
            {
                if (_player.movementCompo != null)
                {
                    _player.movementCompo.StopMoving();
                }
            
                climbVelocity = Vector3.zero;
                _player.ChangeState("CLIMBWALL");       
            }
        }

        public void Climbing()
        {
            if (_controller == null || _movement == null) return;
        
            Collider[] hits = Physics.OverlapBox(_endTrm.position, _detectedOutSize, Quaternion.identity, _detectedLayer);

            if (hits.Length != 0)
            {
                _currentTime = 0;
                climbVelocity = transform.up * speed;
                _controller.Move(climbVelocity * Time.fixedDeltaTime);
            }
            else
            {
                if (_player.movementCompo != null)
                {
                    _player.movementCompo.StopMoving();
                }
            
                _currentTime += Time.fixedDeltaTime;

                if (_currentTime >= _waitTime)
                {
                    VaultOver();
                }
            }
        }
    
        private void VaultOver()
        {
            if (_movement == null)
            {
                UnityLogger.LogError("CharacterMovement가 없어 Vault를 실행할 수 없습니다.");
                _player.ChangeState("IDLE");
                return;
            }
        
            Vector3 vaultDirection = (transform.up * vaultUpForce + transform.forward * vaultForwardForce).normalized;
            float vaultMagnitude = Mathf.Sqrt(vaultUpForce * vaultUpForce + vaultForwardForce * vaultForwardForce);
        
            _movement.ApplyImpulse(vaultDirection, vaultMagnitude, vaultDuration);
        
            _player.ChangeState("IDLE");
            _currentTime = 0;
        }

        private void OnDrawGizmos()
        {
            if (_detectedTrm == null || _endTrm == null) return;
        
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(_detectedTrm.position, _climingSize);
        
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(_endTrm.position, _detectedOutSize);
        }
    }
}