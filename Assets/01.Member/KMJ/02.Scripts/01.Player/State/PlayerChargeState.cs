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
            if (_player.movementCompo != null)
            {
                _player.movementCompo.SetReturnOriginMoveSpeed();
            }
            
            if (_player.swordCompo != null)
            {
                _player.swordCompo.NabDo();
            }
            
            if (_atkCompo != null)
            {
                _atkCompo.StartChargingTimer();
            }
            
            _player.SetJumping(false);
            
            if (_movementCompo != null)
            {
                _movementCompo.StopMoving();
            }
            
           // base.Enter();
        }

        public override void Update()
        {
            if (_player.aimmingComponent != null)
            {
                _player.aimmingComponent.ShootRayForCheckEnemy();
            }
            
            base.Update();
        }

        public override void Exit()
        {
            if (_player.swordCompo != null)
            {
                _player.swordCompo.StopNabDo();
            }
            
            if (_atkCompo != null)
            {
                _atkCompo.StopChargingTimer();
            }
            
            _player.SetJumping(true);
            
            //base.Exit();
        }
    }
}