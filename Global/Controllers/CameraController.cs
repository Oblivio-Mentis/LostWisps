using Godot;
using System;

namespace LostWisps.Global
{
    public partial class CameraController : Camera2D
    {
        [Export] private float zoomSpeed = 0.5f;
        [Export] private float minZoom = 0.5f;
        [Export] private float maxZoom = 2.0f;

        public override void _Input(InputEvent @event)
        {
            if (@event.IsActionPressed("zoom_in"))
            {
                Zoom = new Vector2(
                    Mathf.Clamp(Zoom.X + zoomSpeed * (float)GetProcessDeltaTime(), minZoom, maxZoom),
                    Mathf.Clamp(Zoom.Y + zoomSpeed * (float)GetProcessDeltaTime(), minZoom, maxZoom)
                );
            }

            if (@event.IsActionPressed("zoom_out"))
            {
                Zoom = new Vector2(
                    Mathf.Clamp(Zoom.X - zoomSpeed * (float)GetProcessDeltaTime(), minZoom, maxZoom),
                    Mathf.Clamp(Zoom.Y - zoomSpeed * (float)GetProcessDeltaTime(), minZoom, maxZoom)
                );
            }
        }
    }
}

