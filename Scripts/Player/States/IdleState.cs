using Godot;
using System;

namespace LostWisps.Player
{
	public partial class IdleState : PlayerState
	{
		public IdleState(Player player) : base(player) {}

		public override void EnterState()
		{
			player.SetAnimation("idle");
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
			player.MovementController.ApplyMovement(0, delta);
>>>>>>> Stashed changes
=======
			player.MovementController.ApplyMovement(0, delta);
>>>>>>> Stashed changes
=======
			player.MovementController.ApplyMovement(0, delta);
>>>>>>> Stashed changes
=======
			player.MovementController.ApplyMovement(0, delta);
>>>>>>> Stashed changes
=======
			player.MovementController.ApplyMovement(0, delta);
>>>>>>> Stashed changes
=======
			HandleHorizontalMovement(delta);
>>>>>>> parent of ca63d9b (Rework movement controller)
		}

		public override void Update(double delta)
		{
			if (!player.IsOnFloor())
			{
				player.CoyoteTimer.Start(player.Stats.CoyoteTime);
				player.ChangeState(new FallState(player));
				return;
			}

			if (player.KeyJump)
			{
				player.ChangeState(new JumpState(player));
				return;
			}

			if (player.KeyLeft || player.KeyRight)
			{
				player.ChangeState(new RunState(player));
				return;
			}

			if (player.IsOnWallOnly())
            {
                player.ChangeState(new SlideState(player));
                return;
            }
		}

		private void HandleHorizontalMovement(double delta)
		{
			float groundFriction = 600f;
			player.frameVelocity.X = Mathf.MoveToward(
				player.frameVelocity.X,
				0,
				groundFriction * (float)delta
			);
		}
	}
}
