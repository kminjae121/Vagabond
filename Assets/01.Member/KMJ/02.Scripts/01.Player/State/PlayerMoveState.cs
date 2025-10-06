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
            _player._movementCompo.SetSpeed(_movementCompo.maxmoveSpeed);
            base.Enter();
        }

        public override void Update()
        {
            _movementCompo.SetMove(_movementCompo._inputReader.MoveValue.x, _movementCompo._inputReader.MoveValue.y);

            if (_movementCompo._inputReader.MoveValue == Vector2.zero)
            {
                _player.ChangeState("IDLE");
                _movementCompo.StopMoving();
            }
        }

        public override void FixedUpdate()
        {
            _rbCompo.linearVelocity = new Vector3(_entity.transform.TransformDirection(_movementCompo._move).x * _movementCompo.moveSpeed,
                _rbCompo.linearVelocity.y, _entity.transform.TransformDirection(_movementCompo._move).z * _movementCompo.moveSpeed);
        }

        public override void Exit()
        {
            base.Exit();
            _player._movementCompo.SetReturnOriginMoveSpeed();
        }
    }
}