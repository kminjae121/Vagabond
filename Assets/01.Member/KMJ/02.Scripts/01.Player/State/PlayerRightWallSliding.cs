using _01.Member.KMJ._02.Scripts._01.Player.SlidingCompo;
using UnityEngine;

namespace _01.Member.KMJ._02.Scripts._01.Player.State
{
    public class PlayerRightWallSliding : PlayerState
    {
        private WallSliding _slidingCompo;
        
        public PlayerRightWallSliding(Entity entity, int animationHash) : base(entity, animationHash)
        {
            _slidingCompo = entity.GetCompo<WallSliding>();
        }

        public override void Enter()
        {
            base.Enter();
            _player._camCompo.SetTilt(15f);  
            _movementCompo.SetSpeed(15f);
            _movementCompo.StopMoving();
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
        
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            _slidingCompo.WallSlide();
        }

        public override void Exit()
        {
            base.Exit();
            _player._camCompo.SetTilt(0f);
            _movementCompo.SetReturnOriginMoveSpeed();
            _slidingCompo.EndWallSlide();
        }
    }
}