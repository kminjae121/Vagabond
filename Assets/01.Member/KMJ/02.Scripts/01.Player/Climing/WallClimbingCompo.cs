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

        [Header("Climbing Settings")]
        [SerializeField] private float speed = 5;
        [SerializeField] private float _waitTime = 0.06f;

        [Header("Vault Settings")]
        [SerializeField] private float vaultUpForce = 5f;
        [SerializeField] private float vaultForwardForce = 7.5f;
    
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
                UnityLogger.LogError("[WallClimbingCompo] CharacterController를 찾을 수 없습니다.");
            }
        
            if (_movement == null)
            {
                UnityLogger.LogError("[WallClimbingCompo] CharacterMovement를 찾을 수 없습니다.");
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
                _player.movementCompo.StopMoving();
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
                _player.movementCompo.StopMoving();
                _currentTime += Time.fixedDeltaTime;

                if (_currentTime >= _waitTime)
                {
                    VaultOver();
                }
            }
        }
    
        private void VaultOver()
        {
            Vector3 vaultVelocity = transform.up * vaultUpForce + transform.forward * vaultForwardForce;
            _controller.Move(vaultVelocity * Time.fixedDeltaTime);
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