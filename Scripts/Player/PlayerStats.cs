using Godot;

[GlobalClass]
public partial class PlayerStats : Resource
{
	[Export] public float MaxSpeed = 600f;
	[Export] public float Acceleration = 800f;
	[Export] public float GroundDeceleration = 1000f;
	[Export] public float AirDeceleration = 300f;

	[Export] public float GravityJump = 600f;
	[Export] public float GravityFall = 750f;

	[Export] public float JumpPower = 400f;
	[Export] public float MaxFallSpeed = 300f;
	[Export] public float FallAcceleration = 800f;

	[Export] public float CoyoteTime = 0.15f; // Время после выхода с земли, когда можно прыгнуть
	[Export] public float JumpBufferTime = 0.15f; // Время после нажатия, чтобы использовать прыжок позже
}
