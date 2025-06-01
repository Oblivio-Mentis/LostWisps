using Godot;
using System;

namespace Player
{
	public partial class JumpState : PlayerState
	{
		private bool isBlockedByCeiling = false;

		public JumpState(Player player) : base(player) { }

		public override void EnterState()
		{
			player.SetAnimation("jump");
			player.JumpBuffer.Stop();
			player.CoyoteTimer.Stop();
			Jump();
		}

		public override void PhysicsUpdate(double delta)
		{
			HandleHorizontalMovement(delta);
			ApplyGravity(delta);
		}

		public override void Update(double delta)
		{
			// Переход в FallState при начале падения (вертикальная скорость >= 0)
			if (player.frameVelocity.Y >= 0)
				player.ChangeState(new FallState(player));

			// Если игрок резко меняет направление против движения — тоже в FallState
			if (player.frameInput.X != 0 && Mathf.Sign(player.frameVelocity.X) != player.frameInput.X)
				player.ChangeState(new FallState(player));
		}

		private void HandleHorizontalMovement(double delta)
		{
			float direction = player.frameInput.X;

			// if (direction != 0)
			//     player.skeletonContainer.Scale = new Vector2(direction, 1);

			player.frameVelocity.X = Mathf.MoveToward(
				player.frameVelocity.X,
				player.frameInput.X * player.Stats.JumpPowerX,
				player.Stats.AirAcceleration * (float)delta
			);
		}

		private void Jump()
		{
			player.frameVelocity.Y = -player.Stats.JumpPowerY;
		}

		private void ApplyGravity(double delta)
		{
			bool hitCeilingThisFrame = IsTopCollided();

			if (!isBlockedByCeiling)
				isBlockedByCeiling = hitCeilingThisFrame;

			float gravityMultiplier = hitCeilingThisFrame ? 2 : 1;
			float gravity = player.Stats.GravityJump * gravityMultiplier * (float)delta;
			player.frameVelocity.Y +=  gravity;

			if (isBlockedByCeiling && !hitCeilingThisFrame)
				player.frameVelocity.Y = 0f;
		}

		private bool IsTopCollided()
		{
			for (int i = 0; i < player.GetSlideCollisionCount(); i++)
			{
				var collision = player.GetSlideCollision(i);
				Vector2 normal = collision.GetNormal();

				if (normal.Dot(Vector2.Down) > 0.7f) 
					return true;
			}

			return false;
		}
	}
}
