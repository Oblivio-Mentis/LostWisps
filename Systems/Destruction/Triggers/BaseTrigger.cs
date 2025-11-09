using Godot;

namespace LostWisps.Global.Destruction
{
    [GlobalClass]
    public abstract partial class BaseTrigger : Node, IDestructionStrategy
    {
        public abstract void Activate();
    }
}