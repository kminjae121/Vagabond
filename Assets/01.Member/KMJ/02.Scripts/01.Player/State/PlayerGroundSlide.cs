using _01.Member.KMJ._02.Scripts._01.Player.State;
using Code.Core.Debugs;
using Code.Entities;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerGroundSlide : PlayerState
{
    public PlayerGroundSlide(Entity entity, int animationHash) : base(entity, animationHash)
    {
    }

    public override void Enter()
    {
        _player.isSliding = true;
        _movementCompo.SetSpeedZero();
        _movementCompo.moveModifierSpeed = 1.21f;
        base.Enter();
    }

    public override void Update()
    {
        _player._groundSlideCompo.Sliding();
        
        if (!_player.isSliding || _movementCompo.moveSpeed <= 1)
        {
            _player.ChangeState("IDLE");
        }
        
        base.Update();
    }

    public override void Exit()
    {
        _movementCompo.moveModifierSpeed = 3f;
        _player._groundSlideCompo.ReturnSliding();
        _player.isSliding = false;
        base.Exit();
    }
}