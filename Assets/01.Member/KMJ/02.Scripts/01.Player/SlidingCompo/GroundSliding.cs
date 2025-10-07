
using Code.Entities;
using Code.Interfaces;
using UnityEngine;

namespace _01.Member.KMJ._02.Scripts._01.Player.SlidingCompo
{
    public class GroundSliding : MonoBehaviour, IEntityComponent
    {
        public Player _player;

        [SerializeField] private Transform _downTrm;
        
        public void Initialize(Entity entity)
        {
            _player = entity as Player;
        }

        public void Slide()
        {
            if (_player.movementCompo.CheckGroundDetected())
            {
                _player.ChangeState("SLIDE");
            }
        }
    }
}