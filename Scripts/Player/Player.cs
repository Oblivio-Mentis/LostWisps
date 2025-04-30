using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Export] public PlayerStats Stats;
	
	// Ввод
	public Vector2 FrameInput { get; private set; }
	private Vector2 FrameVelocity = Vector2.Zero;
	
		// Таймеры
	private float _time;
	private float _timeJumpWasPressed;
	private float _frameLeftGrounded = -999f;
	
	// События
	public event Action<bool, float> GroundedChanged;
	public event Action Jumped;
	
	// Логика прыжка
	private bool grounded;
	private bool _jumpToConsume;
	private bool _bufferedJumpUsable;
	private bool _coyoteUsable = true;
	
	public override void _PhysicsProcess(double delta)
	{
		_time += (float)delta;
		GatherInput();
		HandleDirection(delta);
		HandleGravity(delta);

		Velocity = FrameVelocity;
		MoveAndSlide(); // Единственный вызов MoveAndSlide
		CheckCollisions(); // После MoveAndSlide!
		HandleJump();
	}
	
	private void GatherInput()
	{
		float horizontal = Input.GetAxis("ui_left", "ui_right");
		
		// Для геймпада. Этот блок "снапит" (привязывает) ввод к значениям -1, 0, +1, чтобы убрать плавные промежуточные значения
		if (Stats.SnapInput)
			horizontal = Mathf.Abs(horizontal) < Stats.HorizontalDeadZoneThreshold ? 0 : Mathf.Sign(horizontal);

		FrameInput = new Vector2(horizontal, 0);

		if (Input.IsActionJustPressed("ui_accept"))
		{
			_jumpToConsume = true;
			_timeJumpWasPressed = _time;
		}
	}
	
	private void CheckCollisions()
	{
		bool groundHit = IsOnFloor();
		bool ceilingHit = IsOnCeiling();

		if (ceilingHit)
			FrameVelocity = new Vector2(FrameVelocity.X, Mathf.Min(0, FrameVelocity.Y));

		// На земле
		if (groundHit && !grounded)
		{
			grounded = true;
			_coyoteUsable = true;
			_bufferedJumpUsable = true;
			GroundedChanged?.Invoke(true, Mathf.Abs(FrameVelocity.Y));
		}
		// Упал с платформы
		else if (!groundHit && grounded)
		{
			grounded = false;
			_frameLeftGrounded = _time;
			GroundedChanged?.Invoke(false, 0);
		}
	}
	
	private bool HasBufferedJump => _bufferedJumpUsable && _time < _timeJumpWasPressed + Stats.JumpBuffer;
	private bool CanUseCoyote => _coyoteUsable && !IsOnFloor() && _time < _frameLeftGrounded + Stats.CoyoteTime;
	
	private void HandleJump()
	{

		if (_jumpToConsume && IsOnFloor() || CanUseCoyote)
			ExecuteJump();

		_jumpToConsume = false;
	}

	private void ExecuteJump()
	{
		_timeJumpWasPressed = 0;
		_bufferedJumpUsable = false;
		_coyoteUsable = false;
		FrameVelocity = new Vector2(Mathf.Sign(FrameInput.X) * Stats.MaxSpeed, -Stats.JumpPower); // минус важен
		Jumped?.Invoke();
	}

	private void HandleDirection(double delta)
	{
		if (FrameInput.X == 0)
		{
			float decel = grounded ? Mathf.Abs(FrameVelocity.X) / 0.1f : Stats.AirDeceleration;
			FrameVelocity = new Vector2(Mathf.MoveToward(FrameVelocity.X, 0,  decel * (float)delta), FrameVelocity.Y);
		}
		else
		{
			FrameVelocity = new Vector2(
				Mathf.MoveToward(FrameVelocity.X, FrameInput.X * Stats.MaxSpeed, Stats.Acceleration * (float)delta),
				FrameVelocity.Y
			);
		}
	}

	private void HandleGravity(double delta)
	{
		if (grounded && FrameVelocity.Y <= 0)
		{
			FrameVelocity = new Vector2(FrameVelocity.X, Mathf.Max(FrameVelocity.Y, -800));
		}
		else
		{
			float inAirGravity = Stats.FallAcceleration;

			FrameVelocity += new Vector2(0, inAirGravity * (float)delta);

			if (FrameVelocity.Y > Stats.MaxFallSpeed)
				FrameVelocity = new Vector2(FrameVelocity.X, Stats.MaxFallSpeed);
	}
}
}
