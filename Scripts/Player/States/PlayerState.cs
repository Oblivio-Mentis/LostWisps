using Godot;

namespace Player
{
	public abstract class PlayerState
	{
		protected Player player;

		protected PlayerState(Player player) => this.player = player;

		public virtual void EnterState() { }
		public virtual void ExitState() { }
		public virtual void Update(double delta) { }
		public virtual void PhysicsUpdate(double delta) { }
		public virtual void Input(InputEvent @event) { }
		public virtual void HandleAnimations() { }
		public virtual void Draw() { }
	}
}
