using System;
using System.Collections.Generic;
using Godot;

namespace MathRPG.Entities
{
    public abstract class Entity: KinematicBody2D
    {
        private int _speed;
        private List<Vector2> _path;
        private AnimatedSprite _animatedSprite;
        private int _health;
        private int _moveRadius;
        File _memory;
        string _memoryPath;

        [Signal]
        public delegate void MovementDone();

        public List<Vector2> Path
        {
            get => _path;
            set
            {
                if (value == null) throw new ArgumentNullException();

                _path = value;
            }
        }
        protected AnimatedSprite AnimatedSprite
        {
            get => _animatedSprite;
            set
            {
                if (value == null) throw new ArgumentNullException();

                _animatedSprite = value;
            }
        }
        protected int Speed
        {
            get => _speed;
            set
            {
                if (value <= 0) throw new ArgumentException("Speed value should be more than zero");

                _speed = value;
            }
        }
        public int Health
        {
            get => _health;
            set 
            {
                if (value < 0) throw new ArgumentException("Health value should be more than zero");

                _health = value; 
            }
        }
        public int MoveRadius
        {
            get => _moveRadius;
            set
            {
                if (value < 0) throw new ArgumentException("MoveRadius value should be more than zero");

                _moveRadius = value; 
            } 
        }
        protected File Memory
        {
            get => _memory;
            set
            {
                if (value == null) throw new ArgumentException();

                _memory = value;
            }
        }
        protected string MemoryPath
        {
            get => _memoryPath;
            set
            {
                if (value == null) throw new ArgumentException();

                _memoryPath = value;
            }
        }

        public override void _PhysicsProcess(float delta)
        {
            if (Path.Count != 0)
                Move(delta);
            else if (AnimatedSprite.Playing != true || AnimatedSprite.Animation == "walk")
                AnimatedSprite.Play("stay");
        }

        protected virtual void InitializeVariables()
        {
            Speed = 100;
            MoveRadius = 3;
            AnimatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
            Path = new List<Vector2>();
            Memory = new File();
        }

        protected virtual void Move(float delta)
        {
            // Включаем анимацию
            AnimatedSprite.Play("walk");
            AnimatedSprite.FlipH = Position.x - Path[Path.Count - 1].x > 2;

            // Передвижение 
            var nextCell = Path[0];
            MoveAndSlide(Position.DirectionTo(nextCell).Normalized() * Speed);
            
            // Просчет дистанции (прошли точку или нет, если да, то убираем ее из пути)
            var distance = Speed * delta; // расстояние, на которое может двигать игрока на текущем кадре (скорость * время)
            var distanceToNextCell = Position.DistanceTo(nextCell);
            
            if (distance > distanceToNextCell)
                Path.RemoveAt(0);
            if (Path.Count == 0)
                EmitSignal(nameof(MovementDone));
        }
    }
}