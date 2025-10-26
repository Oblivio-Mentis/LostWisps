using Godot;
using System;

namespace LostWisps.Player
{
<<<<<<< Updated upstream
    public partial class JumpState : PlayerState
    {
        private bool isBlockedByCeiling = false;

        public JumpState(Player player) : base(player) { }
=======
	public partial class JumpState : PlayerState
	{
		public JumpState(Player player) : base(player, "jump") { }
>>>>>>> Stashed changes

        public override void EnterState()
        {
            player.SetAnimation("jump");
            player.JumpBuffer.Stop();
			player.CoyoteTimer.Stop();
            Jump();
        }

<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
        public override void PhysicsUpdate(double delta)
        {
            HandleHorizontalMovement(delta);
            ApplyGravity(delta);
        }

        public override void Update(double delta)
        {
            if (player.frameVelocity.Y >= 0)
            {
                player.ChangeState(new FallState(player));
                return;
            }

            if (player.frameInput.X != 0 && Mathf.Sign(player.frameVelocity.X) != player.frameInput.X)
            {
                player.ChangeState(new FallState(player));
                return;
            }
        }

        private void HandleHorizontalMovement(double delta)
        {
            float direction = player.frameInput.X;

            // if (direction != 0)
            //     player.skeletonContainer.Scale = new Vector2(direction, 1);

            player.frameVelocity.X = Mathf.MoveToward(
                player.frameVelocity.X,
                player.frameInput.X * player.Stats.JumpPowerX,
                player.Stats.AirAcceleration * (float)delta
            );
        }

        private void Jump()
        {
            player.frameVelocity.Y = -player.Stats.JumpPowerY;
        }

        private void ApplyGravity(double delta)
        {
			bool hitCeilingThisFrame = IsTopCollided();

			if (!isBlockedByCeiling)
				isBlockedByCeiling = hitCeilingThisFrame;

			float gravityMultiplier = hitCeilingThisFrame ? 2 : 1;
			float gravity = player.Stats.GravityJump * gravityMultiplier * (float)delta;
			player.frameVelocity.Y +=  gravity;

			if (isBlockedByCeiling && !hitCeilingThisFrame)
				player.frameVelocity.Y = 0f;
        }

        private bool IsTopCollided()
        {
            for (int i = 0; i < player.GetSlideCollisionCount(); i++)
            {
                var collision = player.GetSlideCollision(i);
                Vector2 normal = collision.GetNormal();

                if (normal.Dot(Vector2.Down) > 0.7f) 
                    return true;
            }

            return false;
        }
    }
}
=======
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
			player.MovementController.Velocity = new Vector2(player.MovementController.Velocity.X, player.Stats.JumpVelocity);
		}

		public override void PhysicsUpdate(double delta)
		{
			player.MovementController.ApplyMovement(player.frameInput.X, delta);
			player.MovementController.ApplyJumpGravity(delta);
		}

		public override void Update(double delta)
		{
			if (player.KeyJumpReleased || player.Velocity.Y >= 0)
			{
				player.ChangeState(new FallState(player));
				return;
			}

			// if (player.frameInput.X != 0 && Mathf.Sign(player.Velocity.X) != player.frameInput.X)
			// {
			// 	player.ChangeState(new FallState(player));
			// 	return;
			// }
		}
	}
}
>>>>>>> Stashed changes
