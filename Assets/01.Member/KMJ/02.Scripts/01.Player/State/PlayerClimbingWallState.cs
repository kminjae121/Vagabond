using _01.Member.KMJ._02.Scripts._01.Player.State;
using Code.Entities;

public class PlayerClimbingWallState : PlayerState
{
    private const float CLIMBING_TILT_ANGLE = 15f;
    
    public PlayerClimbingWallState(Entity entity, int animationHash) : base(entity, animationHash)
    {
    }

    public override void Enter()
    {
        if (_player.movementCompo != null)
        {
            _player.movementCompo.StopMoving();
        }
        
        _player.SetJumping(false);
        
        if (_player.camCompo != null)
        {
            _player.camCompo.SetClimbingMode(true, CLIMBING_TILT_ANGLE);
        }
        
       // base.Enter();
    }

    public override void Update()
    {
        base.Update();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        
        if (_player.climbingComponent != null)
        {
            _player.climbingComponent.Climbing();
        }
    }

    public override void Exit()
    {
        _player.SetJumping(true);
        
        if (_player.camCompo != null)
        {
            _player.camCompo.SetClimbingMode(false, 0f);
        }
        
        //base.Exit();
    }
}
