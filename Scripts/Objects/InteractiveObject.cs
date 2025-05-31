using Godot;
using System;

namespace Object
{
    public abstract partial class InteractiveObject : Area2D
    {
        public override void _Ready()
        {
            base._Ready();
        }

        private void OnBodyEntered(Node2D body)
        {
            if (CanInteract(body))
                OnInteract(true);
        }

        private void OnBodyExited(Node2D body)
        {
            if (CanInteract(body))
                OnInteract(false);
        }

        protected virtual bool CanInteract(Node2D body)
        {
            return body is not null;
        }

        protected abstract void OnInteract(bool isActive);
    }
}
