using Godot;
using System;

namespace Player
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
			HandleHorizontalMovement(delta);
			
		}

		public override void Update(double delta)
		{
			if (!player.IsOnFloor())
			{
				player.CoyoteTimer.Start(player.Stats.CoyoteTime);
				player.ChangeState(new FallState(player));
			}
			else if (player.KeyJump || player.JumpBuffer.TimeLeft > 0)
			{
				player.ChangeState(new JumpState(player));
			}
			else if (player.frameInput == Vector2.Zero)
			{
				player.ChangeState(new IdleState(player));
			}
		}

		private void HandleHorizontalMovement(double delta)
		{
			float direction = player.frameInput.X;

			if (direction != 0)
			{
				player.frameVelocity.X = Mathf.MoveToward(player.frameVelocity.X, direction * player.Stats.MaxSpeed, player.Stats.Acceleration * (float)delta);
				// player.skeletonContainer.Scale = new Vector2(direction, 1);
			}
		}
	}
}
