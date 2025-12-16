using System;
using Godot;

namespace LostWisps.Player
{
    public partial class SlopeClimbState : LostWisps.Player.PlayerState
    {
        public SlopeClimbState(Player player) : base(player, "slope") { }

        public override void EnterState()
        {
            player.frameVelocity.Y = 0;
        }

        public override void PhysicsUpdate(double delta)
        {
            player.MovementController.ApplySlopeClimbMovement(player.frameInput.X, delta);
        }

        public override void Update(double delta)
        {
            if (player.IsOnWallOnly() && player.frameInput == Vector2.Zero)
            {
                player.ChangeState(new SlideState(player));
                return;
            }

            if (player.IsOnWallOnly() && player.frameInput == Vector2.Zero)
            {
                if (Mathf.Sign(-player.GetSlopeUpDirection().Y) != player.frameInput.X)
                {
                    player.ChangeState(new SlideState(player));
                    return;
                }
            }
            
            if (player.KeyJumpPressed)
            {
                player.ChangeState(new JumpState(player));
                return;
            }

            if (!player.IsOnFloor() && !player.IsOnWall())
            {
                player.ChangeState(new FallState(player));
                return;
            }
        }
	}
}
