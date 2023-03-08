using Godot;
using System.Collections.Generic;

namespace MathRPG
{
    public class Player: KinematicBody2D
    {
        const int speed = 100;
        List<Vector2> path = new List<Vector2>() {};
        AnimatedSprite animatedSprite;
        [Signal]
        public delegate void MovementDone();

        public override void _Ready()
        {
            animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
        }

        public override void _PhysicsProcess(float delta)
        {
            if (path.Count != 0) // как только мы получаем не пустой путь, то начинаем движение
                MoveAlongPath(delta);
            else
                animatedSprite.Play("stay");
        }

        // Отсюда мы сможем устанавливать путь
        public void SetPath(List<Vector2> path)
        {
            this.path = path;
        }

        // Само движение по пути
        private void MoveAlongPath(float delta)
        {
            // Подруб анимации
            animatedSprite.Play("walk");
            animatedSprite.FlipH = Position.x - path[path.Count - 1].x > 2;
            // Передвижение 
            var nextCell = path[0];
            MoveAndSlide(Position.DirectionTo(nextCell).Normalized() * speed);
            // Просчет дистанции (прошли точку или нет, если да, то убираем ее из пути)
            var distanse = speed * delta; // расстояние, на которое может двигать игрока на текущем кадре (скорость * время)
            var distanseToNextCell = Position.DistanceTo(nextCell);
            if (distanse > distanseToNextCell)
                path.RemoveAt(0);
            if (path.Count == 0)
                EmitSignal(nameof(MovementDone));
        }


    }
}