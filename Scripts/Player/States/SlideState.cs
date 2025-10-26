using Godot;
using System;

namespace LostWisps.Player
{
    public partial class SlideState : PlayerState
    {
        public SlideState(Player player) : base(player) { }

        public override void EnterState()
        {
        }

        public override void ExitState()
        {
        }

        public override void PhysicsUpdate(double delta)
        {
<<<<<<< Updated upstream
            HandleHorizontalMovement(delta);
            ApplySlideFriction(delta);
            HandleGravity(delta);
<<<<<<< HEAD
=======
            float slopeDirection = -player.GetSlopeUpDirection().Y;
            // player.MovementController.Velocity = new Vector2(
            //     player.MovementController.Velocity.X + player.Stats.Acceleration * Mathf.Sign(slopeDirection) * (float)delta,
            //     player.MovementController.Velocity.Y
            // );

            // player.MovementController.Velocity = new Vector2(
            //     Mathf.Clamp(player.MovementController.Velocity.X, -player.Stats.MaxSpeed, player.Stats.MaxSpeed),
            //     player.MovementController.Velocity.Y
            // );

            player.MovementController.ApplySlideFriction(delta);
            //player.MovementController.ApplyGravity(delta);
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
=======
>>>>>>> parent of ca63d9b (Rework movement controller)
=======
            player.MovementController.ApplySlideFriction(delta);
            player.MovementController.ApplyFallGravity(delta);
>>>>>>> Stashed changes
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
                GD.Print(slopeUpDirection);
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

        private void HandleHorizontalMovement(double delta)
        {
            player.frameVelocity.X += player.Stats.Acceleration * Mathf.Sign(-player.GetSlopeUpDirection().Y) * (float)delta;
            player.frameVelocity.X = Mathf.Min(player.frameVelocity.X, player.Stats.MaxSpeed);
        }

        private void ApplySlideFriction(double delta)
        {
            float slideFriction = 0.95f;
            player.frameVelocity = new Vector2(
                player.frameVelocity.X * slideFriction * (float)delta,
                player.frameVelocity.Y
            );
        }

        private void HandleGravity(double delta)
        {
            player.frameVelocity += new Vector2(0, player.Stats.GravityFall * (float)delta);
            player.frameVelocity.Y = Mathf.Min(player.frameVelocity.Y, player.Stats.MaxFallSpeed);
        }
	}
}
