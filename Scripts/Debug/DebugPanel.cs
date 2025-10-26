using Godot;
using System;
using System.Collections.Generic;

namespace LostWisps.Debug
{
    public partial class DebugPanel : Control
    {
        private VBoxContainer LevelsGroup;

        private const string LevelsFolderPath = "res://Debug/";

        private Dictionary<LogCategory, CheckBox> LogCheckboxes = new();

        public override void _Ready()
        {
            Visible = false;

            LevelsGroup = GetNode<VBoxContainer>("PanelContainer/MarginContainer/VBoxContainer/LevelsGroup");

            AddLevelButtons();
            AddLogCategoryCheckboxes();
        }

        private void AddLogCategoryCheckboxes()
        {
            var logGroup = GetNode<VBoxContainer>("PanelContainer/MarginContainer/VBoxContainer/LogCategoriesGroup");
            if (logGroup == null)
            {
                Logger.Warn(LogCategory.UI, "LogCategoriesGroup is not found in the DebugPanel.", this);
                return;
            }

            foreach (LogCategory category in Enum.GetValues<LogCategory>())
            {
                var cb = new CheckBox
                {
                    Text = $"Log: {category}",
                    ButtonPressed = Logger.IsCategoryEnabled(category)
                };

                cb.Toggled += (bool pressed) =>
                {
                    Logger.SetCategoryEnabled(category, pressed);
                };

                LogCheckboxes[category] = cb;
                logGroup.AddChild(cb);
            }
        }

        private void AddLevelButtons()
        {
            var dir = DirAccess.Open(LevelsFolderPath);
            if (dir == null)
            {
                Logger.Error(LogCategory.UI, $"Failed to open the directory: {LevelsFolderPath}", this);
                return;
            }

            dir.ListDirBegin();
            string fileName = dir.GetNext();
            var sceneFiles = new List<string>();
            while (fileName != "")
            {
                if (!dir.CurrentIsDir() && fileName.EndsWith(".tscn", StringComparison.OrdinalIgnoreCase))
                {
                    sceneFiles.Add(fileName);
                }
                fileName = dir.GetNext();
            }
            dir.ListDirEnd();

            sceneFiles.Sort();

            foreach (var sceneFile in sceneFiles)
            {
                var displayName = System.IO.Path.GetFileNameWithoutExtension(sceneFile);

                var btn = new Button
                {
                    Text = displayName
                };

                var scenePath = LevelsFolderPath + sceneFile;
                
                btn.Pressed += () =>
                {
                    GetTree().ChangeSceneToFile(scenePath);
                    GD.Print($"[DEBUG] Переход на уровень: {scenePath}");
                    HidePanel();
                };

                LevelsGroup.AddChild(btn);
            }
        }

        public void Toggle()
        {
            if (Visible) HidePanel();
            else ShowPanel();
        }

        public void ShowPanel()
        {
            Visible = true;
        }

        public void HidePanel()
        {
            Visible = false;
        }

        public override void _UnhandledInput(InputEvent @event)
        {
            if (!Visible) return;
            if (@event.IsActionPressed("ui_cancel"))
                HidePanel();
        }
    }
}
