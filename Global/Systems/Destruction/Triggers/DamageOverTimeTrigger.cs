using Godot;
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

		public override void _Ready()
		{
			BodyEntered += OnBodyEntered;

			nextDamageTimer = new Timer();
			nextDamageTimer.OneShot = true;
			nextDamageTimer.WaitTime = interval;
			nextDamageTimer.Timeout += OnDamageTimerCompleted;
			AddChild(nextDamageTimer);
		}

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
}
