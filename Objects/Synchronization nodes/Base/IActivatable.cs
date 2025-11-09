using Godot;
using System;

namespace LostWisps.Object
{
    public interface IActivatable
    {
        bool IsActivated { get; }

        public virtual void Activate() { }
        public virtual void Deactivate() { }
        public virtual void SetIsActivated(bool isActivated) { }
    }
}
