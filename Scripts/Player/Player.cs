using Godot;
using Player;
using System;

using System.Runtime.CompilerServices;

namespace Player
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

		// Входные данные
		public bool KeyUp { get; private set; }
		public bool KeyDown { get; private set; }
		public bool KeyLeft { get; private set; }
		public bool KeyRight { get; private set; }
		public bool KeyJump { get; private set; }
		public bool KeyJumpPressed { get; private set; }

		public override void _Ready()
		{
			if (Stats == null)
				GD.PrintErr("PlayerStats не назначен!");

			animationTree.Active = true;
			currentAnimationState = "idle";

			currentState = new IdleState(this);
			currentState.EnterState();
		}

		public override void _PhysicsProcess(double delta)
		{
			GetInputStates();
			currentState.PhysicsUpdate(delta);

			Velocity = frameVelocity;

			MoveAndSlide();
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
				// GD.Print($"Animation - Switching from '{currentAnimationState}' to '{newAnimationState}'");
				stateMachine.Travel(newAnimationState);
				currentAnimationState = newAnimationState;
			}
		}
	}
}
