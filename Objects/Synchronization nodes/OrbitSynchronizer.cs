#nullable enable

using Godot;
using System;

namespace LostWisps.Object
{
    public partial class OrbitSynchronizer : Synchronizer<float>
    {
        [Export] public float TargetAngle = 360f;
        [Export] public RotationDirectionType RotationDirection = RotationDirectionType.CounterClockwise;
        [Export] public bool AlwaysFaceCenter = false;
        [Export] public float FacingOffset = -Mathf.Pi / 2f;

        private float DirectionMultiplier => RotationDirection == RotationDirectionType.Clockwise ? 1.0f : -1.0f;

        private Vector2[] _initialOffsets = Array.Empty<Vector2>();

        public override void _Ready()
        {
            int count = TargetNodes.Length;
            _initialOffsets = new Vector2[count];

            var center = GlobalPosition;
            for (int i = 0; i < count; i++)
            {
                var node = TargetNodes[i];
                _initialOffsets[i] = node == null ? Vector2.Zero : (node.GlobalPosition - center);
            }

            base._Ready();
        }

        public override float GetTarget() => TargetAngle;

        public override float GetNextTarget(float from) => from + TargetAngle;

        public override float Lerp(float from, float to, float t) => Mathf.Lerp(from, to, t);

        protected override void ApplyCurrentValue()
        {
            float angleRad = Mathf.DegToRad(current) * DirectionMultiplier;
            Vector2 center = GlobalPosition;

            for (int i = 0; i < TargetNodes.Length; i++)
            {
                var node = TargetNodes[i];
                if (node is not Node2D node2D) continue;

                Vector2 newPos = center + _initialOffsets[i].Rotated(angleRad);

                if (AlwaysFaceCenter)
                {
                    Vector2 toCenter = center - newPos;
                    float rot = toCenter.Angle() + FacingOffset;
                    node2D.GlobalTransform = new Transform2D(rot, newPos);
                }
                else
                {
                    node2D.GlobalPosition = newPos;
                }
            }
        }

        public override float ValueToTarget(float normalizedValue) => normalizedValue * TargetAngle;

        public override float ValueToTargetDirect(float value)
        {
            value = Mathf.Clamp(value, -1f, 1f);
            return value * TargetAngle;
        }

        protected override void UpdateConstant(float delta)
        {
            if (TargetAngle <= 0f)
            {
                current = 0f;
                return;
            }

            current += Speed * delta;
            current = Mathf.Wrap(current, 0f, TargetAngle);
        }

        protected override float CalculateAnimationDuration(float from, float to)
        {
            float diff = Mathf.Abs(to - from);
            return Speed > 0f ? diff / Speed : 0.01f;
        }

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

        protected override void ActivateAdditive()
        {
            startValue = current;
            endValue = GetNextTarget(current);
            animationStartTime = 0f;
            animationDuration = CalculateAnimationDuration(startValue, endValue);
            isAnimating = true;
        }
    }
}
