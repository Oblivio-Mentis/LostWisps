using Godot;
using System;

namespace LostWisps.Player
{
    public partial class SlideState : PlayerState
    {
        public SlideState(Player player) : base(player, "slide") { }

        public override void EnterState()
        {
        }

        public override void ExitState()
        {
        }

        public override void PhysicsUpdate(double delta)
        {
            float slopeDirection = -player.GetSlopeUpDirection().Y;
            player.MovementController.Velocity = new Vector2(
                player.MovementController.Velocity.X + player.Stats.Acceleration * Mathf.Sign(slopeDirection) * (float)delta,
                player.MovementController.Velocity.Y
            );

            player.MovementController.Velocity = new Vector2(
                Mathf.Clamp(player.MovementController.Velocity.X, -player.Stats.MaxSpeed, player.Stats.MaxSpeed),
                player.MovementController.Velocity.Y
            );

            player.MovementController.ApplySlideFriction(delta);
            player.MovementController.ApplyGravity(delta);
        }

        public override void Update(double delta)
        {
            if (player.IsOnFloor())
            {
                player.frameVelocity.Y = 0;
                player.ChangeState(new IdleState(player));
                return;
            }

            if (player.KeyJumpPressed)
            {
                player.ChangeState(new JumpState(player));
                return;
            }

            if (player.IsOnWallOnly() && player.frameInput != Vector2.Zero)
            {
                var slopeUpDirection = player.GetSlopeUpDirection();
                if (Mathf.Abs(slopeUpDirection.Y) >= 0.7f && Mathf.Abs(slopeUpDirection.Y) < 1f
                    && Mathf.Sign(-slopeUpDirection.Y) == player.frameInput.X
                    && slopeUpDirection.X > 0)
                {
                    player.ChangeState(new SlopeClimbState(player));
                    return;
                }
            }

            if (!player.IsOnFloor() && !player.IsOnWall())
            {
                player.ChangeState(new FallState(player));
                return;
            }
        }
	}
}
