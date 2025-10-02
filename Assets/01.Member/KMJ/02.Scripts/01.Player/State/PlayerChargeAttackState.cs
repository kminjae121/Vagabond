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
            base.Enter();
            _atkCompo.ChargeAttackSec();
        }

        public override void Update()
        {
            base.Update();
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}