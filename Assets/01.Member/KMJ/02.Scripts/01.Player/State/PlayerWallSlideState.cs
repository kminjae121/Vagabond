using _01.Member.KMJ._02.Scripts._01.Player.SlidingCompo;
using Code.Entities;
using UnityEngine;

namespace _01.Member.KMJ._02.Scripts._01.Player.State
{
    public class PlayerWallSlideState : PlayerState
    {
        private WallSliding _slidingCompo;
        private float currentTilt = 0f;
        private const float BLOODTHIEF_TILT_ANGLE = 15f;
        private const float WALL_KICK_AWAY_FORCE = 8f;
        private float WALL_Foward_AWAY_FORCE = 0;
        private const float WALL_KICK_UP_FORCE = 2f;
        private bool hasRequestedJump = false;

        public PlayerWallSlideState(Entity entity, int animationHash) : base(entity, animationHash)
        {
            _slidingCompo = entity.GetCompo<WallSliding>();
        }

        public override void Enter()
        {
            //base.Enter();
            WALL_Foward_AWAY_FORCE = _movementCompo.GetCurrentMoveSpeed();
            _player.isSliding = false;
            _player.SetJumping(true);
            hasRequestedJump = false;
            
            string wallSide = _slidingCompo.GetWallSide();
            
            currentTilt = wallSide == "Left" ? -BLOODTHIEF_TILT_ANGLE : BLOODTHIEF_TILT_ANGLE;
            
            if (_player.camCompo != null)
            {
                _player.camCompo.SetTilt(currentTilt);
                _player.camCompo.SetWallSlideMode(true);
            }
            
            _slidingCompo.StartWallSlide();

            _movementCompo.wallSlideForwardSpeed = _movementCompo.GetCurrentMoveSpeed();
        }

        public override void Update()
        {
            base.Update();
            
            if (_player.inputReader != null && _player.inputReader.IsJumpPressed() && !hasRequestedJump)
            {
                hasRequestedJump = true;
                PerformWallKick();
                return;
            }

            if (_movementCompo.GetWallSlideSpeed() <= 0)
            {
                hasRequestedJump = true;
                PerformWallKick();
                return;
            }
            
            if (_slidingCompo.CanSlidingWall() == "None")
            {
                _player.ChangeState("IDLE");
            }
        }

        private void PerformWallKick()
        {
            Vector3 wallNormal = _slidingCompo.GetWallNormal();
            
            if (_movementCompo != null)
            {
                _movementCompo.ApplyWallKick(wallNormal, _player.transform,WALL_KICK_AWAY_FORCE,WALL_Foward_AWAY_FORCE, WALL_KICK_UP_FORCE);
            }
            
            _slidingCompo.EndWallSlide();
            _player.ChangeState("IDLE");
        }

        public override void Exit()
        {
           // base.Exit();
            
            if (_player.camCompo != null)
            {
                _player.camCompo.ReturnOwnTilt();
                _player.camCompo.SetWallSlideMode(false);
            }
            
            _slidingCompo.EndWallSlide();
        }
    }
}