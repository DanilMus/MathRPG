// Загрузка библиотек
using Godot;

using MathRPG.Entities;
using MathRPG.Entities.Enemies;

namespace MathRPG.LVL
{
    public class TestEnemy : Main
    {
        // Подготовка переменных
        Node2D enemies;
        


        // Загрузка сцены
        public override void _Ready() 
        {
            base.InitializeVariables();

            enemies = GetNode<Node2D>("Enemies");

        }



        // Обработка нажатий
        public override void _Input(InputEvent @event)
        {
            base._Input(@event);
        }
    
    
    
        // Обработка сигналов
        public void OnTimerForMoveTimeout() // Высчитвает время, чтобы враг сходил
        {
            // Враг делает свой ход
            Vector2 move = enemy.Thinking(player.Position);
            SetPath(move, enemy);
        }
    }
}