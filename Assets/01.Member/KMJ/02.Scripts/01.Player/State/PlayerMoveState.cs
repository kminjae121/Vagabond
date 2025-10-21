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
           // base.Enter();
        }

        public override void Update()
        {
            if (_player.inputReader != null && _player.inputReader.MoveValue == Vector2.zero)
            {
                _player.ChangeState("IDLE");
            }
            
            base.Update();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        public override void Exit()
        {
           // base.Exit();
        }
    }
}