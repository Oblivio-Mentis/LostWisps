// File: Global/Systems/Animation/AnimationPriorityManager.cs

using Godot;
using System.Collections.Generic;

using LostWisps.Object;

namespace LostWisps.Global.Animation
{
    public partial class AnimationPriorityManager : Node
    {
        // Singleton instance
        public static AnimationPriorityManager Instance { get; private set; }

        // Храним активные анимации по: объект -> тип -> синхронизатор
        private readonly Dictionary<Node, Dictionary<System.Type, object>> ActiveAnimations = new();

        public override void _EnterTree()
        {
            base._EnterTree();
            // Проверяем, есть ли уже экземпляр
            if (Instance != null && Instance != this)
            {
                QueueFree(); // Удаляем дубликат
                return;
            }

            Instance = this;
            Name = "AnimationPriorityManager";
        }

        public override void _Ready()
        {
            GD.Print("✅ AnimationPriorityManager успешно загружен");
        }

        public override void _ExitTree()
        {
            base._ExitTree();
            if (Instance == this)
                Instance = null;
        }

        /// <summary>
        /// Активировать синхронизатор, остановив предыдущий того же типа на тех же объектах.
        /// </summary>
        public static void RequestActivation<T>(Synchronizer<T> synchronizer) where T : struct
        {
            if (Instance == null)
            {
                GD.PrintErr("Instance is null!");
                return;
            }

            GD.Print("🌀 Начинаем RequestActivation");

            var syncType = synchronizer.GetType();
            GD.Print($"🌀 Тип синхронизатора: {syncType}");

            foreach (var targetNode in synchronizer.TargetNodes)
            {
                GD.Print($"🌀 Обрабатываем объект: {targetNode?.Name ?? "null"}");

                if (targetNode == null)
                {
                    GD.PrintErr("❌ TargetNode == null");
                    continue;
                }

                if (!Instance.ActiveAnimations.TryGetValue(targetNode, out var activeSyncs))
                {
                    GD.Print($"🌀 Создаём словарь для {targetNode.Name}");
                    activeSyncs = new Dictionary<System.Type, object>();
                    Instance.ActiveAnimations[targetNode] = activeSyncs;
                }

                if (activeSyncs.TryGetValue(syncType, out var oldSyncObj))
                {
                    GD.Print($"🌀 Деактивируем старый синхронизатор: {oldSyncObj.GetType()}");
                    if (oldSyncObj is Synchronizer<T> oldSync)
                    {
                        oldSync.Deactivate();
                    }
                }

                GD.Print($"🌀 Активируем новый синхронизатор: {syncType}");
                synchronizer.Activate(); // <-- Возможно здесь ошибка
                activeSyncs[syncType] = synchronizer;
            }
        }

        /// <summary>
        /// Очистить все активные анимации (например, при смене уровня).
        /// </summary>
        public static void Clear()
        {
            if (Instance == null)
                return;

            foreach (var entry in Instance.ActiveAnimations)
            {
                foreach (var syncObj in entry.Value.Values)
                {
                    if (syncObj is IActivatable sync)
                    {
                        sync.Deactivate();
                    }
                }
            }

            Instance.ActiveAnimations.Clear();
        }
    }
}