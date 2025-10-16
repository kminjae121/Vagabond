using _01.Member.KMJ._02.Scripts._01.Player.State;
using Code.Core.Debugs;
using Code.Entities;
using UnityEngine;

public class PlayerGuideAttackState : PlayerState
{
    public PlayerGuideAttackState(Entity entity, int animationHash) : base(entity, animationHash)
    {
    }

    public override void Enter()
    {
        _player.swordCompo.BalDo();
        _player.atkComponent.isDashAttacking = true;
        _player.SetJumping(false);
        base.Enter();
    }

    public override void Update()
    {
        _player.atkComponent.GuidedAttack();
        base.Update();
    }

    public override void Exit()
    {
        _player.swordCompo.StopBalDo();
        _player.atkComponent.isDashAttacking = false;
        _player.aimmingComponent.SetEnemyNull();
        _player.SetJumping(true);
        base.Exit();
    }
}