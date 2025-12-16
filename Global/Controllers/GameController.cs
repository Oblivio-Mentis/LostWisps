using Godot;
using System;

namespace LostWisps.Global
{
    public partial class GameController : Node
    {
        public override void _Ready()
        {
            Debug.Logger.InitializeFromProjectSettings();
        }
    }
}
