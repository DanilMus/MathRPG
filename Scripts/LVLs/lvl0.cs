using Godot;
using MathRPG.Entities.Heroes;

namespace MathRPG.LVL
{
    public class lvl0 : Main
    {
        FirstFriend friend; // для друга
        

        public override void _Ready() // Первая инициализация сцены
        {
            base.InitializeVariables();

            friend = entities.GetNode<FirstFriend>("FirstFriend");
            friend.Position = pathFinder.GetClosestPosition(friend.Position); // Прикрепление позиции игрока к сетке
            
            // friend = GetNode<AnimatedSprite>("FirstFriend");
            // friend.Position = pathFinder.GetClosestPosition(friend.Position);
            // friend.Connect("MousePressed", this, nameof(PlayScene));
        }

        public override void _Input(InputEvent @event)
        {
            base._Input(@event);
        }
    }
}