using Godot;
using System;

namespace Player
{
    public partial class JumpState : PlayerState
    {
        public JumpState(Player player) : base(player) {}

        public override void EnterState()
        {
            player.SetAnimation("jump");
            Jump();
        }

        public override void PhysicsUpdate(double delta)
        {
            HandleHorizontalMovement(delta);
            ApplyGravity(delta);
        }

        public override void Update(double delta)
        {
            if (player.frameVelocity.Y >= 0)
                player.ChangeState(new FallState(player));

            if (player.frameInput.X != 0 && Mathf.Sign(player.frameVelocity.X) != player.frameInput.X)
                player.ChangeState(new FallState(player));
        }

        private void HandleHorizontalMovement(double delta)
        {
            float direction = player.frameInput.X;

            if (direction != 0)
                player.skeletonContainer.Scale = new Vector2(direction, 1);

            player.frameVelocity.X = Mathf.MoveToward(player.frameVelocity.X, player.frameInput.X * player.Stats.MaxSpeed, player.Stats.Acceleration * (float)delta);
        }

        private void Jump()
        {
            player.frameVelocity.Y = -player.Stats.JumpPower;
        }

        private void ApplyGravity(double delta)
		{
            player.frameVelocity.Y += player.Stats.GravityJump * (float)delta;
		}
    }
}
