#nullable enable

using Godot;
using System;

namespace LostWisps.Object
{
    public abstract partial class Synchronizer<T> : BaseSynchronizer, IActivatable, IValueReceiver
    {
        [Export] public float Speed = 1.0f;

        [ExportGroup("🔄 Режимы")]
        [Export] public bool IsConstant = false;
        [Export] public bool PingPong = false;
        [Export] public bool IsPathLooped = false;

        [ExportGroup("📈 Анимация")]
        [Export] public Curve? EasingCurve = null;

        [ExportGroup("🎯 Управление активацией")]
        [Export] public bool IsAlwaysActive = false;
        [Export] public bool CanBeDeactivated = true;

        protected readonly ActivatableComponent _activatable = new();
        private bool _manuallyDeactivated = false;

        protected T current = default!;
        protected T target = default!;

        protected float animationStartTime = 0f;
        protected float animationDuration = 0f;
        protected T startValue = default!;
        protected T endValue = default!;
        protected bool isAnimating = false;

        private enum PingPongState { Forward, Backward }
        private PingPongState pingPongState = PingPongState.Forward;

        public bool IsActivated => _activatable.IsActivated;

        public override void _Ready()
        {
            ResetState();
            if (IsAlwaysActive || IsConstant)
                _activatable.Activate();
        }

        protected virtual void ResetState()
        {
            current = default!;
            target = IsConstant || PingPong || IsAlwaysActive ? GetTarget() : default!;
        }

        public override void _PhysicsProcess(double delta)
        {
            if (!IsActivated || TargetNodes.Length == 0)
                return;

            if (IsConstant)
            {
                UpdateConstant((float)delta);
            }
            else if (PingPong)
            {
                UpdatePingPong((float)delta);
            }
            else
            {
                UpdateNormal((float)delta);
            }

            ApplyCurrentValue();
        }

        protected virtual void UpdateConstant(float delta)
        {
        }

        protected virtual void UpdateNormal(float delta)
        {
            if (!isAnimating)
            {
                startValue = current;
                endValue = target;
                animationStartTime = 0f;
                animationDuration = CalculateAnimationDuration(startValue, endValue);
                isAnimating = true;
            }

            animationStartTime += delta;

            float t = Mathf.Clamp(animationStartTime / animationDuration, 0f, 1f);
            float easedT = GetEasedProgress(t);

            current = Lerp(startValue, endValue, easedT);

            if (t >= 1f)
            {
                current = endValue;
                isAnimating = false;

                if (endValue.Equals(default(T)) && !_manuallyDeactivated && !IsAlwaysActive)
                {
                    _activatable.Deactivate();
                }
            }
        }

        protected virtual void UpdatePingPong(float delta)
        {
            if (!isAnimating)
            {
                startValue = current;
                endValue = pingPongState == PingPongState.Forward ? GetTarget() : default!;
                animationStartTime = 0f;
                animationDuration = CalculateAnimationDuration(startValue, endValue);
                isAnimating = true;
            }

            animationStartTime += delta;

            float t = Mathf.Clamp(animationStartTime / animationDuration, 0f, 1f);
            float easedT = GetEasedProgress(t);

            current = Lerp(startValue, endValue, easedT);

            if (t >= 1f)
            {
                current = endValue;
                isAnimating = false;
                SwitchPingPongDirection();
            }
        }

        protected virtual void SwitchPingPongDirection()
        {
            if (pingPongState == PingPongState.Forward)
            {
                pingPongState = PingPongState.Backward;
                target = default!;
            }
            else
            {
                pingPongState = PingPongState.Forward;
                target = GetTarget();

                if (!IsPathLooped)
                {
                    Deactivate();
                    return;
                }
            }

            isAnimating = false;
        }

        protected virtual float CalculateAnimationDuration(T from, T to)
        {
            return Speed > 0 ? Speed : 0.01f;
        }

        protected virtual float GetEasedProgress(float t) =>
            EasingCurve?.SampleBaked(Mathf.Clamp(t, 0f, 1f)) ?? t;

        protected abstract void ApplyCurrentValue();

        public abstract T Lerp(T from, T to, float t);

        public abstract T GetTarget();

        public abstract T ValueToTarget(float normalizedValue);

        public abstract T ValueToTargetDirect(float value);

        public void SetValue(float value)
        {
            if (PingPong)
                return;

            value = Mathf.Clamp(value, -1f, 1f);
            _activatable.Activate();
            target = ValueToTargetDirect(value);
            isAnimating = false;
        }

        public void Activate()
        {
            if (IsConstant)
            {
                if (_activatable.IsActivated)
                    return;

                _manuallyDeactivated = false;
                _activatable.Activate();
                return;
            }

            if (_activatable.IsActivated && !_manuallyDeactivated)
                return;

            isAnimating = false;
            _manuallyDeactivated = false;

            _activatable.Activate();

            if (PingPong)
            {
                pingPongState = PingPongState.Forward;
                target = GetTarget();
            }
            else
            {
                target = GetTarget();
            }
        }

        public void Deactivate()
        {
            if (!CanBeDeactivated)
                return;

            _manuallyDeactivated = true;

            if (IsConstant)
            {
                _activatable.Deactivate();
                return;
            }

            target = default!;
            StartReturnAnimation();
        }

        private void StartReturnAnimation()
        {
            if (!_manuallyDeactivated && isAnimating)
            {
                isAnimating = false;
                return;
            }

            startValue = current;
            endValue = default!;

            if (startValue.Equals(endValue))
            {
                _activatable.Deactivate();
                return;
            }

            animationStartTime = 0f;
            animationDuration = CalculateAnimationDuration(startValue, endValue);
            isAnimating = true;

            _activatable.Activate();
        }
    }
}