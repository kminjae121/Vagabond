using _01.Member.KMJ._02.Scripts._01.Player.AttackCompo;
using Code.Entities;

namespace _01.Member.KMJ._02.Scripts._01.Player.State
{
    public class PlayerChargeAttackState : PlayerState
    {
        private PlayerAttack _atkCompo;
        
        public PlayerChargeAttackState(Entity entity, int animationHash) : base(entity, animationHash)
        {
            _atkCompo = entity.GetCompo<PlayerAttack>();
        }
        
        public override void Enter()
        {

            _player.maskController.BalDoScrean();
            if (_player.atkComponent != null)
            {
                _player.atkComponent._timer = 0;
            }
            
            if (_player.swordCompo != null)
            {
                _player.swordCompo.BalDo();
            }
            
            _player.SetJumping(false);
            
            if (_atkCompo != null)
            {
                _atkCompo.Dash();
            }
            
           // base.Enter();
        }

        public override void Update()
        {
            base.Update();
        }

        public override void Exit()
        {
            if (_player.swordCompo != null)
            {
                _player.swordCompo.StopBalDo();
            }
            
            _player.SetJumping(true);
            
            if (_atkCompo != null)
            {
                _atkCompo.isDashAttacking = false;
            }

            if (_movementCompo != null)
            {
                _movementCompo.SetOriginGravity();
            }
            
           // base.Exit();
        }
    }
}