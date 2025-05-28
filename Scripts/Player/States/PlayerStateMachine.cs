// using Godot;

// namespace Player
// {
//     public partial class PlayerStateMachine : CharacterBody2D
//     {
//         [Export] public Sprite2D Sprite;
//         [Export] public AnimationPlayer AnimPlayer;

//         private PlayerState _currentState;

//         // Состояния
//         public PlayerState IdleState { get; private set; }
//         public PlayerState RunState { get; private set; }
//         public PlayerState JumpState { get; private set; }
//         public PlayerState FallState { get; private set; }
//         public PlayerState JumpPeakState { get; private set; }

//         public override void _Ready()
//         {
//             // Инициализация состояний
//             IdleState = new IdleState();
//             RunState = new RunState();
//             JumpState = new JumpState();
//             FallState = new FallState();
//             JumpPeakState = new JumpPeakState();

//             foreach (var state in new[] { IdleState, RunState, JumpState, FallState, JumpPeakState })
//             {
//                 AddChild(state);
//             }

//             SwitchState(IdleState);
//         }

//         public override void _Process(double delta)
//         {
//             _currentState.Update(delta);
//         }

//         public override void _PhysicsProcess(double delta)
//         {
//             _currentState.PhysicsUpdate(delta);
//         }

//         public override void _Input(InputEvent @event)
//         {
//             _currentState.Input(@event);
//         }

//         public void SwitchState(PlayerState newState)
//         {
//             _currentState.ExitState();
//             _currentState = newState;
//             _currentState.EnterState();
//         }
//     }
// }