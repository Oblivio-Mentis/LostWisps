using Godot;
using System;

namespace Object
{
    public partial class PressurePlate : Node2D
    {
        [Export] public Node Target { get; set; }

        public override void _Ready()
        {
        }

        private void OnBodyEntered(Node2D body)
        {
            GD.Print($"Object entered the pressure plate: {body.Name}");
            if (CanInteract(body) && Target is IActivatable activatable)
            {
                activatable.Activate();
            }
        }

        private void OnBodyExited(Node2D body)
        {
            GD.Print($"Object exited the pressure plate: {body.Name}");
            if (CanInteract(body) && Target is IActivatable activatable)
            {
                activatable.Deactivate();
            }
        }

        private bool CanInteract(Node2D body)
        {
            return body is CharacterBody2D || body is RigidBody2D; // Можно расширить
        }
    }
}
