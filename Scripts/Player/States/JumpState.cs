using Godot;
using System;

namespace Player
{
	public partial class JumpState : PlayerState
	{
		private bool topCollision;
		public JumpState(Player player) : base(player) {topCollision = false;}

		public override void EnterState()
		{
			player.SetAnimation("jump");
			GD.Print(player.frameVelocity.X);
			Jump();
		}

		public override void PhysicsUpdate(double delta)
		{
			HandleHorizontalMovement(delta);
			ApplyGravity(delta);
		}

		public override void Update(double delta)
		{
			if (player.frameVelocity.Y >= 0)
				player.ChangeState(new FallState(player));
				
			//if (IsTopCollided())
				//player.ChangeState(new FallState(player));

			if (player.frameInput.X != 0 && Mathf.Sign(player.frameVelocity.X) != player.frameInput.X)
				player.ChangeState(new FallState(player));
		}

		private void HandleHorizontalMovement(double delta)
		{
			float direction = player.frameInput.X;

			if (direction != 0)
				player.skeletonContainer.Scale = new Vector2(direction, 1);

			player.frameVelocity.X = Mathf.MoveToward(player.frameVelocity.X, player.frameInput.X * player.Stats.JumpPowerX, player.Stats.AirAcceleration * (float)delta);
		}

		private void Jump()
		{
			player.frameVelocity.Y = -player.Stats.JumpPowerY;
		}

		private void ApplyGravity(double delta)
		{
			if (topCollision == false)
				topCollision = IsTopCollided();
			player.frameVelocity.Y += player.Stats.GravityJump * (float)delta;
			if (topCollision)
				player.frameVelocity.Y += player.Stats.GravityJump * (float)delta;
		}
		
		private bool IsTopCollided()
		{
			for (int i = 0; i < player.GetSlideCollisionCount(); i++)
			{
				var collision = player.GetSlideCollision(i);
				Vector2 normal = collision.GetNormal();
				GD.Print(normal.Dot(Vector2.Up));
				// Проверяем, сталкивались ли мы головой (нормаль направлена вверх)
				if (normal.Dot(Vector2.Up) == -1) // угол ~0 градусов
				{
					return true;
				}
			}
			return false;
		}
	}
}
