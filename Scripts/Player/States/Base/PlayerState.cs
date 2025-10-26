using System;
using Godot;

namespace LostWisps.Player
{
	public abstract class PlayerState
	{
		protected Player player;

		protected readonly String animationState = default;

		protected PlayerState(Player player, String animationState)
        {
			this.player = player;
			this.animationState = animationState;
        }

		public virtual void EnterState() { }
		public virtual void ExitState() { }
		public virtual void Update(double delta) { }
		public virtual void PhysicsUpdate(double delta) { }
		public virtual void Input(InputEvent @event) { }
		public virtual void HandleAnimations() { }
		public virtual void Draw() { }
	}
}
