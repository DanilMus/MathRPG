using Godot;
using System.Collections.Generic;

namespace MathRPG.Player
{
    public class Player: KinematicBody2D
    {
        const int speed = 40;
        List<Vector2> path = new List<Vector2>();

        public override void _PhysicsProcess(float delta)
        {
            // как только мы получаем не пустой путь, то начинаем движение
            if (path.Count != 0)
                MoveAlongPath(delta);
        }

        // отсюда мы сможем устанавливать путь
        public void SetPath(List<Vector2> path)
        {
            this.path = path;
        }

        // само движение по пути
        private void MoveAlongPath(float delta)
        {
            // var distanse = speed * delta; // расстояние, на которое может двигать игрока на текущем кадре (скорость * время)
            for (int i = 0; i < path.Count; i++)
            {
                var nextCell = path[i];
                // var distanseToNextCell = Position.DistanceTo(nextCell);
                
                // distanse -= distanseToNextCell; // так как мы не знаем, сколько пройдем на текущем кадре, то смотрим на оставшееся расстояние
                MoveAndSlide(Position.DirectionTo(nextCell).Normalized() * speed);
            }
            path.Clear();
        }   
    }
}