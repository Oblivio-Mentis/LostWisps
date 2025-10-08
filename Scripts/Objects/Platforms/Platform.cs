using Godot;

namespace LostWisps.Object
{
    [Tool]
    public partial class Platform : AnimatableBody2D
    {
        private Sprite2D _sprite;
        private CollisionShape2D _collisionShape;

        private PlatformAsset _asset;
        private bool _assetConnected;

        [Export]
        public PlatformAsset Asset
        {
            get => _asset;
            set
            {
                if (_asset == value) return;

                UnsubscribeFromAsset(_asset);
                _asset = value;
                SubscribeToAsset(_asset);

                ApplyAsset();
            }
        }

        public override void _EnterTree()
        {
            if (_asset != null && !_assetConnected)
                SubscribeToAsset(_asset);
        }

        public override void _ExitTree()
        {
            UnsubscribeFromAsset(_asset);
        }

        public override void _Ready()
        {
            _sprite = GetNodeOrNull<Sprite2D>("Texture");
            _collisionShape = GetNodeOrNull<CollisionShape2D>("Collider");

            ApplyAsset();
        }

        public override void _Notification(int what)
        {
            if (Engine.IsEditorHint())
            {
                if (what == NotificationEditorPostSave)
                    ApplyAsset();
            }
        }

        private void SubscribeToAsset(PlatformAsset asset)
        {
            if (asset == null || _assetConnected) return;
            asset.Changed += OnAssetChanged;
            _assetConnected = true;
        }

        private void UnsubscribeFromAsset(PlatformAsset asset)
        {
            if (asset == null || !_assetConnected) return;
            asset.Changed -= OnAssetChanged;
            _assetConnected = false;
        }

        private void OnAssetChanged()
        {
            ApplyAsset();
        }

        private void ApplyAsset()
        {
            if (_sprite == null || _collisionShape == null)
                return;

            if (_asset == null)
            {
                _sprite.Texture = null;
                _collisionShape.Shape = null;
                _collisionShape.Disabled = true;
                return;
            }

            _sprite.Texture = _asset.Texture;

            var dupShape = _asset.DuplicateShape();
            _collisionShape.Shape = dupShape;
            _collisionShape.Disabled = dupShape == null;
        }
    }
}
