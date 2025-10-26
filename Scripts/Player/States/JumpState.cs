using Godot;
using System;

namespace LostWisps.Player
{
	public partial class JumpState : PlayerState
	{
		private bool isBlockedByCeiling = false;

		public JumpState(Player player) : base(player, "jump") { }

		public override void EnterState()
		{
			player.SetAnimation(animationState);
			player.JumpBuffer.Stop();
			player.CoyoteTimer.Stop();

			player.MovementController.Velocity = new Vector2(player.MovementController.Velocity.X, -player.Stats.JumpPowerY);
		}

		public override void PhysicsUpdate(double delta)
		{
			player.MovementController.ApplyAirMovement(player.frameInput.X, delta);
			bool isJumping = player.frameVelocity.Y < 0;
			player.MovementController.ApplyGravity(delta, isJumping: isJumping);
		}

		public override void Update(double delta)
		{
			if (player.KeyJumpReleased || player.Velocity.Y >= 0)
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

		public override void PhysicsUpdate(double delta)
		{
			player.MovementController.ApplyMovement(player.frameInput.X, delta);
			player.MovementController.ApplyJumpGravity(delta);
		}

	}
}