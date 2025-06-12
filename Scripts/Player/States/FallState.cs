using Godot;
using System;

namespace LostWisps.Player
{
	public partial class FallState : PlayerState
	{
		public FallState(Player player) : base(player) {}

		public override void EnterState()
		{
			player.SetAnimation("fall");
		}
		
		public override void ExitState()
		{
			player.frameVelocity.Y = 0;
		}

		public override void PhysicsUpdate(double delta)
		{
			HandleJumpBuffer();
			HandleHorizontalMovement(delta);
			HandleGravity(delta);
		}

		public override void Update(double delta)
		{
			if (player.IsOnFloor() && player.JumpBuffer.TimeLeft > 0f)
				player.ChangeState(new JumpState(player));
			else if (player.KeyJumpPressed && player.CoyoteTimer.TimeLeft > 0f)
				player.ChangeState(new JumpState(player));
			else if (player.IsOnFloor() && (player.KeyLeft || player.KeyRight))
				player.ChangeState(new RunState(player));
			else if (player.IsOnFloor())
				player.ChangeState(new IdleState(player));
			else if (player.IsOnWallOnly())
				player.ChangeState(new SlideState(player));
		}

		private void HandleJumpBuffer()
		{
			if (player.KeyJumpPressed)
				player.JumpBuffer.Start(player.Stats.JumpBufferTime);
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
		}
	}
}
