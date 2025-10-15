
using _01.Member.KMJ._02.Scripts._01.Player.SlidingCompo;
using Code.Entities;
using UnityEngine;

namespace _01.Member.KMJ._02.Scripts._01.Player.State
{
    public class PlayerRightWallSliding : PlayerState
    {
        private WallSliding _slidingCompo;
        
        private Rigidbody _rbComponentl;
        
        public PlayerRightWallSliding(Entity entity, int animationHash) : base(entity, animationHash)
        {
            _slidingCompo = entity.GetCompo<WallSliding>();
        }

        public override void Enter()
        {
            base.Enter();
            _rbComponentl = _player.GetComponent<Rigidbody>();
            _player.isSliding = false;
            //_movementCompo.SetSpeed(11);
            _player.camCompo.SetTilt(15f);  
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
            _rbComponentl.AddForce(-_player.transform.right * 1.7f, ForceMode.Impulse);
            _player.camCompo.ReturnOwnTilt();
            _movementCompo.SetReturnOriginMoveSpeed();
            _slidingCompo.EndWallSlide();
        }
    }
}