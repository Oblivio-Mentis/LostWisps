#nullable enable

using Godot;
using System;

namespace LostWisps.Object
{
    public enum RotationDirectionType
    {
        Clockwise,
        CounterClockwise
    }

    public partial class RotationSynchronizer : Synchronizer<float>
    {
        [Export] public float TargetAngle = 90.0f;
        [Export] public RotationDirectionType RotationDirection = RotationDirectionType.CounterClockwise;

        private float directionMultiplier = -1.0f;

        public override void _Ready()
        {
            ResetDirection();
            base._Ready();
        }

        private void ResetDirection()
        {
            directionMultiplier = RotationDirection == RotationDirectionType.Clockwise ? 1.0f : -1.0f;
        }

        public override float GetTarget() => TargetAngle * directionMultiplier;

        public override float Lerp(float from, float to, float t) => Mathf.Lerp(from, to, t);

        protected override void ApplyCurrentValue()
        {
            foreach (var node in TargetNodes)
            {
                if (node != null)
                    node.RotationDegrees = current;
            }
        }

        public override float ValueToTarget(float normalizedValue) =>
            normalizedValue * TargetAngle * directionMultiplier;

        public override float ValueToTargetDirect(float value) =>
            value * TargetAngle * directionMultiplier;

        protected override void UpdateConstant(float delta)
        {
            current += Speed * directionMultiplier * delta;
            current = Mathf.Wrap(current, 0f, 360f);
        }

        protected override float CalculateAnimationDuration(float from, float to) =>
            Mathf.Abs(to - from) / Speed;

        protected override void ResetState()
        {
            base.ResetState();

            if (IsAlwaysActive && !IsConstant && !PingPong)
            {
                startValue = current;
                endValue = target;
                animationStartTime = 0f;
                animationDuration = CalculateAnimationDuration(startValue, endValue);
                isAnimating = true;
            }
        }
    }
}