// Загрузка библиотек
using Godot;

using MathRPG.Entities;
using MathRPG.Entities.Heroes;

namespace MathRPG.LVL
{
    public class lvl0 : Main
    {
        // Подготовка переменных
        FirstFriend friend; // для друга
        


        // Загрузка сцены
        public override void _Ready() // Первая инициализация сцены
        {
            InitializeVariables();
        }
        protected override void InitializeVariables()
        {
            // Старые подлючения
            base.InitializeVariables();

            // И новый друг
            friend = GetNode<FirstFriend>("FirstFriend");
            entities.Add((Entity)friend);
        }



        // Обработка полученных нажатий
        public override void _Input(InputEvent @event)
        {
            base._Input(@event);
        }



        // Доп функции
        public void SetPathFromCutscenes(Vector2 whereGo, NodePath whoGoPath) // Такая установка пути не ограничивает существа в радиусе передвижения
        {
            Entity whoGo = GetNode<Entity>(whoGoPath);
            var path = pathFinder.GetMovePath(whoGo.Position, whereGo);
            whoGo.Path = path;
        }
        public void AfterChildhood() // Загрузка на другую сцену
        {
            GetTree().ChangeScene("res://Scenes/LVLs/lvl0_afterChildhood.tscn");
        }
    }
}