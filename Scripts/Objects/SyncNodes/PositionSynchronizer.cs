#nullable enable

using Godot;
using System;

namespace LostWisps.Object
{
    public partial class PositionSynchronizer : Path2D, IActivatable, IValueReceiver
    {
        [Export] public float Speed = 1.0f;
        [Export] public bool CanDeactivate = true;
        [Export] public bool IsAlwaysActive = false;
        [Export] public bool IsPathLooped = false;
        [Export] public Node2D[] TargetNodes = Array.Empty<Node2D>();

        private Vector2[] offsets = Array.Empty<Vector2>();
        private PathFollow2D? pathFollow2D;
        private AnimationPlayer? animationPlayer;
        private RemoteTransform2D? remoteTransform;
        private Vector2 lastPathPosition;

        private readonly ActivatableComponent _activatable = new();

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
            offsets = new Vector2[count];

            Vector2 currentPathPosition = pathFollow2D.GlobalPosition;

            for (int i = 0; i < count; i++)
            {
                if (TargetNodes[i] != null)
                    offsets[i] = TargetNodes[i].Position - pathFollow2D.Position;
            }

            animationPlayer.SpeedScale = Speed;

            if (IsAlwaysActive)
            {
                _activatable.Activate();
                string animName = IsPathLooped ? "IsPathLooped" : "IsPathNotLooped";
                animationPlayer.Play(animName);
            }

            lastPathPosition = currentPathPosition;
        }

        public override void _PhysicsProcess(double delta)
        {
            if (pathFollow2D == null || TargetNodes == null || TargetNodes.Length == 0)
                return;

            Vector2 currentPathPosition = pathFollow2D.GlobalPosition;

            if (currentPathPosition.DistanceTo(lastPathPosition) < 1e-4f)
                return;

            lastPathPosition = currentPathPosition;

            Vector2 pathPoint = pathFollow2D.Position;

            for (int i = 0; i < TargetNodes.Length; i++)
            {
                if (TargetNodes[i] != null)
                    TargetNodes[i].Position = pathPoint + offsets[i];
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