
using Code.Entities;
using UnityEngine;

namespace _01.Member.KMJ._02.Scripts._01.Player.State
{
    public class PlayerMoveState : PlayerState
    {
        
        public PlayerMoveState(Entity entity, int animationHash) : base(entity, animationHash)
        {
            
        }

        public override void Enter()
        {
            _player.movementCompo.SetSpeed(_movementCompo.maxmoveSpeed);
            base.Enter();
        }

        public override void Update()
        {
            _movementCompo.SetMove(_movementCompo._inputReader.MoveValue.x, _movementCompo._inputReader.MoveValue.y);

            if (_movementCompo._inputReader.MoveValue == Vector2.zero)
            {
                _player.ChangeState("IDLE");
            }
        }

        public override void FixedUpdate()
        {
            // Bhop Physics를 사용하는 경우 CharacterMovement가 자동으로 처리하므로 제거
            if (_movementCompo.useBhopPhysics)
                return;
        
            Vector3 keyDir = new Vector3(_player.movementCompo._move.x, 0, _player.movementCompo._move.z).normalized;
            Vector3 movement = _player.cameraTrm.forward * keyDir.z + _player.cameraTrm.right * keyDir.x;
            movement *= _movementCompo.moveSpeed;
            movement.y = _rbCompo.linearVelocity.y;
            _rbCompo.linearVelocity = movement;
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}