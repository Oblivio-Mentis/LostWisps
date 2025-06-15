using Godot;

namespace LostWisps.Global.Destruction.Triggers
{
    [GlobalClass]
    public partial class DamageOnContactTrigger : Area2D, IDestructionStrategy
    {
        public override void _Ready()
        {
            BodyEntered += OnBodyEntered;
        }

        private void OnBodyEntered(Node body)
        {
            if (body is CharacterBody2D || body is RigidBody2D)
            {
                Activate();
            }
        }

        public void Activate()
        {
            var system = GetParent<DestructionSystem>();
            if (system != null)
            {
                system.OnHit();
            }
        }
    }
}