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

            EditorInterface.Singleton.GetEditorSettings().SettingsChanged += OnEditorSettingsChanged;
        }

        public override void _ExitTree()
        {
            EditorInterface.Singleton.GetEditorSettings().SettingsChanged -= OnEditorSettingsChanged;

            if (PanelInstance != null)
            {
                RemoveControlFromContainer(CustomControlContainer.ProjectSettingTabRight, PanelInstance);
                PanelInstance.QueueFree();
                PanelInstance = null;
            }
        }

        private void OnEditorSettingsChanged()
        {
            bool enabled = LostWisps.Utils.Utils.IsEditorSettingEnabled(LostWisps.Global.GlobalConstants.DebugSettings.SETTING_ENABLE_ANIMATE_KEY);

            if (!enabled)
                ResetAllResettableNodesInCurrentScene();
        }

        private void ResetAllResettableNodesInCurrentScene()
        {
            var editedScene = EditorInterface.Singleton.GetEditedSceneRoot();
            if (editedScene == null) return;

            ResetResettableNodesRecursive(editedScene);
        }

        private void ResetResettableNodesRecursive(Node node)
        {
            if (node is LostWisps.Object.IEditorResettable resettable)
            {
                try
                {
                    resettable.ResetEditorState();
                }
                catch (System.Exception e)
                {
                    GD.PushWarning($"[LostWisps] Failed to reset {node.GetPath()}: {e.Message}");
                }
            }

            foreach (Node child in node.GetChildren())
            {
                ResetResettableNodesRecursive(child);
            }
        }
    }
}

#endif