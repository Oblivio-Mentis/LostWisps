using Godot;
using LostWisps.Debug;
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
		[Export] public MovementController MovementController;

		public Vector2 frameVelocity = Vector2.Zero;
		public Vector2 frameInput = Vector2.Zero;
		private PlayerState currentState;
		private PlayerState previousState;
		private String currentAnimationState = "idle";

		private AnimationNodeStateMachinePlayback animationNodeStateMachinePlayback;

		public bool KeyUp { get; private set; }
		public bool KeyDown { get; private set; }
		public bool KeyLeft { get; private set; }
		public bool KeyRight { get; private set; }
		public bool KeyJump { get; private set; }
		public bool KeyJumpPressed { get; private set; }

		public override void _Ready()
		{
			if (Stats == null)
				Logger.Error(LogCategory.Player, "PlayerStats is not assigned!", this);

			animationTree.Active = true;

			var playbackVariant = animationTree.Get("parameters/playback");
			animationNodeStateMachinePlayback = playbackVariant.As<AnimationNodeStateMachinePlayback>();

			if (animationNodeStateMachinePlayback == null)
            {
				Logger.Error(LogCategory.Player, "Failed to get AnimationNodeStateMachinePlayback from AnimationTree.", this);
            }

			currentState = new IdleState(this);
			currentState.EnterState();

			if (MovementController == null)
				Logger.Error(LogCategory.Player, "MovementController is not assigned!", this);

			MovementController.Initialize(this);
		}

		public override void _PhysicsProcess(double delta)
		{
			GetInputStates();
			currentState.PhysicsUpdate(delta);

			Velocity = MovementController.Velocity;

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

			Logger.Log(LogCategory.Player, $"State Change From '{previousState.GetType().Name}'\t->\t'{currentState.GetType().Name}'", this);
		}

		public void SetAnimation(string newAnimationState)
		{
			if (animationNodeStateMachinePlayback == null || string.IsNullOrEmpty(newAnimationState))
				return;

			if (currentAnimationState != newAnimationState)
            {
				animationNodeStateMachinePlayback.Travel(newAnimationState);
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
