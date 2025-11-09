using Godot;
using System;
using System.Collections.Generic;

namespace LostWisps.Global.Destruction
{
    public enum DestructionType
    {
        Damage,
        Contact,
        Timer,
        Event
    }

    public class DestructionEventArgs : EventArgs
    {
        public Node Target { get; set; }
        public DestructionType Type { get; set; }
    }

    public partial class DestructionController : Node
    {
        private List<DestructionSystem> systems = new List<DestructionSystem>();

        public void TriggerDestruction(DestructionSystem system, DestructionType type = DestructionType.Damage)
        {
            var parent = system.GetParent();
            parent?.QueueFree();
            OnDestructionOccurred(new DestructionEventArgs { Target = system, Type = type });
        }

        public event EventHandler<DestructionEventArgs> DestructionOccurred;

        protected virtual void OnDestructionOccurred(DestructionEventArgs e)
        {
            DestructionOccurred?.Invoke(this, e);
        }
    }
}