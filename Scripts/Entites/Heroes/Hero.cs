using Godot;

namespace MathRPG.Entities.Heroes
{
    public abstract class Hero: Entity
    {
        protected void Death()
        {
            Hide();
            // Отключение столкновения игрока может привести к
            // ошибке, поэтому мы используем SetDegerred. Он говорит Годот ждать
            // отключения, пока не будет безопасно.
            GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred("disabled", true);
        }
    }
}