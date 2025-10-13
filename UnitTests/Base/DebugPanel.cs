using Godot;
using System;
using System.Collections.Generic;

namespace LostWisps.Debug
{
    public partial class DebugPanel : Control
    {
        private VBoxContainer LevelsGroup;

        private const string LevelsFolderPath = "res://UnitTests/UnitTestLevels/";

        public override void _Ready()
        {
            Visible = false;

            LevelsGroup = GetNode<VBoxContainer>("PanelContainer/MarginContainer/VBoxContainer/LevelsGroup");

            AddLevelButtons();
        }

        private void AddLevelButtons()
        {
            var dir = DirAccess.Open(LevelsFolderPath);
            if (dir == null)
            {
                GD.PushError($"[DebugPanel] Не удалось открыть директорию: {LevelsFolderPath}");
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

            if (sceneFiles.Count == 0)
            {
                var lbl = new Label { Text = "Нет доступных тестовых уровней!" };
                LevelsGroup.AddChild(lbl);
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
