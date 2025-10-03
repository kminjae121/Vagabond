using _01.Member.KMJ._02.Scripts._01.Player.AttackCompo;
using UnityEngine;

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
            _atkCompo.ChargingAttack();
            _player.SetJumping(false);
            base.Enter();
        }

        public override void Update()
        {
        }

        public override void Exit()
        {
            _player.SetJumping(true);
            base.Exit();
        }
    }
}