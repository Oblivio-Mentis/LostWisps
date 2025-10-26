using Godot;
using System;

namespace LostWisps.Player
{
	public partial class RunState : PlayerState
	{
		public RunState(Player player) : base(player, "walk") { }

		public override void EnterState()
		{
			player.SetAnimation(animationState);
		}

		public override void PhysicsUpdate(double delta)
		{

			player.MovementController.ApplyGroundMovement(player.frameInput.X, delta);
		}

		public override void Update(double delta)
		{
			if (!player.IsOnFloor())
			{
				player.CoyoteTimer.Start(player.Stats.CoyoteTime);
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
	}
}
