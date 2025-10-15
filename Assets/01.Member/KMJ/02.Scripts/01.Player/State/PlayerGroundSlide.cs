using Code.Entities;

namespace _01.Member.KMJ._02.Scripts._01.Player.State
{
    public class PlayerGroundSlide : PlayerState
    {
        public PlayerGroundSlide(Entity entity, int animationHash) : base(entity, animationHash)
        {
        }

        public override void Enter()
        {
            _player.isSliding = true;
            _player.SetJumping(false);
            base.Enter();
        }

        public override void Update()
        {
            _player._groundSlideCompo.Sliding();
    
            // 속도가 떨어지거나 점프하면 종료
            if (_movementCompo.GetHorizontalSpeed() <= _movementCompo.baseSpeed * 1.2f || 
                !_movementCompo.CheckGroundDetected())
            {
                _player.ChangeState("IDLE");
            }
    
            base.Update();
        }

        public override void Exit()
        {
            _player._groundSlideCompo.ReturnSliding();
            _player.isSliding = false;
            _player.SetJumping(true);
            base.Exit();
        }
    }
}