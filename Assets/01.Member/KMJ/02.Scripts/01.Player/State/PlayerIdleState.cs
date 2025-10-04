using UnityEngine;

namespace _01.Member.KMJ._02.Scripts._01.Player.State
{
    public class PlayerIdleState : PlayerState
    {
        public PlayerIdleState(Entity entity, int animationHash) : base(entity, animationHash)
        {
        }

        public override void Enter()
        {
            base.Enter();
            _player.SetJumping(true);
        }

        public override void Update()
        {
            if (_movementCompo._inputReader.MoveValue != Vector2.zero)
            {
                _player.ChangeState("MOVE");
            }
            base.Update();
        }
    }
}