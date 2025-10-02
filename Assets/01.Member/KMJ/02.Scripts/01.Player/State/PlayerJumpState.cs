using UnityEngine;

namespace _01.Member.KMJ._02.Scripts._01.Player.State
{
    public class PlayerJumpState : PlayerState
    {
        public PlayerJumpState(Entity entity, int animationHash) : base(entity, animationHash)
        {
        }

        public override void Enter()
        {
            base.Enter();
            _movementCompo.Jump();  
        }

        public override void Update()
        {
            base.Update();
            _player.ChangeState("IDLE");
        }
    }
}