using Godot;

using MathRPG.Entities;
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
        }

        public override void _Input(InputEvent @event)
        {
            base._Input(@event);
        }

        public void SetPathFromCutscenes(Vector2 whereGo, NodePath whoGoPath) // Такая установка пути не ограничивает существа в радиусе передвижения
        {
            Entity whoGo = GetNode<Entity>(whoGoPath);
            var path = pathFinder.GetMovePath(whoGo.Position, whereGo);
            whoGo.Path = path;
        }

        public void AfterChildhood()
        {
            GetTree().ChangeScene("res://Scenes/LVLs/lvl0_afterChildhood.tscn");
        }
    }
}