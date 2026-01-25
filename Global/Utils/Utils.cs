using Godot;

namespace LostWisps.Global
{
    public static class GlobalConstants
    {
        public static class DebugSettings
        {
            public const string SETTING_LOG_BASE_KEY = "debug/log/";
            public const string SETTING_ENABLE_ANIMATE_KEY = "debug/enable_animate_in_inspector";
            public const string SETTING_SHOW_LINKED_OBJECTS = "debug/show_linked_objects_in_inspector";
            public const string SETTING_SHOW_SYNCHRONIZERS_DEBUG_INFO = "debug/show_synchronizers_debug_info";
        }
    }
}

namespace LostWisps.Utils
{
    public partial class Utils
    {
        public static bool ObjectCanInteract(Node2D node2D)
        {
            return node2D.IsInGroup("InteractiveObject");
        }

        public static bool IsProjectSettingEnabled(string key)
        {
            var projectSettings = ProjectSettings.Singleton;
            bool enabled = projectSettings.HasSetting(key)
                         ? projectSettings.GetSetting(key).As<bool>()
                         : false;

            return enabled;
        }

        public static bool IsEditorSettingEnabled(string key)
        {
            var editorSettings = EditorInterface.Singleton.GetEditorSettings();
            bool enabled = editorSettings.HasSetting(key)
                         ? editorSettings.GetSetting(key).As<bool>()
                         : false;

            return enabled;
        }
    }
}