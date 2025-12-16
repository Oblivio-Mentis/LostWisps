#nullable enable

using Godot;
using System;

namespace LostWisps.Object
{
    [Tool]
    public partial class OrbitSynchronizer : ValueSynchronizer<float>
    {
        [Export] public float TargetAngle = 360f;
        [Export] public RotationDirectionType RotationDirection = RotationDirectionType.CounterClockwise;
        [Export] public bool AlwaysFaceCenter = false;
        [Export] public float FacingOffset = -Mathf.Pi / 2f;

        private float DirectionMultiplier => 
            RotationDirection == RotationDirectionType.Clockwise ? 1.0f : -1.0f;

        private Vector2[] _initialOffsets = Array.Empty<Vector2>();

        public override void _Ready()
        {
            // Инициализация смещений ДО вызова base._Ready(),
            // потому что ResetInitialState() может использовать current
            InitializeOffsets();
            base._Ready(); // вызовет ResetInitialState() и SwitchMode()
        }

        private void InitializeOffsets()
        {
            int count = TargetNodes.Length;
            _initialOffsets = new Vector2[count];
            Vector2 center = GlobalPosition;

            for (int i = 0; i < count; i++)
            {
                var node = TargetNodes[i];
                _initialOffsets[i] = node?.GlobalPosition - center ?? Vector2.Zero;
            }
        }

        public override float GetTarget() => TargetAngle;

        public override float GetNextTarget(float from) => from + TargetAngle;

        public override float Lerp(float from, float to, float t) => Mathf.Lerp(from, to, t);

        protected override void SetInitialOffsets()
        {
            // Вызывается из базового класса при активации в Additive-режиме
            InitializeOffsets();
        }

        protected override void ApplyCurrentValue()
        {
            if (IsConstant)
            {
                if (IsActivated && TargetAngle > 0f)
                {
                    // Используем физический дельта-тайм, так как обновление в _PhysicsProcess
                    float delta = (float)GetPhysicsProcessDeltaTime();
                    current += AnimationSpeed * delta;
                    current = Mathf.Wrap(current, 0f, TargetAngle);
                }
            }

            float angleRad = Mathf.DegToRad(current) * DirectionMultiplier;
            Vector2 center = GlobalPosition;

            for (int i = 0; i < TargetNodes.Length; i++)
            {
                var node = TargetNodes[i];
                if (node is not Node2D node2D) continue;

                Vector2 offset = _initialOffsets[i];
                Vector2 newPos = center + offset.Rotated(angleRad);

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
            float progress = Mathf.Remap(value, -1f, 1f, 0f, 1f);
            return progress * TargetAngle;
        }

        protected override float CalculateAnimationDuration(float from, float to)
        {
            float diff = Mathf.Abs(to - from);
            return AnimationSpeed > 0f ? diff / AnimationSpeed : 0.01f;
        }

        protected override void ResetInitialState()
        {
            base.ResetInitialState();

            // Если активен сразу и не в Constant/PingPong — запускаем анимацию
            if (IsAlwaysActive && !IsConstant && !IsPingPong)
            {
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