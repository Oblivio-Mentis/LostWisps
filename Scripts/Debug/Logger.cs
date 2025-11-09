using Godot;
using System;
using System.Collections.Generic;

namespace LostWisps.Debug
{
    public enum LogCategory
    {
        Player,
        Synchronizer,
        Destruction,
        Interaction,
        Raycast,
        UI,
        General
    }

    public static class Logger
    {
        private static readonly Dictionary<LogCategory, bool> _enabledCategories = new()
        {
            { LogCategory.Player, true },
            { LogCategory.Synchronizer, true },
            { LogCategory.Destruction, true },
            { LogCategory.Interaction, true },
            { LogCategory.Raycast, true },
            { LogCategory.UI, true },
            { LogCategory.General, true }
        };

        private static readonly bool _isEnabled = OS.IsDebugBuild() || Engine.IsEditorHint();

        public static void SetCategoryEnabled(LogCategory category, bool enabled)
        {
            if (_enabledCategories.ContainsKey(category))
                _enabledCategories[category] = enabled;
        }

        public static bool IsCategoryEnabled(LogCategory category)
        {
            return _isEnabled && _enabledCategories.GetValueOrDefault(category, false);
        }

        public static void Log(LogCategory category, string message, Node context = null)
        {
            if (!_isEnabled || !IsCategoryEnabled(category)) return;
            GD.Print(FormatMessage(category, message, context));
        }

        public static void Warn(LogCategory category, string message, Node context = null)
        {
            if (!_isEnabled || !IsCategoryEnabled(category)) return;
            GD.PushWarning(FormatMessage(category, message, context));
        }

        public static void Error(LogCategory category, string message, Node context = null)
        {
            if (!_isEnabled) return;
            GD.PushError(FormatMessage(category, message, context));
        }

        private static string FormatMessage(LogCategory category, string message, Node context)
        {
            string prefix = $"[{category}]";

            if (context != null && context.IsInsideTree())
            {
                string sceneName = context.SceneFilePath != "" 
                    ? System.IO.Path.GetFileNameWithoutExtension(context.SceneFilePath) 
                    : "UnknownScene";
                string nodeName = context.Name;
                prefix += $" [{sceneName}/{nodeName}]";
            }

            return $"{prefix} {message}";
        }
    }
}