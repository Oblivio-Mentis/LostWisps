using Godot;

namespace LostWisps.Global.Destruction
{
    [GlobalClass]
    public partial class DestructionStage : Resource
    {
        [Export] public int Threshold { get; set; } = 0;
        [Export] public Texture2D Texture { get; set; }
        [Export] public AudioStream SoundEffect { get; set; }
        [Export] public PackedScene EffectScene { get; set; }

        public void Apply(Node target)
        {
            GD.Print(target is Sprite2D && Texture != null);
            if (target is Sprite2D spriteNode && Texture != null)
            {
                spriteNode.Texture = Texture;
            }

            if (SoundEffect != null && target.GetParent() is Node parentNode)
            {
                var soundPlayer = new AudioStreamPlayer();
                soundPlayer.Stream = SoundEffect;
                parentNode.AddChild(soundPlayer);
                soundPlayer.Play();
            }

            if (EffectScene != null && target is Node2D node2D && target.GetParent() is Node sceneParent)
            {
                var effect = EffectScene.Instantiate();
                sceneParent.AddChild(effect);

                if (effect is Node2D effectAsNode2D)
                {
                    effectAsNode2D.GlobalPosition = node2D.GlobalPosition;
                }
            }
        }
    }
}