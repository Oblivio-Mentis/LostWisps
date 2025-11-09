using Godot;
using LostWisps.Debug;
using System.Collections.Generic;

namespace LostWisps.Global.Destruction.Triggers
{
    [GlobalClass]
    public partial class DamageOverTimeTrigger : Area2D, IDestructionStrategy
    {
        [Export] private float interval = 1.0f;
        [Export] private int damageAmount = 1;

        private HashSet<Node> bodiesInside = new HashSet<Node>();
        private Timer nextDamageTimer;

<<<<<<< Updated upstream
        public override void _Ready()
        {
            BodyEntered += OnBodyEntered;
=======
		public override void _Ready()
		{
			BodyEntered += OnBodyEntered;
			BodyExited += OnBodyExited;
>>>>>>> Stashed changes

            nextDamageTimer = new Timer();
            nextDamageTimer.OneShot = true;
            nextDamageTimer.WaitTime = interval;
            nextDamageTimer.Timeout += OnDamageTimerCompleted;
            AddChild(nextDamageTimer);
        }

<<<<<<< Updated upstream
        private void OnBodyEntered(Node body)
        {
            if ((body is CharacterBody2D || body is RigidBody2D) && !bodiesInside.Contains(body))
            {
                bodiesInside.Add(body);
                Activate();
                StartNextDamageTimer();
            }
        }

        private void StartNextDamageTimer()
        {
            if (nextDamageTimer.IsStopped())
            {
                nextDamageTimer.Start();
            }
        }

        private void OnDamageTimerCompleted()
        {
            if (bodiesInside.Count > 0)
            {
                Activate();
                StartNextDamageTimer(); // Перезапускаем таймер
            }
        }

        public void Activate()
        {
            var system = GetParent<DestructionSystem>();
            system?.TakeDamage(damageAmount);
        }
    }
=======
		private void OnBodyEntered(Node body)
		{
			if ((body is CharacterBody2D || body is RigidBody2D) && !bodiesInside.Contains(body))
			{
				bodiesInside.Add(body);
				if (nextDamageTimer.IsStopped())
				{
					nextDamageTimer.Start();
				}
			}
		}

		private void OnBodyExited(Node body)
		{
			if (bodiesInside.Remove(body) && bodiesInside.Count == 0)
			{
				nextDamageTimer.Stop();
			}
		}

		private void OnDamageTimerCompleted()
		{
			if (bodiesInside.Count > 0)
			{
				Activate();
				nextDamageTimer.Start();
			}
		}

		public void Activate()
		{
			var system = GetParent<DestructionSystem>();
			system?.TakeDamage(damageAmount);

			Logger.Log(LogCategory.Destruction, $"Damage taken {damageAmount}. Current durability {system.currentDurability + 1}");
		}
	}
>>>>>>> Stashed changes
}