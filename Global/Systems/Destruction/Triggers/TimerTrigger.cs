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
            var DestructionSystem = GetParent<DestructionSystem>();
            if (DestructionSystem != null)
            {
                var DestructionController = GetNode<DestructionController>("/root/DestructionController");
                DestructionController.TriggerDestruction(GetParent<DestructionSystem>());
            }
        }
    }
}