using Godot;
using System;

namespace Object
{
    public partial class ConstantlyMovingPlatform : Path2D
    {
        [Export] public bool loop = true;
        [Export] public float speed = 1.0f;
        private PathFollow2D pathFollow2D;
        private AnimationPlayer animationPlayer;

        public override void _Ready()
        {
            pathFollow2D = GetNode<PathFollow2D>("PathFollow2D");
            animationPlayer = GetNode<AnimationPlayer>("PathFollow2D/AnimationPlayer");

            var animationName = loop ? "move_loop" : "move_once";
            animationPlayer.Play(animationName);
            animationPlayer.SpeedScale = speed;
		}
    }
}