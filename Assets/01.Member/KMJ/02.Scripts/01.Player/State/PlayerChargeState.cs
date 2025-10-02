using UnityEngine;

namespace _01.Member.KMJ._02.Scripts._01.Player.State
{
    public class PlayerChargeState : PlayerState
    {
        public PlayerChargeState(Entity entity, int animationHash) : base(entity, animationHash)
        {
        }

        public override void Enter()
        {
            Debug.Log("차징공격 시작함");
            base.Enter();
        }

        public override void Update()
        {
            
        }

        public override void Exit()
        {
            
        }
    }
}