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

    [Tool]
    public partial class PositionSynchronizer : ValueSynchronizer<float>
    {
        [Export] public float TargetProgress = 1.0f;

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
            SetInitialOffsets();

            base._Ready();
        }

        public override float GetTarget() => TargetProgress;
        public override float GetNextTarget(float from) => TargetProgress;
        public override float Lerp(float from, float to, float t) => Mathf.Lerp(from, to, t);

        protected override void SetInitialOffsets()
        {
            int count = TargetNodes.Length;
            offsets = new Vector2[count];

            // Используем current (protected из ValueSynchronizer<T>)
            if (pathFollow2D != null)
            {
                pathFollow2D.ProgressRatio = current;
                for (int i = 0; i < count; i++)
                {
                    if (TargetNodes[i] != null)
                        offsets[i] = TargetNodes[i].GlobalPosition - pathFollow2D.GlobalPosition;
                }
            }
        }

        public override float ValueToTarget(float normalizedValue) =>
            normalizedValue * TargetProgress;

        public override float ValueToTargetDirect(float value)
        {
            value = Mathf.Clamp(value, -1f, 1f);
            float progress = Mathf.Remap(value, -1f, 1f, 0f, 1f);

            if (!IsPingPong)
            {
                bool newForward = value >= 0;
                SetDirectionWithAnimation(newForward ? PathDirectionType.Forward : PathDirectionType.Backward);
            }

            return progress;
        }

        public void SetDirectionWithAnimation(PathDirectionType newDirection)
        {
            if (CurrentDirection == newDirection || IsPingPong)
                return;

            float previousProgress = current;
            float mirroredProgress = 1f - previousProgress;

            CurrentDirection = newDirection;

            startValue = previousProgress;
            endValue = mirroredProgress;
            animationStartTime = 0f;
            animationDuration = CalculateAnimationDuration(startValue, endValue);
            isAnimating = true;

            Activate();
            if (!IsActivated)
                Activate();
        }
        
        protected override void ApplyCurrentValue()
        {
            if (pathFollow2D == null)
                return;

            if (IsConstant)
            {
                // Особый режим: current = время * скорость (циклично)
                // Но current у нас обновляется только в анимациях.
                // → Значит, нужно обновлять его здесь!
                // Однако _PhysicsProcess не вызывает ApplyCurrentValue до Update.
                // Поэтому лучше — перехватить в _PhysicsProcess, но мы не можем.

                // Альтернатива: вынеси логику в отдельный метод и вызывай из _Process/_PhysicsProcess,
                // но это нарушит архитектуру.

                // 👉 Лучшее решение: убери поддержку "движущегося Constant" из этого синхронизатора,
                // или создай отдельный компонент.

                // Но если очень нужно — сделаем так:
                // (не идеально, но работает)
                if (IsActivated)
                {
                    current += AnimationSpeed * (float)GetProcessDeltaTime();
                    current = Mathf.Wrap(current, 0f, 1f);
                }
            }

            pathFollow2D.ProgressRatio = current;
            var currentPathPos = pathFollow2D.GlobalPosition;

            for (int i = 0; i < TargetNodes.Length; i++)
            {
                if (TargetNodes[i] != null)
                {
                    Vector2 targetPosition = currentPathPos + offsets[i];
                    TargetNodes[i].GlobalPosition = targetPosition;
                }
            }
        }

        protected override float CalculateAnimationDuration(float from, float to) =>
            Mathf.Abs(to - from) / AnimationSpeed;

        protected override void ResetInitialState()
        {
            base.ResetInitialState();

            if (IsAlwaysActive && !IsConstant && !IsPingPong)
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