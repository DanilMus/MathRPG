using Godot;

namespace MathRPG.Entities.Heroes
{
    public class FirstFriend : Entity
    {
        [Signal]
        public delegate void MousePressed(); // Доп копия, т.к. в главная сцена не видит объекты этой сцены
    
        public override void _Ready()
        {
            InitializeVariables();
        }

        private void OnInputEvent(Node viewport, InputEvent @event, int shapeIdx)
        {
            if (!(@event is InputEventMouseButton || @event is InputEventScreenTouch && @event.IsPressed()))
                return;
            
            EmitSignal(nameof(MousePressed));
        }

        public void SayHello(Vector2 forWhoPosition)
        {
            AnimatedSprite.Play("hello");
            AnimatedSprite.FlipH = Position.x - forWhoPosition.x > 2;
        }
    }
}