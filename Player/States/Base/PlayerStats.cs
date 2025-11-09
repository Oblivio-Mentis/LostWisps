using Godot;
using System;

namespace LostWisps.Player
{
    [GlobalClass]
    public partial class PlayerStats : Resource
    {
        [Export] public float FallGravity = 1500f;
        [Export] public float FallVelocity = 500f;
        [Export] public float WalkVelocity = 200f;
        [Export] public float JumpVelocity = -600f;
        [Export] public float JumpDeceleration = 1500f;

    }
}
