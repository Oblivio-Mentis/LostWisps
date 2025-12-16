using Godot;

namespace LostWisps.Global
{
    public static class GlobalConstants
    {
        public static class DebugSettings
        {
            public const string SETTING_LOG_BASE_KEY = "debug/log/";
            public const string SETTING_ENABLE_ANIMATE_KEY = "debug/enable_animate_in_inspector";
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
    }
}