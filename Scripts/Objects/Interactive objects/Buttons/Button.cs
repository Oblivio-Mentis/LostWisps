#nullable enable

using Godot;

namespace LostWisps.Object
{
    [Tool]
    public partial class Button : BaseSynchronizer
    {
        // === Экспортируемые параметры ===

        [Export] bool OneShot { get; set; } = false;

        // === Приватные поля ===

        private AnimationPlayer? animationPlayer;
        private bool isActivated = false;

        // === Жизненный цикл ===

        public override void _Ready()
        {
            base._Ready();
            animationPlayer ??= GetNode<AnimationPlayer>("AnimationPlayer");
        }

        // === Внутренняя логика активации ===
        
        private void Activate()
        {
            if (isActivated) return;

            isActivated = true;

            ActivateTargetNodes();
        }

        private void OnInteractiveBodyEntered()
        {
            animationPlayer?.Play("Toggle");
            Activate();
        }

        // === События коллайдера ===

        private void OnAllInteractiveBodiesExited()
        {
            if (!OneShot)
            {
                isActivated = false;
                animationPlayer?.PlayBackwards("Toggle");
            }
        }
    }
}