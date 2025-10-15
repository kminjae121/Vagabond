using _01.Member.KMJ._02.Scripts._01.Player.State;
using Code.Entities;
using UnityEngine;

public class PlayerClimbingWallState : PlayerState
{
    public PlayerClimbingWallState(Entity entity, int animationHash) : base(entity, animationHash)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();
    }

    public override void FixedUpdate()
    {
        _player.climbingComponent.Climbing();
        base.FixedUpdate();
    }

    override public void Exit()
    {
        
    }
}
