using Godot;
using System;

namespace LostWisps.Object
{
    [Tool]
    public partial class LevelObject : Node2D
    {
        [Export] public LevelObjectConfig Config { get; set; }

        [ExportGroup("🔧 References (set in editor)")]
        [Export] private Sprite2D texture;
        [Export] private CollisionShape2D rectangleCollider;
        [Export] private CollisionShape2D circleCollider;
        [Export] private CollisionPolygon2D polygonCollider;

        public override void _Ready()
        {
            ApplyConfig();
        }

        public override void _Notification(int what)
        {
            if (Engine.IsEditorHint() && what == NotificationEnterTree)
            {
                ApplyConfig();
            }
        }

        private void ResetBody()
        {
            texture.Texture = null;
            rectangleCollider.Disabled = true;
            circleCollider.Disabled = true;
            polygonCollider.Disabled = true;
        }

        private void ApplyConfig()
        {
            GD.Print("awsd");

            if (Config == null)
            {
                GD.PrintErr("LevelObject: Config is null.");
                ResetBody();
                return;
            }

            if (Config.TextureAtlas == null)
            {
                GD.PrintErr("LevelObject: TextureAtlas is null in Config.");
                ResetBody();
                return;
            }

            if (Config.ColliderType == ColliderType.Polygon && (Config.PolygonPoints == null || Config.PolygonPoints.Length < 3))
            {
                GD.Print("LevelObject: Polygon collider requires at least 3 points.");
            }

            ResetBody();

            texture.Texture = Config.TextureAtlas;
            texture.RegionEnabled = true;
            texture.RegionRect = Config.TextureRegion;

            switch (Config.ColliderType)
            {
                case ColliderType.Rectangle:
                    rectangleCollider.Disabled = false;

                    if (rectangleCollider.Shape is RectangleShape2D rectangleShape2D)
                        rectangleShape2D.Size = Config.ColliderSize;

                    break;

                case ColliderType.Circle:
                    circleCollider.Disabled = false;

                    if (circleCollider.Shape is CircleShape2D circleShape2D)
                        circleShape2D.Radius = Config.CircleRadius;

                    break;

                case ColliderType.Polygon:
                    polygonCollider.Disabled = false;

                    polygonCollider.Polygon = Config.PolygonPoints;

                    break;

                default:
                    GD.PrintErr($"LevelObject: Unknown ColliderType: {Config.ColliderType}");
                    break;
            }
        }
    }
}
