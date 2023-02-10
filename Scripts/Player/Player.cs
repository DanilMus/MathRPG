using Godot;
using System.Collections.Generic;

namespace MathRPG
{
    public class Player: KinematicBody2D
    {
        const int speed = 100;
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
            // передвижение 
            var nextCell = path[0];
            MoveAndSlide(Position.DirectionTo(nextCell).Normalized() * speed);
            // просчет дистанции (прошли точку или нет, если да, то убираем ее из пути)
            var distanse = speed * delta; // расстояние, на которое может двигать игрока на текущем кадре (скорость * время)
            var distanseToNextCell = Position.DistanceTo(nextCell);
            if (distanse > distanseToNextCell)
                path.RemoveAt(0);
        }
    }
}