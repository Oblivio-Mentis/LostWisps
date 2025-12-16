#nullable enable

using Godot;
using System.Collections.Generic;

namespace LostWisps.Object
{
    [GlobalClass]
    public partial class ContactWatcher : Area2D
    {
        [Signal] public delegate void OnInteractiveBodyEnteredEventHandler();
        [Signal] public delegate void OnInteractiveBodyExitedEventHandler();
        [Signal] public delegate void OnAllInteractiveBodiesExitedEventHandler();

        private readonly HashSet<Node2D> activeBodies = new();
        
        public override void _Ready()
        {
            BodyEntered += OnBodyEntered;
            BodyExited += OnBodyExited;
        }

        public void OnBodyEntered(Node2D body)
        {
            if (Utils.Utils.ObjectCanInteract(body))
            {
                activeBodies.Add(body);
                EmitSignal(SignalName.OnInteractiveBodyEntered);
            }
        }

        public void OnBodyExited(Node2D body)
        {
            activeBodies.Remove(body);
            EmitSignal(SignalName.OnInteractiveBodyExited);

            if (!HasActiveBodies())
                EmitSignal(SignalName.OnAllInteractiveBodiesExited);
        }

        public bool HasActiveBodies() => activeBodies.Count > 0;
    }
}
