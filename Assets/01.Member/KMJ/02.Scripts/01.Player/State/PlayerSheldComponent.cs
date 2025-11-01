using _01.Member.KMJ._02.Scripts._01.Player.State;
using Code.Entities;

    public class PlayerSheldComponent : PlayerState
    {
        public PlayerSheldComponent(Entity entity, int animationHash) : base(entity, animationHash)
        {
        }

        public override void Enter()
        {
            _barrierCompo.sheldCollider.enabled = true;
        }

        public override void Update()
        {
            if (_player.bloodSystemCompo.GetCurrentFlowerCnt() <= 0)
            {
                _player.ChangeState("IDLE");
            }
            base.Update();
        }

        public override void Exit()
        {
            _barrierCompo.EndBarrier();
            _barrierCompo.sheldCollider.enabled = false;
        }
    }