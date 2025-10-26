using Godot;
using System.Collections.Generic;

namespace LostWisps.Global.Destruction
{
	[GlobalClass]
	public partial class DestructionSystem : Node2D
	{
		[Export] public int Durability = 1;
		[Export] public DestructionStage[] Stages = new DestructionStage[0];
		[Export] public float DestroyDelay = 1f;

		public IDestructionStrategy[] Triggers = new IDestructionStrategy[0];
		private int currentDurability;
		private int lastStageIndex = 0;
		private Timer destroyTimer;

		public override void _Ready()
		{
			currentDurability = Durability;

			List<IDestructionStrategy> foundTriggers = new List<IDestructionStrategy>();

			foreach (Node child in GetChildren())
			{
				if (child is IDestructionStrategy strategy)
				{
					foundTriggers.Add(strategy);
				}
			}

			Triggers = foundTriggers.ToArray();

			foreach (var trigger in Triggers)
			{
				Node node = trigger as Node;
				if (node.GetParent() == null)
				{
					AddChild(node);
					node.Owner = this;
				}
			}

			destroyTimer = new Timer();
			AddChild(destroyTimer);
			destroyTimer.OneShot = true;
			destroyTimer.Timeout += OnDestroyTriggered;
		}

		public void TakeDamage(int amount)
		{
			currentDurability -= amount;

			CheckStages();

			if (currentDurability <= 0 && destroyTimer.IsStopped())
			{
				destroyTimer.Start(DestroyDelay);
			}
		}

		public void OnHit() => TakeDamage(1);

		public void Destruct() => destroyTimer.Start(DestroyDelay);

		private void CheckStages()
		{
			for (int i = lastStageIndex; i < Stages.Length; i++)
			{
				if (currentDurability <= Stages[i].Threshold)
				{
					Stages[i].Apply(this);
					lastStageIndex = i + 1;
				}
			}
		}

		private void OnDestroyTriggered()
		{
			DestructionManager.Instance.TriggerDestruction(this);
		}
	}
}
