#nullable enable

using Godot;
using System;
using System.Collections.Generic;

namespace LostWisps.Object
{
    public partial class Button : BaseSynchronizer
    {
        private AnimationPlayer? animationPlayer;
        private HashSet<Node2D> activeBodies = new HashSet<Node2D>();
        private bool isActivated = false;

        public override void _Ready()
        {
            base._Ready();

            animationPlayer ??= GetNode<AnimationPlayer>("AnimationPlayer");

            if (TargetNodes == null || TargetNodes.Length == 0)
                return;
        }

        private void OnBodyEntered(Node2D body)
        {
            if (!CanInteract(body))
                return;

            activeBodies.Add(body);

            if (!isActivated)
            {
                isActivated = true;
                animationPlayer?.Play("Toggle");
                
                foreach (var node in TargetNodes)
                {
                    if (node is IActivatable activatable)
                    {
                        activatable.Activate();
                    }
                }
            }
        }

        private void OnBodyExited(Node2D body)
        {
            activeBodies.Remove(body);

            if (activeBodies.Count == 0)
            {
                isActivated = false;
                animationPlayer?.PlayBackwards("Toggle");
            }
        }

        private bool CanInteract(Node2D body)
        {
            return body is CharacterBody2D || body is RigidBody2D;
        }
    }
}