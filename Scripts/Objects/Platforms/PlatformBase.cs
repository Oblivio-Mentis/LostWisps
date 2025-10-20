using Godot;
using System;
using System.Drawing;

namespace LostWisps.Object
{
    [Tool]
    public partial class PlatformBase : Sprite2D
    {
        public override void _Ready()
        {
            RegionRect = new Rect2(Vector2.Zero, Texture.GetSize());

            // Texture = GetNodeOrNull<Sprite2D>("Texture");
            // Collider = GetNodeOrNull<CollisionShape2D>("Collider");

            // ApplyAsset();
        }

        public override void _Notification(int what)
        {
            RegionRect = new Rect2(Vector2.Zero, Texture.GetSize());
            // ApplyAsset();
            // if (Engine.IsEditorHint())
            // {
            //     // if (what == NotificationEditorPostSave)
            //         ApplyAsset();
            // }
        }

        private void ApplyAsset()
        {
            // if (Texture == null || Texture.Texture == null)
            //     return;

            // GD.Print("asd");
            // Texture.RegionRect = new Rect2(Vector2.Zero, Texture.Texture.GetSize());

        }
    }
}
