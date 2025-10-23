using Code.Entities;
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
            //base.Enter();
            _player.isSliding = false;
            _movementCompo.Jump();
            // 점프 실행 후 바로 IDLE로 전환
            _player.ChangeState("IDLE");
        }

        public override void Update()
        {
            base.Update();
            // Update에서 상태 전환 제거
        }
    }
}