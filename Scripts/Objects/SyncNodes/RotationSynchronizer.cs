#nullable enable

using Godot;
using System;

namespace LostWisps.Object
{
    public partial class RotationSynchronizer : Path2D, IActivatable, IValueReceiver
    {
        [Export] public float Speed = 1.0f;
        [Export] public bool CanDeactivate = true;
        [Export] public bool IsAlwaysActive = false;
        [Export] public bool IsPathLooped = false;
        [Export] public Node2D[] TargetNodes = Array.Empty<Node2D>();

        private float[] offsets = Array.Empty<float>();
        private PathFollow2D? pathFollow2D;
        private AnimationPlayer? animationPlayer;
        private RemoteTransform2D? remoteTransform;
        private float lastPathRotation;

        private readonly ActivatableComponent _activatable;

        public RotationSynchronizer() => _activatable = new();

        public bool IsActivated => _activatable.IsActivated;

        public override void _Ready()
        {
            pathFollow2D = GetNodeOrNull<PathFollow2D>("PathFollow2D");
            animationPlayer = GetNodeOrNull<AnimationPlayer>("AnimationPlayer");

            if (pathFollow2D == null || animationPlayer == null)
            {
                GD.PushError("Не найдены обязательные ноды: PathFollow2D или AnimationPlayer");
                return;
            }

            int count = TargetNodes?.Length ?? 0;
            offsets = new float[count];

            float currentRotation = pathFollow2D.GlobalRotation;

            for (int i = 0; i < count; i++)
            {
                if (TargetNodes[i] != null)
                    offsets[i] = TargetNodes[i].GlobalRotation - currentRotation;
            }

            animationPlayer.SpeedScale = Speed;

            if (IsAlwaysActive)
            {
                string animName = IsPathLooped ? "IsPathLooped" : "IsPathNotLooped";
                animationPlayer.Play(animName);
            }

            lastPathRotation = currentRotation;
        }

        public override void _PhysicsProcess(double delta)
        {
            if (pathFollow2D == null || TargetNodes == null || TargetNodes.Length == 0)
                return;

            float currentPathRotation = pathFollow2D.GlobalRotation;

            if (Mathf.IsEqualApprox(currentPathRotation, lastPathRotation))
                return;

            lastPathRotation = currentPathRotation;

            float pathAngle = currentPathRotation;

            for (int i = 0; i < TargetNodes.Length; i++)
            {
                if (TargetNodes[i] != null)
                    TargetNodes[i].GlobalRotation = pathAngle + offsets[i];
            }
        }

        public void SetValue(float value)
        {
            if (pathFollow2D == null)
                return;

            float clamped = Mathf.Remap(value, -1f, 1f, 0f, 1f);
            clamped = Mathf.Clamp(clamped, 0f, 1f);

            pathFollow2D.ProgressRatio = clamped;

            pathFollow2D.ForceUpdateTransform();
        }

        public void Activate()
        {
            if (_activatable.IsActivated || IsAlwaysActive || animationPlayer == null)
                return;

            _activatable.Activate();
            animationPlayer.Play("Activate");
        }

        public void Deactivate()
        {
            if (!CanDeactivate || animationPlayer == null)
                return;

            _activatable.Deactivate();
            animationPlayer.PlayBackwards("Activate");
        }
    }
}