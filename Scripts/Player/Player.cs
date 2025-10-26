using Godot;
using System;

using System.Runtime.CompilerServices;

namespace LostWisps.Player
{
	public partial class Player : CharacterBody2D
	{
		[Export] public PlayerStats Stats;
		[Export] public Timer JumpBuffer;
		[Export] public Timer CoyoteTimer;
		[Export] public AnimationTree animationTree;
		[Export] public Node2D skeletonContainer;

		public Vector2 frameVelocity = Vector2.Zero;
		public Vector2 frameInput = Vector2.Zero;
		private PlayerState currentState;
		private PlayerState previousState;
		private String currentAnimationState;

		public bool KeyUp { get; private set; }
		public bool KeyDown { get; private set; }
		public bool KeyLeft { get; private set; }
		public bool KeyRight { get; private set; }
		public bool KeyJump { get; private set; }
		public bool KeyJumpPressed { get; private set; }

		public override void _Ready()
		{
			if (Stats == null)
<<<<<<< HEAD
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
=======
>>>>>>> parent of ca63d9b (Rework movement controller)
				GD.PrintErr("PlayerStats не назначен!");

			animationTree.Active = true;
			currentAnimationState = "idle";

			currentState = new IdleState(this);
			currentState.EnterState();
<<<<<<< HEAD
=======
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
			{
				Logger.Error(LogCategory.Player, "Player.Stats is not assigned! Assign it in the inspector.", this);
				return;
			}

			if (MovementController == null)
			{
				Logger.Error(LogCategory.Player, "Player.MovementController is not assigned! Assign it in the inspector.", this);
				return;
			}

			if (animationTree == null)
			{
				Logger.Error(LogCategory.Player, "Player.animationTree is not assigned! Make sure it's exported and connected in the inspector.", this);
				return;
			}

			animationTree.Active = true;

			var playbackVariant = animationTree.Get("parameters/playback");
			if (playbackVariant.VariantType == Variant.Type.Nil)
			{
				Logger.Error(LogCategory.Player, "AnimationTree does not contain a 'playback' parameter. Check your AnimationTree setup.", this);
				return;
			}

			animationNodeStateMachinePlayback = playbackVariant.As<AnimationNodeStateMachinePlayback>();
			if (animationNodeStateMachinePlayback == null)
			{
				Logger.Error(LogCategory.Player, "Failed to cast playback to AnimationNodeStateMachinePlayback. Ensure the root of your AnimationTree is a StateMachine.", this);
				return;
			}

			if (Collider == null)
			{
				Logger.Error(LogCategory.Player, "Player.Collider is not assigned! Make sure it's exported and connected in the inspector.", this);
				return;
			}

			currentState = new IdleState(this);
			currentState.EnterState();

			MovementController.Initialize(this);
>>>>>>> Stashed changes
=======
>>>>>>> parent of ca63d9b (Rework movement controller)
		}

		public override void _PhysicsProcess(double delta)
		{
			GetInputStates();
			currentState.PhysicsUpdate(delta);

			Velocity = frameVelocity;

			// for (int i = 0; i < GetSlideCollisionCount(); i++)
			// {
			// 	KinematicCollision2D collision = GetSlideCollision(i);

			// 	if (collision.GetCollider() is RigidBody2D rigidBody)
			// 	{
			// 		// 1. Вычисляем направление толчка
			// 		Vector2 pushDir = -collision.GetNormal();

			// 		// 2. Вычисляем разницу скоростей в направлении толчка
			// 		float velocityDiffInPushDir = Velocity.Dot(pushDir) - rigidBody.LinearVelocity.Dot(pushDir);
			// 		velocityDiffInPushDir = Mathf.Max(0.0f, velocityDiffInPushDir); // Только положительная разница

			// 		// 3. Учитываем массу объекта
			// 		float massRatio = Mathf.Min(1.0f, 100 / rigidBody.Mass);

			// 		// 4. Вычисляем силу толчка
			// 		Vector2 pushForce = pushDir * velocityDiffInPushDir * (massRatio * (IsOnFloor() ? 1 : 1.0f));

			// 		// 5. Применяем импульс к объекту
			// 		rigidBody.ApplyCentralImpulse(pushForce);
			// 	}
			// }

			MoveAndSlide();

			HandleForces();
		}

		private void HandleForces()
		{
			const float pushAngleThreshold = 0.2f;
			const float maxPushForce = 500f;

			for (int i = 0; i < GetSlideCollisionCount(); i++)
			{
				var collision = GetSlideCollision(i);
				if (collision.GetCollider() is RigidBody2D body)
				{
					Vector2 pushDirection = -collision.GetNormal();
					Vector2 playerDirection = Velocity.Length() > 0.1f ? Velocity.Normalized() : Vector2.Zero;

					float dot = pushDirection.Dot(playerDirection);

					if (dot > pushAngleThreshold)
					{
						float rawPushForce = (Stats.PushForce * Velocity.Length() / Stats.MaxSpeed) + Stats.MinPushForce;
						float finalPushForce = Mathf.Min(maxPushForce, rawPushForce);

						body.ApplyCentralForce(pushDirection * finalPushForce);
					}
				}
			}
		}

		public override void _Process(double delta)
		{
			currentState.Update(delta);
		}

		public override void _Input(InputEvent @event)
		{
			currentState.Input(@event);
		}

		public override void _Draw()
		{
			currentState.Draw();
		}

		private void GetInputStates()
		{
			KeyUp = Input.IsActionPressed("ui_up");
			KeyDown = Input.IsActionPressed("ui_down");
			KeyLeft = Input.IsActionPressed("ui_left");
			KeyRight = Input.IsActionPressed("ui_right");
			KeyJumpPressed = Input.IsActionJustPressed("ui_accept");
			KeyJump = Input.IsActionPressed("ui_accept");

			frameInput.X = Input.GetAxis("ui_left", "ui_right");
		}

		public void ChangeState(PlayerState newState)
		{
			if (newState == null || currentState == newState)
				return;

			previousState = currentState;
			currentState.ExitState();

			currentState = newState;
			currentState.EnterState();

			// GD.Print($"State machine - State Change From '{previousState.GetType().FullName}' to '{currentState.GetType().FullName}'");
		}

		public void SetAnimation(string newAnimationState)
		{
			if (animationTree == null)
				return;

			AnimationNodeStateMachinePlayback stateMachine = (AnimationNodeStateMachinePlayback)animationTree.Get("parameters/playback");
			if (stateMachine == null)
				return;

			if (currentAnimationState != newAnimationState)
			{
				stateMachine.Travel(newAnimationState);
				currentAnimationState = newAnimationState;
			}
		}

		public Vector2 GetSlopeUpDirection()
		{
			if (!IsOnWall())
				return Vector2.Zero;

			Vector2 normal = GetWallNormal();

            return new Vector2(-normal.Y, normal.X).Normalized();
		}
	}
}
