using Godot;
using System.Collections.Generic;

namespace LostWisps.Object
{
    public partial class Button : Node2D
    {
        [Export] public Node2D[] TargetNodes { get; private set; }
        private AnimationPlayer animationPlayer;
        private IActivatable[] targets;
        private HashSet<Node2D> activeBodies = new HashSet<Node2D>();
        private bool isActivated = false;

        public override void _Ready()
        {
            animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

            if (TargetNodes == null)
                return;

            var list = new List<IActivatable>();

            foreach (var node in TargetNodes)
            {
                if (node is IActivatable activatable)
                {
                    list.Add(activatable);
                }
                else
                {
                    GD.PushWarning($"Node at path {node} does not implement IActivatable.");
                }
            }

            targets = list.ToArray();
        }

        private void OnBodyEntered(Node2D body)
        {
            if (!CanInteract(body))
                return;

            activeBodies.Add(body);

            if (!isActivated)
            {
                isActivated = true;
                animationPlayer.Play("Toggle");

                foreach (var target in targets)
                {
                    if (target == null)
                        continue;

                    if (target.IsActivated)
                        target.Deactivate();
                    else
                        target.Activate();
                }
            }
        }

        private void OnBodyExited(Node2D body)
        {
            activeBodies.Remove(body);

            if (activeBodies.Count == 0)
            {
                isActivated = false;
                animationPlayer.PlayBackwards("Toggle");
            }
        }

        private bool CanInteract(Node2D body)
        {
            return body is CharacterBody2D || body is RigidBody2D;
        }
    }
}

