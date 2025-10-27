using Godot;
using System;

namespace LostWisps.Player
{
	public partial class JumpState : PlayerState
	{
		public JumpState(Player player) : base(player, "jump") { }

		public override void EnterState()
		{
			player.SetAnimation(animationState);
			player.JumpBuffer.Stop();
			player.CoyoteTimer.Stop();

			player.MovementController.Velocity = new Vector2(player.MovementController.Velocity.X, player.PlayerStats.JumpVelocity);
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
		}
	}
}
