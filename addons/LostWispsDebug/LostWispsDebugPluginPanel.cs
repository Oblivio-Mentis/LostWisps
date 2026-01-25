#nullable enable

using Godot;
using LostWisps.Debug;
using LostWisps.Global;
using System;

[Tool]
public partial class LostWispsDebugPluginPanel : PanelContainer
{
    public EditorInterface? EditorInterface { get; set; }
    private CheckBox? EnableAnimateInInspectorCheckBox;

    public override void _Ready() { }

    private void AddAnimateInInspectorCheckBox()
    {
        EnableAnimateInInspectorCheckBox = GetNodeOrNull<CheckBox>("TabContainer/Global/VBoxContainer/EnableAnimateInInspectorCheckBox");
        if (EnableAnimateInInspectorCheckBox == null)
        {
            GD.PushError("LostWispsDebugPluginPanel does not contain 'EnableAnimateInInspectorCheckBox'.");
            return;
        }

        EnableAnimateInInspectorCheckBox.Toggled += OnEnableAnimateInInspectorCheckBoxToggled;
    }

    private void AddLogCategoryCheckboxes()
    {
        var logGroup = GetNodeOrNull<Container>("TabContainer/Logs/VBoxContainer/LogCategoriesGroup");
        if (logGroup == null)
        {
            GD.PushWarning("LogCategoriesGroup container not found. Log checkboxes won't appear.");
            return;
        }

        var projectSettings = ProjectSettings.Singleton;
        string baseKey = GlobalConstants.DebugSettings.SETTING_LOG_BASE_KEY;

        foreach (LogCategory category in Enum.GetValues<LogCategory>())
        {
            string key = baseKey + category.ToString().ToLower();

            if (!projectSettings.HasSetting(key))
            {
                projectSettings.SetSetting(key, true);
                projectSettings.AddPropertyInfo(new Godot.Collections.Dictionary
                {
                    ["name"] = key,
                    ["type"] = (int)Variant.Type.Bool,
                    ["hint"] = (int)PropertyHint.None
                });
            }

            bool isEnabled = projectSettings.GetSetting(key).As<bool>();

            var checkBox = new CheckBox
            {
                Text = $"Log: {category}",
                ButtonPressed = isEnabled,
            };

            checkBox.Toggled += (bool pressed) => OnEnableLogCategoryCheckBoxToggled(pressed, category.ToString());

            logGroup.AddChild(checkBox);
        }
    }

    private void InitializeUI()
    {
        if (!Engine.IsEditorHint() || EditorInterface == null) 
            return;

        AddAnimateInInspectorCheckBox();
        AddLogCategoryCheckboxes();
    }

    public void Initialize(EditorInterface editorInterface)
    {
        EditorInterface = editorInterface;

        InitializeUI();

        if (EnableAnimateInInspectorCheckBox != null)
        {
            var editorSettings = EditorInterface.GetEditorSettings();
            string key = GlobalConstants.DebugSettings.SETTING_ENABLE_ANIMATE_KEY;

            if (!editorSettings.HasSetting(key))
            {
                editorSettings.SetSetting(key, false);
            }

            EnableAnimateInInspectorCheckBox.ButtonPressed = editorSettings.GetSetting(key).As<bool>();
        }
    }

    private void OnEnableLogCategoryCheckBoxToggled(bool pressed, string categoryNameStr)
    {
        if (!Enum.TryParse<LogCategory>(categoryNameStr, out var category))
            return;

        string key = GlobalConstants.DebugSettings.SETTING_LOG_BASE_KEY + categoryNameStr.ToLower();
        ProjectSettings.Singleton.SetSetting(key, pressed);
        Logger.SetCategoryEnabled(category, pressed);
        ProjectSettings.Singleton.Save();
    }

    private void OnEnableAnimateInInspectorCheckBoxToggled(bool pressed)
    {
        SetEditorSetting(GlobalConstants.DebugSettings.SETTING_ENABLE_ANIMATE_KEY, pressed);
    }

    private void SetEditorSetting(string key, bool value)
    {
        if (EditorInterface == null) return;

        var editorSettings = EditorInterface.GetEditorSettings();
        editorSettings.SetSetting(key, value);
    }
}