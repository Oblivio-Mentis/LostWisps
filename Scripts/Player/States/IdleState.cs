using Godot;
using System;

namespace Player
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
			HandleHorizontalMovement(delta);
		}

		public override void Update(double delta)
		{
			if (!player.IsOnFloor())
			{
				player.CoyoteTimer.Start(player.Stats.CoyoteTime);
				player.ChangeState(new FallState(player));
			}
			else if (player.KeyJump)
			{
				player.ChangeState(new JumpState(player));
			}
			else if (player.KeyLeft || player.KeyRight)
			{
				player.ChangeState(new RunState(player));
			}
		}

		private void HandleHorizontalMovement(double delta)
		{
			player.frameVelocity.X = Mathf.MoveToward(player.frameVelocity.X, 0,  Mathf.Abs(player.frameVelocity.X) / 0.05f * (float)delta);
		}
	}
}
