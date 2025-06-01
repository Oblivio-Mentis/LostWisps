using Godot;
using System;
using System.Collections.Generic;

namespace Object
{
    public partial class Button : Node2D
    {
        [Export] public Node2D[] TargetNodes2D { get; private set; }
        [Export] public float releaseDelay = 0f;
        private AnimationPlayer animationPlayer;
        private Timer timer;
        private IActivatable[] targets;
        private HashSet<Node2D> activeBodies = new HashSet<Node2D>();
        private bool isActivated = false;

        public override void _Ready()
        {
            animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
            timer = GetNode<Timer>("ReleaseDelayTimer");

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

            GD.Print($"Object entered the pressure plate: {body.Name}");

            activeBodies.Add(body);

            if (!isActivated)
            {
                isActivated = true;
                timer.Stop();
                animationPlayer.Play("toggle");

                foreach (var target in targets)
                {
                    target?.Activate();
                }
            }
        }

        private void OnBodyExited(Node2D body)
        {
            GD.Print($"Object exited the pressure plate: {body.Name}");
            activeBodies.Remove(body);

            if (activeBodies.Count == 0)
                if (releaseDelay == 0)
                {
                    TimerTimeout();
                }
                else
                {
                    timer.Start(releaseDelay);
                }
        }

        private void TimerTimeout()
        {
            isActivated = false;
            timer.Stop();
            animationPlayer.PlayBackwards("toggle");

            foreach (var target in targets)
            {
                target?.Deactivate();
            }
        }

        private bool CanInteract(Node2D body)
        {
            return body is CharacterBody2D || body is RigidBody2D;
        }
    }
}
