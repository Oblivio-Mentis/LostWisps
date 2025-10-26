using Godot;
using System;

namespace LostWisps.Player
{
	public partial class FallState : PlayerState
	{
		public FallState(Player player) : base(player) {}

		public override void EnterState()
		{
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
			player.SetAnimation("fall");
=======
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
			player.MovementController.ResetVerticalVelocity();
			player.SetAnimation(animationState);
>>>>>>> Stashed changes
		}
		
		public override void ExitState()
		{
		}

		public override void PhysicsUpdate(double delta)
		{
			HandleJumpBuffer();
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
			HandleHorizontalMovement(delta);
			HandleGravity(delta);
=======
			player.MovementController.ApplyMovement(player.frameInput.X, delta);
			player.MovementController.ApplyFallGravity(delta, false);
>>>>>>> Stashed changes
=======
			player.MovementController.ApplyMovement(player.frameInput.X, delta);
			player.MovementController.ApplyFallGravity(delta, false);
>>>>>>> Stashed changes
=======
			player.MovementController.ApplyMovement(player.frameInput.X, delta);
			player.MovementController.ApplyFallGravity(delta, false);
>>>>>>> Stashed changes
=======
			player.MovementController.ApplyMovement(player.frameInput.X, delta);
			player.MovementController.ApplyFallGravity(delta, false);
>>>>>>> Stashed changes
		}

		public override void Update(double delta)
		{
			if (player.IsOnFloor() && player.JumpBuffer.TimeLeft > 0f)
			{
				player.ChangeState(new JumpState(player));
				return;
			}

			if (player.KeyJumpPressed && player.CoyoteTimer.TimeLeft > 0f)
			{
				player.ChangeState(new JumpState(player));
				return;
			}

			if (player.IsOnFloor() && (player.KeyLeft || player.KeyRight))
			{
				player.ChangeState(new RunState(player));
				return;
			}

			if (player.IsOnFloor())
			{
				player.ChangeState(new IdleState(player));
				return;
			}

			var slopeUpDirection = player.GetSlopeUpDirection();
			if (player.IsOnWallOnly() && slopeUpDirection.X > 0)
			{
				player.ChangeState(new SlideState(player));
				return;
			}
				
		}

		private void HandleJumpBuffer()
		{
			// if (player.KeyJumpPressed)
			// 	player.JumpBuffer.Start(player.Stats.JumpBufferTime);
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
		}

		private void HandleHorizontalMovement(double delta)
		{
			float direction = player.frameInput.X;

			// if (direction != 0)
			// 	player.skeletonContainer.Scale = new Vector2(direction, 1);

			player.frameVelocity.X = Mathf.MoveToward(player.frameVelocity.X, player.frameInput.X * player.Stats.JumpPowerX, player.Stats.AirDeceleration * (float)delta);
		}

		private void HandleGravity(double delta)
		{
			player.frameVelocity += new Vector2(0, player.Stats.GravityFall * (float)delta);
			player.frameVelocity.Y = Mathf.Min(player.frameVelocity.Y, player.Stats.MaxFallSpeed);
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
		}
	}
}
