using Godot;
using System;

namespace Object
{
    public partial class MovingPlatform : Path2D
    {
        [Export] private bool loop = true;
        [Export] private float speed = 2.0f;
        [Export] private float animationSpeedScale = 1.0f;
        [Export] private PathFollow2D pathFollow2D;
        [Export] private AnimationPlayer animationPlayer;

        public override void _Ready()
        {
            if (!loop)
            {
                animationPlayer.Play("move");
                animationPlayer.SpeedScale = animationSpeedScale;
                SetProcess(false);
            }
		}

        public override void _PhysicsProcess(double delta)
        {
            pathFollow2D.Progress += speed * (float)delta;
        }
    }
}