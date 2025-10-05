using Godot;

namespace LostWisps.Global.Destruction.Triggers
{
    [GlobalClass]
    public partial class DamageTrigger : Node, IDestructionStrategy
    {
        [Export] private int damageThreshold = 1;
        private int currentDamage = 0;

        public void ApplyDamage(int amount)
        {
            currentDamage += amount;
            if (currentDamage >= damageThreshold)
            {
                Activate();
            }
        }

        public override void _Ready()
        {
        }

        public void Activate()
        {
            DestructionManager.Instance.TriggerDestruction(GetParent<DestructionSystem>());
        }
    }
}