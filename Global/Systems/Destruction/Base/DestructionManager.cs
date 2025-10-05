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

    [GlobalClass]
    public partial class DestructionManager : Node
    {
        private static DestructionManager instance;

        public static DestructionManager Instance => instance;

        private List<DestructionSystem> systems = new List<DestructionSystem>();

        public override void _EnterTree()
        {
            if (instance == null)
            {
                instance = this;
                AddToGroup("singleton");
            }
            else
            {
                QueueFree();
            }
        }

        public void Register(DestructionSystem system)
        {
            systems.Add(system);
        }

        public void TriggerDestruction(DestructionSystem system, DestructionType type = DestructionType.Damage)
        {
            var parent = system.GetParent();
            if (parent != null)
            {
                parent.QueueFree();
            }

            OnDestructionOccurred(new DestructionEventArgs { Target = system, Type = type });
        }

        public event EventHandler<DestructionEventArgs> DestructionOccurred;

        protected virtual void OnDestructionOccurred(DestructionEventArgs e)
        {
            DestructionOccurred?.Invoke(this, e);
        }
    }
}