#if TOOLS

using Godot;
using LostWisps.Plugins;

namespace LostWisps.Plugins
{
    [Tool]
    public partial class LostWispsDebugPlugin : EditorPlugin
    {
        private LostWispsDebugPluginPanel? PanelInstance;

        public override void _EnterTree()
        {
            var scene = ResourceLoader.Load<PackedScene>("res://addons/LostWispsDebug/LostWispsDebugPluginPanel.tscn");
            if (scene == null)
            {
                GD.PushError("LostWispsDebugPluginPanel.tscn not found.");
                return;
            }

            PanelInstance = scene.Instantiate<LostWispsDebugPluginPanel>();
            AddControlToContainer(CustomControlContainer.ProjectSettingTabRight, PanelInstance);
            PanelInstance.Initialize(EditorInterface.Singleton);
        }

        public override void _ExitTree()
        {
            if (PanelInstance != null)
            {
                RemoveControlFromContainer(CustomControlContainer.ProjectSettingTabRight, PanelInstance);
                PanelInstance.QueueFree();
                PanelInstance = null;
            }
        }
    }
}

#endif