#nullable enable

using Godot;
using System;
using System.Collections.Generic;

namespace LostWisps.Object
{
    public partial class Lever : BaseSynchronizer
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

            isActivated = !isActivated;

            if (animationPlayer != null)
            {
                if (isActivated)
                    animationPlayer.Play("toggle");
                else
                    animationPlayer.PlayBackwards("toggle");
            }

            foreach (var node in TargetNodes)
            {
                if (node is IActivatable activatable)
                {
                    if (isActivated)
                        activatable.Activate();
                    else
                        activatable.Deactivate();
                }
            }
        }

        private bool CanInteract(Node2D body)
        {
            return body is CharacterBody2D;
        }
    }
}