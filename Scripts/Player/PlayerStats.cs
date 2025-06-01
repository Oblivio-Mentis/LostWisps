using Godot;

[GlobalClass]
public partial class PlayerStats : Resource
{
	[Export] public float MaxSpeed = 220f;
	[Export] public float Acceleration = 1000f; 
	[Export] public float AirDeceleration = 1500f;

	[Export] public float GravityJump = 1100f;
	[Export] public float GravityFall = 1200f;

	[Export] public float JumpPowerX = 300f;
	[Export] public float JumpPowerY = 600f;
	[Export] public float MaxFallSpeed = 800f;
	[Export] public float AirAcceleration = 1500f;

	[Export] public float CoyoteTime = 0.1f; // Время после выхода с земли, когда можно прыгнуть
	[Export] public float JumpBufferTime = 0.1f; // Время после нажатия, чтобы использовать прыжок позже
}

// var normal = player.GetFloorNormal();
// if (normal.Y < player.Stats.SlopeClimbThreshold)
// {
// 	if (player.KeyLeft || player.KeyRight)
// 	{
// 		player.ChangeState(new ClimbState(player)); // Переход в ClimbState
// 		return;
// 	}

// 	player.ChangeState(new SlidingState(player)); // Иначе — SlideState
// 	return;
// }
