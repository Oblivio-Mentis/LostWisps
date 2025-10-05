using Godot;
using System;
using System.Collections.Generic;

namespace LostWisps.Object
{
    public partial class Slider : Line2D
    {
        [Export] public float InitialValue { get; set; } = 0.0f;
        [Export] public Node2D[] TargetNodes { get; private set; }

        private RigidBody2D marker;
        private Vector2 startPoint;
        private Vector2 endPoint;
        private Vector2 axis = Vector2.Zero;
        private float length = 0.0f;
        private HashSet<Node2D> activeBodies = new();

        public override void _Ready()
        {
            InitializeMarkerAndTrack();
            ValidateTrackPoints();
            CalculateAxisAndLength();
            SetMarkerToInitialPosition();
            ValidateTargetNodes();
        }

        public override void _PhysicsProcess(double delta)
        {
            ConstrainMarkerToTrack();
            UpdateTargetsIfActive();

            if (activeBodies.Count == 0)
            {
                ResetMarkerVelocity();
            }
        }

        private void InitializeMarkerAndTrack()
        {
            marker ??= GetNode<RigidBody2D>("Marker");
        }

        private void ValidateTrackPoints()
        {
            if (Points.Length < 2)
            {
                GD.PushError("Track должен содержать минимум две точки.");
                return;
            }

            Vector2 localStart = GetPointPosition(0);
            Vector2 localEnd = GetPointPosition((int)Points.Length - 1);

            startPoint = GlobalPosition + localStart;
            endPoint = GlobalPosition + localEnd;
        }

        private void CalculateAxisAndLength()
        {
            axis = (endPoint - startPoint).Normalized();
            length = endPoint.DistanceTo(startPoint);
        }

        private void SetMarkerToInitialPosition()
        {
            float clampedValue = Mathf.Clamp(InitialValue, -1, 1);

            float distance = Mathf.Remap(clampedValue, -1f, 1f, 0f, length);
            Vector2 position = startPoint + axis * distance;

            marker.GlobalPosition = position;
        }

        private void ValidateTargetNodes()
        {
            foreach (var node in TargetNodes)
            {
                if (node is null)
                {
                    GD.PushWarning($"[{nameof(Slider)}] Найден null в массиве TargetNodes.");
                    continue;
                }

                if (!(node is IValueReceiver))
                {
                    string nodePath = node.GetPath();
                    GD.PushWarning($"[{nameof(Slider)}] Узел '{nodePath}' не реализует интерфейс {nameof(IValueReceiver)} и будет проигнорирован.");
                }
            }
        }

        private void UpdateTargetsIfActive()
        {
            if (activeBodies.Count == 0)
                return;

            float value = Mathf.Round(GetNormalizedValue() * 10000) / 10000;

            foreach (var node in TargetNodes)
            {
                if (node is IValueReceiver receiver)
                {
                    receiver.SetValue(value);
                }
            }
        }

        private void ResetMarkerVelocity()
        {
            marker.LinearVelocity = Vector2.Zero;
            marker.AngularVelocity = 0f;
        }

        private void OnBodyEntered(Node2D body)
        {
            if (body == marker || !CanInteract(body))
                return;

            activeBodies.Add(body);
        }

        private void OnBodyExited(Node2D body)
        {
            activeBodies.Remove(body);
        }

        private void ConstrainMarkerToTrack()
        {
            if (axis.IsZeroApprox())
                return;

            Vector2 pos = marker.GlobalPosition;
            Vector2 toPos = pos - startPoint;
            float t = toPos.Dot(axis);

            t = Mathf.Clamp(t, 0, length);

            Vector2 constrainedPos = startPoint + axis * t;
            marker.GlobalPosition = constrainedPos;
        }

        public float GetNormalizedValue()
        {
            Vector2 pos = marker.GlobalPosition;
            float distance = pos.DistanceTo(startPoint);
            return Mathf.Remap(distance, 0, length, -1, 1);
        }

        private bool CanInteract(Node2D body)
        {
            return body is CharacterBody2D or RigidBody2D;
        }
    }
}