using Godot;
using System;

namespace LostWisps.Player
{
	public partial class RunState : PlayerState
	{

		public RunState(Player player) : base(player) { }

		public override void EnterState()
		{
			player.SetAnimation("walk");
		}

		public override void PhysicsUpdate(double delta)
		{
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
			HandleHorizontalMovement(delta);
=======
			player.MovementController.ApplyMovement(player.frameInput.X, delta);
>>>>>>> Stashed changes
=======
			player.MovementController.ApplyMovement(player.frameInput.X, delta);
>>>>>>> Stashed changes
=======
			player.MovementController.ApplyMovement(player.frameInput.X, delta);
>>>>>>> Stashed changes
=======
			player.MovementController.ApplyMovement(player.frameInput.X, delta);
>>>>>>> Stashed changes
=======
			player.MovementController.ApplyMovement(player.frameInput.X, delta);
>>>>>>> Stashed changes
		}

		public override void Update(double delta)
		{
			if (!player.IsOnFloor())
			{
				player.CoyoteTimer.Start();
				player.ChangeState(new FallState(player));
				return;
			}

			if (player.KeyJump || player.JumpBuffer.TimeLeft > 0)
			{
				player.ChangeState(new JumpState(player));
				return;
			}

			if (player.frameInput == Vector2.Zero)
			{
				player.ChangeState(new IdleState(player));
				return;
			}
		}

		private void HandleHorizontalMovement(double delta)
		{
			float direction = player.frameInput.X;

			if (direction != 0)
			{
				player.frameVelocity.X = Mathf.MoveToward(player.frameVelocity.X, direction * player.Stats.MaxSpeed, player.Stats.Acceleration * (float)delta);
			}
		}
	}
}
