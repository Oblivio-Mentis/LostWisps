#nullable enable

using Godot;
using System;
using System.Collections.Generic;

namespace LostWisps.Object
{
    public partial class ButtonTimer : BaseSynchronizer
    {
        [Export] public float ReleaseDelay = 0f;

        private AnimationPlayer? animationPlayer;
        private Timer? timer;

        private HashSet<Node2D> activeBodies = new HashSet<Node2D>();
        private bool isActivated = false;

        public override void _Ready()
        {
            base._Ready();

            animationPlayer ??= GetNode<AnimationPlayer>("AnimationPlayer");

            timer ??= GetNode<Timer>("ReleaseDelayTimer");

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
                timer?.Stop();
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
                if (ReleaseDelay <= 0)
                {
                    TimerTimeout();
                }
                else
                {
                    timer?.Start(ReleaseDelay);
                }
            }
        }

        private void TimerTimeout()
        {
            isActivated = false;
            timer?.Stop();
            animationPlayer?.PlayBackwards("Toggle");

            foreach (var node in TargetNodes)
            {
                if (node is IActivatable activatable)
                {
                    activatable.Deactivate();
                }
            }
        }

        private bool CanInteract(Node2D body)
        {
            return body is CharacterBody2D || body is RigidBody2D;
        }
    }
}