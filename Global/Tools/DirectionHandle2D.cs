using Godot;
using System;

namespace LostWisps.Object
{
    [Tool]
    public partial class DirectionHandle2D : Node2D
    {
        [ExportGroup("🎛️ Handle")]
        [Export(PropertyHint.Range, "10,1000,1")]
        public float HandleRadius = 80f;

        [ExportGroup("🎨 Gizmo")]
        [Export]
        public Color GizmoColor = new Color(0.3f, 0.8f, 1f, 0.8f);

        [ExportGroup("🧭 Direction")]
        public Vector2 Direction => _direction;

        private Vector2 _direction = Vector2.Right;
        private bool _dragging = false;

        [Signal]
        public delegate void DirectionChangedEventHandler();

        public override void _Ready()
        {
            if (_direction == Vector2.Zero)
                _direction = Vector2.Right;
            UpdateGizmo();
        }

        public override void _Input(InputEvent @event)
        {
            if (!IsInsideTree())
                return;

            if (@event is InputEventMouseButton mb && mb.ButtonIndex == MouseButton.Left)
            {
                var localMouse = ToLocal(GetViewport().GetMousePosition());
                if (mb.Pressed)
                {
                    if (localMouse.Length() <= HandleRadius * 1.25f)
                        _dragging = true;
                }
                else
                {
                    _dragging = false;
                }
            }

            if (_dragging && @event is InputEventMouseMotion)
            {
                var localMouse = ToLocal(GetViewport().GetMousePosition());
                if (localMouse.Length() > 0.0001f)
                {
                    _direction = localMouse.Normalized();
                    UpdateGizmo();
                    EmitSignal(SignalName.DirectionChanged);
                }
            }
        }

        public void SetDirection(Vector2 dir)
        {
            if (dir.LengthSquared() > 0)
            {
                _direction = dir.Normalized();
                UpdateGizmo();
                EmitSignal(SignalName.DirectionChanged);
            }
        }

        private void UpdateGizmo() => QueueRedraw();

        public override void _Draw()
        {
            var center = Vector2.Zero;
            var end = _direction * HandleRadius;
            DrawLine(center, end, GizmoColor, 3f);
            var headA = end + _direction.Rotated(Mathf.DegToRad(150)) * 12f;
            var headB = end + _direction.Rotated(Mathf.DegToRad(-150)) * 12f;
            DrawLine(end, headA, GizmoColor, 3f);
            DrawLine(end, headB, GizmoColor, 3f);
        }
    }
}
