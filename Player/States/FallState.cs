using Godot;
using System;

namespace LostWisps.Player
{
	public partial class FallState : PlayerState
	{
		public FallState(Player player) : base(player, "fall") { }

		public override void EnterState()
		{
			player.MovementController.ResetVerticalVelocity();
			player.SetAnimation(animationState);
		}
		
		public override void ExitState()
		{
			player.MovementController.ResetVerticalVelocity();
		}

		public override void PhysicsUpdate(double delta)
		{
			HandleJumpBuffer();

			player.MovementController.ApplyMovement(player.frameInput.X, delta);
			player.MovementController.ApplyFallGravity(delta, false);
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
			if (player.KeyJumpPressed)
				player.JumpBuffer.Start();
		}
	}
}
