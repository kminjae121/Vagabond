
using _01.Member.KMJ._02.Scripts._01.Player.AttackCompo;
using Code.Entities;
using UnityEngine;

namespace _01.Member.KMJ._02.Scripts._01.Player.State
{
    public class PlayerChargeState : PlayerState
    { 
        private PlayerAttack _atkCompo;
        public PlayerChargeState(Entity entity, int animationHash) : base(entity, animationHash)
        {
            _movementCompo = entity.GetCompo<CharacterMovement>();
            _atkCompo = entity.GetCompo<PlayerAttack>();
        }

        public override void Enter()
        {
            _player.swordCompo.NabDo();
            _atkCompo.StartChargingTimer();
            _player.SetJumping(false);
            base.Enter();
        }

        public override void Update()
        {
            
        }

        public override void Exit()
        {
            _player.swordCompo.StopNabDo();
            _atkCompo.StopChargingTimer();
        }
    }
}