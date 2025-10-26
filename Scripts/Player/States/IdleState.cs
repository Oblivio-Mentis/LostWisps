using Godot;
using System;

namespace LostWisps.Player
{
	public partial class IdleState : PlayerState
	{
		public IdleState(Player player) : base(player, "idle") {}

		public override void EnterState()
		{
			player.SetAnimation(animationState);
		}

		public override void PhysicsUpdate(double delta)
		{

			player.MovementController.ApplyMovement(0, delta);
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
	}
}
