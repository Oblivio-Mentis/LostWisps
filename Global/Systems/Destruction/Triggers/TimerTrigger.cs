using Godot;

namespace LostWisps.Global.Destruction.Triggers
{
    [GlobalClass]
    public partial class TimerTrigger : Node, IDestructionStrategy
    {
        [Export] private float lifeTime = 5f;

        public override void _Ready()
        {
            var timer = new Timer();
            AddChild(timer);
            timer.WaitTime = lifeTime;
            timer.Timeout += Activate;
            timer.Start();
        }

        public void Activate()
        {
            DestructionManager.Instance.TriggerDestruction(GetParent<DestructionSystem>());
        }
    }
}