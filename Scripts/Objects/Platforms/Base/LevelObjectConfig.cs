using Godot;
using System;

namespace LostWisps.Object
{
    public enum BodyType { StaticBody2D, AnimatableBody2D }
    public enum ColliderType { Rectangle, Circle, Polygon }

    [Tool]
    [GlobalClass]
    public partial class LevelObjectConfig : Resource
    {
        [ExportGroup("🖼️ Базовые настройки")]
        [Export] public Texture2D TextureAtlas { get; set; }
        [Export] public Rect2 TextureRegion { get; set; }
        [Export] public BodyType BodyType { get; set; } = BodyType.StaticBody2D;

        [Export]public ColliderType ColliderType { get; set; } = ColliderType.Rectangle;

        [ExportGroup("🟥 Настройки коллайдера")]
        [Export] public Vector2 ColliderSize { get; set; } = new Vector2(64, 64);
        [Export] public float CircleRadius { get; set; } = 32.0f;
        [Export] public Vector2[] PolygonPoints { get; set; }
    }
}