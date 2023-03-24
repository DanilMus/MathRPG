using System;
using System.Collections.Generic;
using Godot;

namespace MathRPG.Scripts.BaseClasses
{
    public abstract class Entity: KinematicBody2D
    {
        private int _speed;
        private List<Vector2> _path;
        private AnimatedSprite _animatedSprite;

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
        
        [Signal]
        public delegate void MovementDone();

        public abstract override void _Ready();

        public override void _PhysicsProcess(float delta)
        {
            if (Path.Count != 0)
                Move(delta);
            else
                AnimatedSprite.Play("stay");
        }

        protected virtual void InitializeVariables()
        {
            Speed = 100;
            AnimatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
            Path = new List<Vector2>();
        }

        protected virtual void Move(float delta)
        {
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