using _01.Member.KMJ._02.Scripts._01.Player.State;
using Code.Entities;
using UnityEngine;

namespace _01.Member.KMJ._02.Scripts._01.Player.State
{
    public class PlayerClimbingWallState : PlayerState
    {
        public PlayerClimbingWallState(Entity entity, int animationHash) : base(entity, animationHash)
        {
        }

        public override void Enter()
        {
            base.Enter();
        }

        public override void Update()
        {
            base.Update();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            
            if (_player.climbingComponent != null)
            {
                _player.climbingComponent.Climbing();
            }
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}