using Godot;
using System;
using System.Collections.Generic;

namespace LostWisps.Object
{
    public partial class Lever : Node2D
    {
        [Export] public Node2D[] TargetNodes2D { get; private set; }

        private AnimationPlayer animationPlayer;
        private IActivatable[] targets;
        private HashSet<Node2D> activeBodies = new HashSet<Node2D>();
        private bool isActivated = false;

        public override void _Ready()
        {
            animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

            if (TargetNodes2D == null)
                return;

            var list = new List<IActivatable>();

            foreach (var node in TargetNodes2D)
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

            isActivated = !isActivated;
            
            if (isActivated)
                animationPlayer.Play("toggle");
            else
                animationPlayer.PlayBackwards("toggle");

            foreach (var target in targets)
            {
                if (isActivated)
                {
                    target?.Activate();
                }
                else
                {
                    target?.Deactivate();
                }
            }
        }

        private bool CanInteract(Node2D body)
        {
            return body is CharacterBody2D;
        }
    }
}
