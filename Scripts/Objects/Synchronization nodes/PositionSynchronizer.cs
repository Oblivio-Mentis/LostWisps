#nullable enable

using Godot;
using System;

namespace LostWisps.Object
{
    public enum PathDirectionType
    {
        Forward,
        Backward
    }

    public partial class PositionSynchronizer : Synchronizer<float>
    {
        [Export] public float TargetProgress = 1.0f;
        [Export] public PathDirectionType Direction = PathDirectionType.Forward;

        private PathFollow2D? pathFollow2D;
        private Vector2[] offsets = Array.Empty<Vector2>();
        private Vector2 initialPosition;

        private PathDirectionType currentDirection;

        public PathDirectionType CurrentDirection
        {
            get => currentDirection;
            private set
            {
                if (currentDirection == value)
                    return;

                currentDirection = value;
            }
        }

        public override void _Ready()
        {
            pathFollow2D = GetNodeOrNull<PathFollow2D>("PathFollow2D");

            if (pathFollow2D == null)
            {
                GD.PushError("Не найден PathFollow2D");
                return;
            }

            initialPosition = pathFollow2D.GlobalPosition;

            int count = TargetNodes.Length;
            offsets = new Vector2[count];

            for (int i = 0; i < count; i++)
            {
                if (TargetNodes[i] != null)
                    offsets[i] = TargetNodes[i].GlobalPosition - pathFollow2D.GlobalPosition;
            }

            CurrentDirection = Direction;

            base._Ready();
        }

        public override float GetTarget() => TargetProgress;

        public override float Lerp(float from, float to, float t) => Mathf.Lerp(from, to, t);

        protected override void ApplyCurrentValue()
        {
            if (pathFollow2D == null)
                return;

            pathFollow2D.ProgressRatio = current;

            var currentPathPos = pathFollow2D.GlobalPosition;

            for (int i = 0; i < TargetNodes.Length; i++)
            {
                if (TargetNodes[i] != null)
                {
                    Vector2 delta = (currentPathPos - initialPosition) * (CurrentDirection == PathDirectionType.Backward ? -1f : 1f);
                    Vector2 targetPosition = offsets[i] + initialPosition + delta;
                    TargetNodes[i].GlobalPosition = targetPosition;
                }
            }
        }

        public override float ValueToTarget(float normalizedValue) =>
            normalizedValue * TargetProgress;

        public override float ValueToTargetDirect(float value)
        {
            value = Mathf.Clamp(value, -1f, 1f);

            if (!PingPong)
            {
                bool newForward = value >= 0;
                SetDirectionWithAnimation(newForward ? PathDirectionType.Forward : PathDirectionType.Backward);
            }

            return Mathf.Abs(value);
        }

        public void SetDirectionWithAnimation(PathDirectionType newDirection)
        {
            if (CurrentDirection == newDirection || PingPong)
                return;

            float previousProgress = current;
            float mirroredProgress = 1f - previousProgress;

            CurrentDirection = newDirection;

            startValue = previousProgress;
            endValue = mirroredProgress;
            animationStartTime = 0f;
            animationDuration = CalculateAnimationDuration(startValue, endValue);
            isAnimating = true;

            _activatable.Activate();
        }

        protected override void UpdateConstant(float delta)
        {
            current += Speed * (float)delta;
            current = Mathf.Wrap(current, 0f, 1f);
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