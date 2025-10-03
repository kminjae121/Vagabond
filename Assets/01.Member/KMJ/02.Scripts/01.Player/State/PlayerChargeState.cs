using _01.Member.KMJ._02.Scripts._01.Player.AttackCompo;
using UnityEngine;

namespace _01.Member.KMJ._02.Scripts._01.Player.State
{
    public class PlayerChargeState : PlayerState
    {
        private PlayerAttack _atkCompo;
        public PlayerChargeState(Entity entity, int animationHash) : base(entity, animationHash)
        {
            _atkCompo = entity.GetCompo<PlayerAttack>();
        }

        public override void Enter()
        {
            _atkCompo.StartChargingTimer();
            Debug.Log("차징공격 시작함");
            base.Enter();
        }

        public override void Update()
        {
            
        }

        public override void Exit()
        {
            _atkCompo.StopChargingTimer();
        }
    }
}