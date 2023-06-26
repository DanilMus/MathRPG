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
            InitializeVariables();
        }
        protected override void InitializeVariables()
        {
            // Основные подключения
            base.InitializeVariables();

            // И новое подлючение (враг)
            enemies = GetNode<Node2D>("Enemies");
            // Добавим новых существ (врагов)
            foreach (Entity entity in enemies.GetChildren())
                entities.Add(entity);
            // И так как мы подлючили новых существ
            PrepareEntities();
        }



        // Обработка нажатий
        public override void _Input(InputEvent @event)
        {
            base._Input(@event);
        }
    
    
    
        // Обработка сигналов
        public void OnTimerForMoveTimeout() // Высчитвает время, чтобы враг сходил
        {
            foreach (NumEnemy enemy in enemies.GetChildren())
            {
                // Враг делает свой ход
                Vector2 move = enemy.Thinking(player.Position);
                SetPath(move, enemy);
            }
        }
    }
}