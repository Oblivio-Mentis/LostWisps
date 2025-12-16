#nullable enable

using Godot;
using LostWisps.Debug;
using System;

namespace LostWisps.Object
{
    [Tool]
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
            if (TargetNodes == null || TargetNodes.Length == 0)
            {
                Logger.Warn(LogCategory.Interaction, "TargetNodes is not assigned.");
            }

            foreach (var node in TargetNodes)
            {
                if (node == null)
                {
                    Logger.Warn(LogCategory.Synchronizer, "Found null in the TargetNodes array.", this);
                    continue;
                }

                if (node is not IActivatable activatable)
                {
                    string nodePath = node.GetPath();
                    Logger.Warn(LogCategory.Synchronizer, $"The '{nodePath}' node does not implement the {nameof(IActivatable)} interface and will be ignored.", this);
                }
            }
        }

        protected void ActivateTargetNodes()
        {
            if (TargetNodes == null) return;

            foreach (var node in TargetNodes)
            {
                if (node is IActivatable activatable)
                {
                    activatable.Activate();
                }
            }
        }

        protected void DeactivateTargetNodes()
        {
            if (TargetNodes == null) return;

            foreach (var node in TargetNodes)
            {
                if (node is IActivatable activatable)
                {
                    activatable.Deactivate();
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

#if TOOLS
        public void DrawTargetNodes()
        {
            if (!Engine.IsEditorHint()) return;

            if (TargetNodes == null) return;

            Vector2 origin = GlobalPosition;
            foreach (var targetNode in TargetNodes)
            {
                if (targetNode == null) continue;

                Vector2 targetPosition = targetNode.GlobalPosition;
                Vector2 localTarget = ToLocal(targetPosition);

                DrawLine(Vector2.Zero, localTarget, new Color(0.2f, 0.8f, 1.0f, 0.6f), 2.0f);

                DrawCircle(localTarget, 8, new Color(0.2f, 0.8f, 1.0f, 0.8f));
            }
        }
#endif
    }
}