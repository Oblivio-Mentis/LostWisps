#nullable enable

using Godot;

namespace LostWisps.Object
{
    public enum RotationDirectionType
    {
        Clockwise,
        CounterClockwise
    }

    [Tool]
    public partial class RotationSynchronizer : ValueSynchronizer<float>
    {
        [Export] public float TargetAngle = 90.0f;
        [Export] public RotationDirectionType RotationDirection = RotationDirectionType.CounterClockwise;

        private float lastAngle = 0f;
        private float directionMultiplier = -1.0f;

        public override void _Ready()
        {
            ResetDirection();
            base._Ready(); // вызовет ResetInitialState и SwitchMode
        }

        private void ResetDirection()
        {
            directionMultiplier = RotationDirection == RotationDirectionType.Clockwise ? 1.0f : -1.0f;
        }

        public override float GetTarget() =>
            TargetAngle * directionMultiplier;

        public override float GetNextTarget(float from) =>
            from + TargetAngle * directionMultiplier;

        public override float Lerp(float from, float to, float t) =>
            Mathf.Lerp(from, to, t);

        protected override void SetInitialOffsets()
        {
            // Для вращения смещения не нужны
        }

        protected override void ApplyCurrentValue()
        {
            if (IsConstant)
            {
                // Обновляем current напрямую, как в PositionSynchronizer
                if (IsActivated)
                {
                    current += AnimationSpeed * directionMultiplier * (float)GetPhysicsProcessDeltaTime();
                    current = Mathf.Wrap(current, 0f, 360f);
                }
            }

            // Применяем дельту вращения
            float delta = current - lastAngle;
            foreach (var node in TargetNodes)
            {
                if (node != null)
                    node.RotationDegrees += delta;
            }

            lastAngle = current;
        }

        public override float ValueToTarget(float normalizedValue) =>
            normalizedValue * TargetAngle * directionMultiplier;

        public override float ValueToTargetDirect(float value)
        {
            value = Mathf.Clamp(value, -1f, 1f);
            float progress = Mathf.Remap(value, -1f, 1f, 0f, 1f);
            return progress * TargetAngle * directionMultiplier;
        }

        protected override float CalculateAnimationDuration(float from, float to) =>
            Mathf.Abs(to - from) / AnimationSpeed;

        protected override void ResetInitialState()
        {
            base.ResetInitialState();

            // Сброс направления — уже сделан в _Ready, но если параметры меняются в редакторе — можно добавить:
            // ResetDirection(); ← не обязательно, если RotationDirection не меняется после старта

            if (IsAlwaysActive && !IsConstant && !IsPingPong)
            {
                // Запускаем анимацию сразу, если нужно
                StartAnimation(current, target);
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