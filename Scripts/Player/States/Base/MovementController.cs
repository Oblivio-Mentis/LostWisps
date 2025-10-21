using Godot;
using System;

namespace LostWisps.Player
{
    public partial class MovementController : Node
    {
        private Player Player;
        private PlayerStats Stats => Player.Stats;

        public Vector2 Velocity { get; set; }

        public void Initialize(Player player)
        {
            Player = player;
        }

        public void ApplyGroundMovement(float inputX, double delta)
        {
            float targetSpeed = inputX * Stats.MaxSpeed;
            float acceleration = inputX != 0 ? Stats.Acceleration : Stats.Acceleration * 2;

            Velocity = new Vector2(Mathf.MoveToward(Velocity.X, targetSpeed, acceleration * (float)delta), Velocity.Y);
        }

        public void ApplyAirMovement(float inputX, double delta)
        {
            if (inputX != 0)
            {
                Velocity = new Vector2(Mathf.MoveToward(Velocity.X, inputX * Stats.JumpPowerX, Stats.AirAcceleration * (float)delta), Velocity.Y);
            }
            else
            {
                Velocity = new Vector2(Mathf.MoveToward(Velocity.X, 0, Stats.AirDeceleration * (float)delta), Velocity.Y);
            }
        }

        public void ApplyGravity(double delta, bool isJumping = false)
        {
            float gravity = isJumping ? Stats.GravityJump : Stats.GravityFall;
            Velocity = new Vector2(Velocity.X, Velocity.Y + gravity * (float)delta);
            Velocity = new Vector2(Velocity.X, Mathf.Min(Velocity.Y, Stats.MaxFallSpeed));
        }

        public void ApplySlideFriction(double delta)
        {
            const float slideFriction = 0.95f;
            Velocity = new Vector2(Velocity.X * Mathf.Pow(slideFriction, (float)delta * 60f), Velocity.Y);
        }

        public void ApplySlopeClimbMovement(float inputX, double delta)
        {
            Velocity = new Vector2(Mathf.MoveToward(Velocity.X, inputX * Stats.MaxSpeed, Stats.Acceleration * (float)delta), Velocity.Y);
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
