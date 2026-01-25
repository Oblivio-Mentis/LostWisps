using Godot;
using System;
using System.Collections.Generic;
using System.IO;

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
        private static readonly Dictionary<LogCategory, bool> _enabledCategories = new();
        private static readonly bool _isEnabled = OS.IsDebugBuild() || Engine.IsEditorHint();

        public static void InitializeFromProjectSettings()
        {
            var projectSettings = ProjectSettings.Singleton;
            _enabledCategories.Clear();

            foreach (LogCategory category in Enum.GetValues<LogCategory>())
            {
                string key = LostWisps.Global.GlobalConstants.DebugSettings.SETTING_LOG_BASE_KEY + category.ToString().ToLower();
                _enabledCategories[category] = LostWisps.Utils.Utils.IsProjectSettingEnabled(key);
            }
        }

        public static void SetCategoryEnabled(LogCategory category, bool enabled)
        {
            _enabledCategories[category] = enabled;
        }

        public static bool IsCategoryEnabled(LogCategory category)
        {
            // Ленивая инициализация: если словарь пуст — загружаем настройки
            if (_enabledCategories.Count == 0)
            {
                InitializeFromProjectSettings();
            }
            return _isEnabled && _enabledCategories.GetValueOrDefault(category, true);
        }

        public static void Log(LogCategory category, string message, Node context = null)
        {
            if (!IsCategoryEnabled(category)) return;
            GD.Print(FormatMessage(category, message, context));
        }

        public static void Warn(LogCategory category, string message, Node context = null)
        {
            if (!IsCategoryEnabled(category)) return;
            GD.PushWarning(FormatMessage(category, message, context));
        }

        public static void Error(LogCategory category, string message, Node context = null)
        {
            // Ошибки всегда выводим, даже если категория отключена
            GD.PushError(FormatMessage(category, message, context));
        }

        private static string FormatMessage(LogCategory category, string message, Node context)
        {
            string prefix = $"[{category}]";

            if (context != null && context.IsInsideTree())
            {
                string sceneName = !string.IsNullOrEmpty(context.SceneFilePath)
                    ? Path.GetFileNameWithoutExtension(context.SceneFilePath)
                    : "UnknownScene";
                string nodeName = context.Name;
                prefix += $" [{sceneName}/{nodeName}]";
            }

            return $"{prefix} {message}";
        }
    }
}