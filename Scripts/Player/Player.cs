using MathRPG.Scripts.BaseClasses;

namespace MathRPG.Scripts.Player
{
    public class Player: Entity
    {
        public override void _Ready()
        {
            InitializeVariables();
        }

        protected override void Move(float delta)
        {
            // Включаем анимацию
            AnimatedSprite.Play("walk");
            AnimatedSprite.FlipH = Position.x - Path[Path.Count - 1].x > 2;

            base.Move(delta);
        }
    }
}