using Godot;
using System;

// namespace LostWisps.Object
// {
//     public partial class ActivatedPlatform : Path2D, IActivatable
//     {
//         [Export] public float speed = 1.0f;
//         [Export] public bool IsDeactivatable = true;
//         public bool isActivated = false;
//         private PathFollow2D pathFollow2D;
//         private AnimationPlayer animationPlayer;

//         public override void _Ready()
//         {
//             pathFollow2D = GetNode<PathFollow2D>("PathFollow2D");
//             animationPlayer = GetNode<AnimationPlayer>("PathFollow2D/AnimationPlayer");
//             animationPlayer.SpeedScale = speed;
//         }

//         public void Activate()
//         {
//             if (isActivated)
//                 return;

//             isActivated = true;
//             animationPlayer.Play("move");
//         }

//         public void Deactivate()
//         {
//             if (!IsDeactivatable)
//                 return;

//             isActivated = false;
//             animationPlayer.PlayBackwards("move");
//         }
//     }
// }
