using _01.Member.KMJ._02.Scripts._01.Player.SlidingCompo;
using Code.Entities;
using UnityEngine;

namespace _01.Member.KMJ._02.Scripts._01.Player.State
{
    public class PlayerWallSlideState : PlayerState
    {
        private WallSliding _slidingCompo;
        private float currentTilt = 0f;

        public PlayerWallSlideState(Entity entity, int animationHash) : base(entity, animationHash)
        {
            _slidingCompo = entity.GetCompo<WallSliding>();
        }

        public override void Enter()
        {
            base.Enter();
            _player.isSliding = false;
            
            string wallSide = _slidingCompo.GetWallSide();
            currentTilt = wallSide == "Left" ? -15f : 15f;
            _player.camCompo.SetTilt(currentTilt);
            
            _slidingCompo.StartWallSlide();
        }

        public override void Update()
        {
            base.Update();
            
            if (_slidingCompo.CanSlidingWall() == "None")
            {
                _player.ChangeState("IDLE");
            }
        }

        public override void Exit()
        {
            base.Exit();
            _player.camCompo.ReturnOwnTilt();
            _slidingCompo.EndWallSlide();
        }
    }
}