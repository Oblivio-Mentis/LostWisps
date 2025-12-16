#nullable enable

using Godot;

namespace LostWisps.Object
{
    [Tool]
    public partial class ButtonTimer : BaseSynchronizer
    {
        [Export] public float ReleaseDelay = 0f;

        [Export] public bool Inverse = false;

        private AnimationPlayer? animationPlayer;
        private Timer? timer;

        private bool isActivated = false;

        public override void _Ready()
        {
            base._Ready();

            animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
            timer = GetNode<Timer>("ReleaseDelayTimer");

            if (timer != null)
            {
                timer.OneShot = true;
                timer.Timeout += TimerTimeout;
            }
        }

        private void Activate()
        {
            if (isActivated) return;

            isActivated = true;
            timer?.Stop();

            ActivateTargetNodes();
        }

        private void Deactivate()
        {
            if (!isActivated) return;

            isActivated = false;
            timer?.Stop();

            DeactivateTargetNodes();
        }

        public void OnInteractiveBodyEntered()
        {
            animationPlayer?.Play("Toggle");
            if (Inverse) Deactivate(); else Activate();
        }

        public void OnAllInteractiveBodiesExited()
        {
            if (ReleaseDelay <= 0f)
            {
                animationPlayer?.PlayBackwards("Toggle");
                if (Inverse) Activate(); else Deactivate();
            }
            else
            {
                timer?.Start(ReleaseDelay);
            }
        }

        private void TimerTimeout()
        {
            animationPlayer?.PlayBackwards("Toggle");
            if (Inverse) Activate(); else Deactivate();
        }
    }
}