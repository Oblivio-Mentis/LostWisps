#nullable enable

using Godot;
using System;

namespace LostWisps.Object
{
    public abstract partial class BaseSynchronizer : Node2D
    {
        [Export] public Node2D[] TargetNodes = Array.Empty<Node2D>();

        public override void _Ready()
        {
            base._Ready();
            ValidateTargetNodes();
        }

        private void ValidateTargetNodes()
        {
            foreach (var node in TargetNodes)
            {
                if (node == null)
                {
                    GD.PushWarning($"[{nameof(BaseSynchronizer)}] Найден null в массиве TargetNodes.");
                    continue;
                }

                if (node is not IActivatable activatable)
                {
                    string nodePath = node.GetPath();
                    GD.PushWarning($"[{nameof(BaseSynchronizer)}] Узел '{nodePath}' не реализует интерфейс {nameof(IActivatable)} и будет проигнорирован.");
                }
            }
        }

        protected void ApplyTransform(Func<Node2D, Transform2D> transformFunc)
        {
            foreach (var node in TargetNodes)
            {
                if (node != null)
                    node.Transform = transformFunc(node);
            }
        }

        protected Vector2 GetRelativeOffset(Node2D target, Vector2 basePosition)
        {
            return target.GlobalPosition - basePosition;
        }
    }
}