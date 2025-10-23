using Code.Entities;
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
           // base.Enter();
            _player.isSliding = false;
            _player.SetJumping(true);
            
           // _player.swordCompo.SetNormalSword();
        }

        public override void Update()
        {
            if (_player.inputReader != null && _player.inputReader.MoveValue != Vector2.zero)
            {
                _animatorCompo.SetAllBoolParamFalse();
                _animatorCompo.animator.SetBool("MOVE", true);
                _player.ChangeState("MOVE");
            }
            
            base.Update();
        }
    }
}