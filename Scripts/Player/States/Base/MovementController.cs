using Godot;
using System;

namespace LostWisps.Player
{
    public partial class MovementController : Node
    {
        private Player Player;
        private PlayerStats PlayerStats => Player.PlayerStats;

        public Vector2 Velocity { get; set; }

        public void Initialize(Player player)
        {
            Player = player;
        }

        public void ApplyMovement(float inputX, double delta)
        {
            Velocity = new Vector2(inputX * PlayerStats.WalkVelocity, Velocity.Y);
        }

        public void ApplyFallGravity(double delta, bool isJumping = false)
        {
            Velocity = new Vector2(Velocity.X, Mathf.MoveToward(Velocity.Y, PlayerStats.FallVelocity, PlayerStats.FallGravity * (float)delta));
        }

        public void ApplyJumpGravity(double delta, bool isJumping = false)
        {
            Velocity = new Vector2(Velocity.X, Mathf.MoveToward(Velocity.Y, 0, PlayerStats.JumpDeceleration * (float)delta));
        }

        public void ApplySlideFriction(double delta)
        {
            const float slideFriction = 0.95f;
            Velocity = new Vector2(Velocity.X * Mathf.Pow(slideFriction, (float)delta * 60f), Velocity.Y);
        }

        public void ApplySlopeClimbMovement(float inputX, double delta)
        {
            // Velocity = new Vector2(Mathf.MoveToward(Velocity.X, inputX * Stats.MaxSpeed, Stats.Acceleration * (float)delta), Velocity.Y);
        }

        public void ResetVerticalVelocity()
        {
            Velocity = new Vector2(Velocity.X, 0f);
        }

        public void ResetHorizontalVelocity()
        {
            Velocity = new Vector2(0f, Velocity.Y);
        }
    }
}
