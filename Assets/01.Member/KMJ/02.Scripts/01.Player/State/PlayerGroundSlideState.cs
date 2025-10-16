using Code.Entities;

namespace _01.Member.KMJ._02.Scripts._01.Player.State
{
    public class PlayerGroundSlideState : PlayerState
    {
        public PlayerGroundSlideState(Entity entity, int animationHash) : base(entity, animationHash)
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
            base.Update();
            
            if (!_movementCompo.isGroundSliding || !_movementCompo.CheckGroundDetected())
            {
                _player.ChangeState("IDLE");
            }
        }

        public override void Exit()
        {
            _player.isSliding = false;
            _player.SetJumping(true);
            base.Exit();
        }
    }
}