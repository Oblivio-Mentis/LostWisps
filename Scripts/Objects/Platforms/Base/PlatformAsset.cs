using Godot;

namespace LostWisps.Object
{
    [GlobalClass]
    [Tool]
    public partial class PlatformAsset : Resource
    {
        private Texture2D _texture;
        private Shape2D _collider;

        [Export]
        public Texture2D Texture
        {
            get => _texture;
            set
            {
                if (_texture == value) return;
                _texture = value;
                EmitChanged();
            }
        }

        [Export]
        public Shape2D Collider
        {
            get => _collider;
            set
            {
                if (_collider == value) return;
                _collider = value;
                EmitChanged();
            }
        }

        public bool IsValid(out string error)
        {
            if (_texture == null) { error = "Texture is null."; return false; }
            if (_collider == null) { error = "Collider is null."; return false; }
            error = null;
            return true;
        }

        public Shape2D DuplicateShape()
        {
            return _collider?.Duplicate(true) as Shape2D;
        }
    }
}
