using Godot;
using System;

namespace Object
{
    public partial class Gate : Node2D, IActivatable
    {
        [Export] private float openY = 100f;
        [Export] private float closeY = 50f;
        [Export] private float speed = 50f;
        [Export] private AnimationPlayer animationPlayer;

        private Vector2 initialPosition;
        private Tween currentTween;

        public override void _Ready()
        {
            initialPosition = Position;
        }

        private void Open()
        {
            Vector2 target = initialPosition with { Y = initialPosition.Y - openY };
            StartMovement(target);
        }

        private void Close()
        {
            Vector2 target = initialPosition with { Y = initialPosition.Y - openY + closeY };
            StartMovement(target);
        }

        private void StartMovement(Vector2 target)
        {
            float distance = Position.DistanceTo(target);
            float duration = distance / speed;

            if (duration <= 0)
                duration = 0.01f;

            if (currentTween != null)
            {
                currentTween.Stop();
                currentTween.Kill();
            }

            currentTween = CreateTween();
            currentTween.TweenProperty(this, "position", target, duration)
                       .SetEase(Tween.EaseType.Out)
                       .SetTrans(Tween.TransitionType.Quad);
        }

        public void Activate()
        {
            Open();
        }

        public void Deactivate()
        {
            Close();
        }

        public override void _ExitTree()
        {
            if (currentTween != null)
            {
                currentTween.Kill();
                currentTween = null;
            }
        }
    }
}