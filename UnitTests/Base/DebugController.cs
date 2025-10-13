using Godot;

namespace LostWisps.Debug
{
    public partial class DebugController : Node
    {
        private const string DebugPanelPath = "res://UnitTests/DebugPanel.tscn"; // путь к вашей панели
        private PackedScene _panelScene;
        private DebugPanel _panel;

        public override void _Ready()
        {
            _panelScene = GD.Load<PackedScene>(DebugPanelPath);
        }

        public override void _UnhandledInput(InputEvent @event)
        {
            if (@event.IsActionPressed("debug_toggle"))
            {
                EnsurePanel();
                _panel.Toggle();
                GetViewport().SetInputAsHandled();
            }
        }

        private void EnsurePanel()
        {
            if (_panel != null && IsInstanceValid(_panel))
                return;

            if (_panelScene == null)
            {
                GD.PushError($"DebugController: Не найден DebugPanelScene по пути {DebugPanelPath}");
                return;
            }

            _panel = _panelScene.Instantiate<DebugPanel>();
            GetTree().Root.AddChild(_panel);
            _panel.Owner = null; // не сохранять в сцену
        }
    }
}
