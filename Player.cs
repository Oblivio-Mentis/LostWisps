using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Export] public float MoveSpeed = 300f;         // Базовая скорость
	[Export] public float AirControlFactor = 0.8f;  // Управляемость в воздухе (0 - нет, 1 - как на земле)
	[Export] public float Acceleration = 1500f;     // Ускорение в воздухе
	[Export] public float JumpVelocity = -800f;     // Скорость прыжка
	[Export] public float Gravity = 2000f;          // Гравитация
	[Export] public float MaxFallSpeed = 1000f;     // Максимальная скорость падения

	private Vector2 _velocity = Vector2.Zero;
	private bool _wasOnFloor = false;
	private bool _isJumpCut = false;

	public override void _PhysicsProcess(double delta)
	{
		HandleMovement();
		HandleJump();

		Velocity = _velocity;
		MoveAndSlide();
	}

	private void HandleMovement()
	{
		float direction = Input.GetAxis("ui_left", "ui_right");

		if (!IsOnFloor())
		{
			// Плавное изменение горизонтальной скорости в воздухе
			float targetXVelocity = direction * MoveSpeed * AirControlFactor;
			float acceleration = Acceleration * (float)GetPhysicsProcessDeltaTime() * 0.65f;

			_velocity.X = Mathf.MoveToward(_velocity.X, targetXVelocity, acceleration);
		}
		else
		{
			_velocity.X = direction * MoveSpeed;
		}
	}

	private void HandleJump()
	{
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
		{
			_velocity.X *= 2f;
			_velocity.Y = JumpVelocity;
			_wasOnFloor = true;
			_isJumpCut = false;
		}

		// Подъём
		if (_velocity.Y < 0 && _wasOnFloor && !_isJumpCut)
		{
			_velocity.Y += Gravity * (float)GetPhysicsProcessDeltaTime();
			if (_velocity.Y >= 0)
			{
				_wasOnFloor = false;
			}
		}
		// Падение
		else if (!IsOnFloor())
		{
			_velocity.Y += Gravity * (float)GetPhysicsProcessDeltaTime();
			_velocity.Y = Mathf.Min(_velocity.Y, MaxFallSpeed);

			// Прерывание прыжка при боковом движении
			float direction = Input.GetAxis("ui_left", "ui_right");
			if (_velocity.Y < 0 && direction != 0)
			{
				_velocity.Y = 0; // Останавливаем подъём
				_isJumpCut = true;
			}
		}
		else
		{
			_velocity.Y = 0;
			_wasOnFloor = true;
			_isJumpCut = false;
		}
	}
}
