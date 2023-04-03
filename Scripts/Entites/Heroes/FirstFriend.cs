using Godot;

namespace MathRPG.Entities.Heroes
{
    public class FirstFriend : Hero
    {
        [Signal]
        public delegate void MousePressed(); // Доп копия, т.к. в главная сцена не видит объекты этой сцены
    
        public override void _Ready()
        {
            InitializeVariables();
            MoveRadius = 9;
            Speed = 125;
        }

        private void OnInputEvent(Node viewport, InputEvent @event, int shapeIdx)
        {
            if (!(@event is InputEventMouseButton || @event is InputEventScreenTouch && @event.IsPressed()))
                return;
            
            EmitSignal(nameof(MousePressed));
        }

        public void SayHello(Vector2 forWhoPosition)
        {
            AnimatedSprite.FlipH = Position.x - forWhoPosition.x > 2;
            AnimatedSprite.Play("hello");
        }

        public void Childhood()
        {
            GetTree().ChangeScene("res://Scenes/LVLs/lvl0_childhood.tscn");
        }
    }
}