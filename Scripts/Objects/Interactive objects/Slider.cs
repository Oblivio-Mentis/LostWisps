using Godot;
using System.Collections.Generic;

namespace LostWisps.Object
{
    public partial class Slider : Line2D
    {
        [Export] public Node2D[] TargetNodes { get; private set; }
        private IValueReceiver[] targets;
        private RigidBody2D marker;
        private Line2D track;
        private Vector2 startPoint;
        private Vector2 endPoint;
        private Vector2 axis = Vector2.Zero;
        private float length = 0.0f;
        private HashSet<Node2D> activeBodies = new HashSet<Node2D>();

        public override void _Ready()
        {
            marker = GetNode<RigidBody2D>("Marker");
            track = GetNode<Line2D>(".");

            if (marker == null)
            {
                GD.PushError("Marker не найден в слайдере.");
                return;
            }

            if (track == null)
            {
                GD.PushError("Track не найден в слайдере.");
                return;
            }

            if (track.Points.Length < 2)
            {
                GD.PushError("Track должен содержать минимум две точки.");
                return;
            }

            Vector2 localStart = track.GetPointPosition(0);
            Vector2 localEnd = track.GetPointPosition((int)track.Points.Length - 1);

            startPoint = track.GlobalPosition + localStart;
            endPoint = track.GlobalPosition + localEnd;

            axis = (endPoint - startPoint).Normalized();
            length = endPoint.DistanceTo(startPoint);

            var list = new List<IValueReceiver>();

            foreach (var node in TargetNodes)
            {
                if (node is IValueReceiver valueReceiver)
                {
                    list.Add(valueReceiver);
                }
                else
                {
                    GD.PushWarning($"Node at path {node} does not implement IValueReceiver.");
                }
            }

            targets = list.ToArray();
        }

        public override void _PhysicsProcess(double delta)
        {
            ConstrainMarkerToTrack();

            if (activeBodies.Count == 0)
            {
                marker.LinearVelocity = Vector2.Zero;
                marker.AngularVelocity = 0f;
            }

            UpdateTargetNodes();
        }

        private void UpdateTargetNodes()
        {
            if (activeBodies.Count == 0)
                 return;

            float value = Mathf.Round(GetNormalizedValue() * 10000) / 10000;

            foreach (var target in targets)
            {
                if (target == null)
                    continue;

                target.SetValue(value);
            }
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
            {
                return;
            }

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
            return body is CharacterBody2D || body is RigidBody2D;
        }
    }
}