using Godot;
using System;

[GlobalClass]
public partial class LiquidArea : Area2D
{
    [Export] private Area2D area2D;
    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
        BodyExited += OnBodyExited;
    }

    private void OnBodyEntered(Node body)
    {
        if (body is CharacterBody2D characterBody2D)
        {
            OnPlayerDied(characterBody2D);
        }
    }

    private void OnBodyExited(Node body)
    {
        if (body is CharacterBody2D)
        {
        }
    }

    protected virtual void OnPlayerDied(CharacterBody2D characterBody2D)
    {
        // characterBody2D.QueueFree(); 
    }
}