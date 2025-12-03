using Godot;

namespace LostWisps.Object
{
    public interface IValueReceiver
    {
        public virtual void SetValue(float value) { }
        public virtual void SetInstantValue(float value) => SetValue(value);
    }
}