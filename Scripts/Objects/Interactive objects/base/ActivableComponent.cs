using Godot;

namespace LostWisps.Object
{
    public class ActivatableComponent : IActivatable
    {
        public bool IsActivated { get; private set; }

        public ActivatableComponent(bool isActivated = false)
        {
            IsActivated = isActivated;
        }

        public virtual void Activate()
        {
            IsActivated = true;
        }

        public virtual void Deactivate()
        {
            IsActivated = false;
        }
    }
}