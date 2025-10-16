using _01.Member.KMJ._00.Core._01.Entity._01.EntityState;
using _01.Member.KMJ._00.Core._01.Entity._02.EntityCompo;
using Code.Entities;
using UnityEngine;

namespace _01.Member.KMJ._02.Scripts._01.Player.State
{
    public class PlayerState : EntityState
    {
        protected Player _player;
        protected EntityAnimator _animatorCompo;
        protected EntityAnimatorTrigger _triggerCompo;
        protected CharacterMovement _movementCompo;
        
        public PlayerState(Entity entity, int animationHash) : base(entity, animationHash)
        {
            _animatorCompo = entity.GetCompo<EntityAnimator>();
            _triggerCompo = entity.GetCompo<EntityAnimatorTrigger>();
            _movementCompo = entity.GetCompo<CharacterMovement>();
            _player = entity as Player;
        }
    }
}