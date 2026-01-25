#nullable enable

using Godot;
using System;
using System.Diagnostics;

namespace LostWisps.Object
{

    [Tool]
    public abstract partial class ValueSynchronizer<T> : BaseSynchronizer, IActivatable, IEditorResettable, IValueReceiver where T : struct
    {
        #region AnimationModeStrategy

        interface IAnimationMode
        {
            void Update(ValueSynchronizer<T> owner, float delta);
            void OnEnter(ValueSynchronizer<T> owner);
            void OnExit(ValueSynchronizer<T> owner);
        }

        sealed class ConstantMode : IAnimationMode
        {
            public void OnEnter(ValueSynchronizer<T> owner) { }
            public void OnExit(ValueSynchronizer<T> owner) { }
            public void Update(ValueSynchronizer<T> owner, float delta) { }
        }

        sealed class NormalMode : IAnimationMode
        {
            public void OnEnter(ValueSynchronizer<T> owner) { }

            public void OnExit(ValueSynchronizer<T> owner) { }

            public void Update(ValueSynchronizer<T> owner, float delta)
            {
                if (!owner.isAnimating)
                {
                    if (owner.IsAdditive && owner.IsActivated)
                    {
                        owner.ActivateAdditive();
                    }
                    else if (owner.IsActivated)
                    {
                        owner.StartAnimation(owner.current, owner.target);
                    }
                    return;
                }

                owner.animationStartTime += delta;
                float t = owner.GetClampedProgress();
                float easedT = owner.GetEasedProgress(t);
                owner.current = t >= 1f ? owner.endValue : owner.Lerp(owner.startValue, owner.endValue, easedT);

                if (t >= 1f)
                {
                    owner.current = owner.endValue;
                    owner.isAnimating = false;
                    if (owner.externallyDeactivated || (!owner.IsAlwaysActive && !owner.externallyDeactivated))
                        owner.activatable.Deactivate();
                }
            }
        }

        sealed class PingPongMode : IAnimationMode
        {
            private enum Direction { Forward, Backward }
            private Direction dir = Direction.Forward;

            public void OnEnter(ValueSynchronizer<T> owner) { dir = Direction.Forward; }

            public void OnExit(ValueSynchronizer<T> owner) { }

            public void Update(ValueSynchronizer<T> owner, float delta)
            {
                if (!owner.isAnimating)
                {
                    T nextTarget = dir == Direction.Forward ? owner.GetTarget() : default!;
                    owner.StartAnimation(owner.current, nextTarget);
                }

                owner.animationStartTime += delta;
                float t = owner.GetClampedProgress();
                float easedT = owner.GetEasedProgress(t);
                owner.current = owner.Lerp(owner.startValue, owner.endValue, easedT);

                if (t >= 1f)
                {
                    owner.current = owner.endValue;
                    owner.isAnimating = false;

                    dir = dir == Direction.Forward ? Direction.Backward : Direction.Forward;
                    owner.target = dir == Direction.Forward ? owner.GetTarget() : default!;

                    if (dir == Direction.Backward && !owner.IsLooped)
                    {
                        owner.Deactivate();
                    }
                }
            }
        }

        #endregion

        #region ExportParams

        [Export] public float AnimationSpeed = 1.0f;

        [ExportGroup("Режимы анимации")]
        [Export] public bool IsConstant = false;
        [Export] public bool IsAdditive = false;

        [ExportSubgroup("Ping pong")]
        [Export] public bool IsPingPong = false;
        [Export] public bool IsLooped = false;

        [ExportGroup("Кривая анимации")]
        [Export] public Curve? EasingCurve = null;

        [ExportGroup("Правила активации")]
        [Export] public bool IsActiveOnStart = false;
        [Export] public bool IsAlwaysActive = false;
        [Export] public bool CanBeDeactivated = true;

        #endregion

        #region State

        private readonly ActivatableComponent activatable = new();
        private bool externallyDeactivated = false;

        private IAnimationMode currentMode = new ConstantMode();
        private IAnimationMode ConstantModeInstance = new ConstantMode();
        private IAnimationMode NormalModeInstance = new NormalMode();
        private IAnimationMode PingPongModeInstance = new PingPongMode();

        protected T current = default!;
        protected T target = default!;
        protected T startValue = default!;
        protected T endValue = default!;
        protected float animationStartTime = 0f;
        protected float animationDuration = 0f;
        protected bool isAnimating = false;

        protected T Current { get => current; set => current = value; }
        protected T Target { get => target; set => target = value; }
        protected T StartValue { get => startValue; set => startValue = value; }
        protected T EndValue { get => endValue; set => endValue = value; }
        protected float AnimationStartTime { get => animationStartTime; set => animationStartTime = value; }
        protected bool IsAnimating { get => isAnimating; set => isAnimating = value; }

        public bool IsActivated => activatable.IsActivated;

        #endregion

        #region Lifecycle

        public override void _Ready()
        {
            if (Engine.IsEditorHint() && !LostWisps.Utils.Utils.IsEditorSettingEnabled(LostWisps.Global.GlobalConstants.DebugSettings.SETTING_ENABLE_ANIMATE_KEY))
                return;
            
            ResetInitialState();
            if (IsAlwaysActive || IsConstant || IsActiveOnStart)
                activatable.Activate();
            SwitchMode();
        }

        private void SwitchMode()
        {
            currentMode.OnExit(this);
            currentMode = IsConstant ? ConstantModeInstance
                        : IsPingPong ? PingPongModeInstance
                        : NormalModeInstance;
            currentMode.OnEnter(this);
        }

        public void ResetEditorState()
        {
            // Вынести в каждый synchronizer
            if (!Engine.IsEditorHint()) return;

            ResetInitialState();
            ApplyCurrentValue();
        }

        protected virtual void ResetInitialState()
        {
            current = IsAdditive || (!IsConstant && !IsPingPong && !IsAlwaysActive && !IsActiveOnStart)
                    ? current
                    : default!;

            target = (IsConstant || IsPingPong || IsAlwaysActive || IsActiveOnStart)
                   ? GetTarget()
                   : default!;
        }

        public override void _PhysicsProcess(double delta)
        {
            if (Engine.IsEditorHint() && !LostWisps.Utils.Utils.IsEditorSettingEnabled(LostWisps.Global.GlobalConstants.DebugSettings.SETTING_ENABLE_ANIMATE_KEY))
                return;
                
            if (!IsActivated || TargetNodes.Length == 0) return;

            currentMode.Update(this, (float)delta);
            ApplyCurrentValue();
        }

        #endregion

        #region Helpers

        protected void StartAnimation(T from, T to)
        {
            startValue = from;
            endValue = to;
            animationStartTime = 0f;
            animationDuration = CalculateAnimationDuration(from, to);
            isAnimating = true;
        }

        protected float GetClampedProgress() =>
            animationDuration > 0f
                ? Mathf.Clamp(animationStartTime / animationDuration, 0f, 1f)
                : 1f;

        protected virtual float GetEasedProgress(float t) =>
            EasingCurve?.SampleBaked(Mathf.Clamp(t, 0f, 1f)) ?? t;

        #endregion

        #region PublicAPI

        public void SetValue(float value)
        {
            if (IsPingPong) return;

            value = Mathf.Clamp(value, -1f, 1f);
            activatable.Activate();
            target = ValueToTargetDirect(value);

            if (!IsAdditive) StartAnimation(current, target);
        }

        public void SetInstantValue(float value)
        {
            if (IsPingPong) return;

            value = Mathf.Clamp(value, -1f, 1f);
            current = ValueToTargetDirect(value);
            target = current;
            SetInitialOffsets();
            ApplyCurrentValue();
            isAnimating = false;
        }

        public void Activate()
        {
            if (IsConstant)
            {
                externallyDeactivated = false;
                activatable.Activate();
                return;
            }

            if (activatable.IsActivated && !externallyDeactivated)
                return;

            externallyDeactivated = false;
            activatable.Activate();

            if (IsAdditive)
            {
                ActivateAdditive();
            }
            else
            {
                target = GetTarget();
                if (!isAnimating)
                    StartAnimation(current, target);
            }
        }

        public void Deactivate()
        {
            if (!CanBeDeactivated) return;

            if (IsAlwaysActive) return;

            externallyDeactivated = true;

            if (IsConstant)
            {
                current = default;
                activatable.Deactivate();
                return;
            }

            if (IsAdditive)
            {
                isAnimating = false;
                activatable.Deactivate();
            }
            else
            {
                StartAnimation(current, default!);
                target = default!;
                activatable.Activate();
            }
        }

        #endregion

        #region AbstractMembers

        protected abstract void SetInitialOffsets();
        protected abstract void ApplyCurrentValue();
        public abstract T Lerp(T from, T to, float t);
        public abstract T GetTarget();
        public abstract T GetNextTarget(T from);
        public abstract T ValueToTarget(float normalizedValue);
        public abstract T ValueToTargetDirect(float value);
        protected abstract void ActivateAdditive();

        protected virtual float CalculateAnimationDuration(T from, T to)
        {
            return AnimationSpeed > 0 ? AnimationSpeed : 0.01f;
        }

        #endregion
    }
}
