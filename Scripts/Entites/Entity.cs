// Подключаем библиотеки
using System;
using System.Collections.Generic;
using Godot;

using MathRPG.Entities.Enemies;

namespace MathRPG.Entities
{
    public abstract class Entity: KinematicBody2D
    {
        // Подгружаем переменные существа
        private int _speed;
        private List<Vector2> _path;
        private AnimatedSprite _animatedSprite;
        private int _health;
        private int _moveRadius;
        File _memory;
        string _memoryPath;
        int _damage;


        // Его сигналы
        [Signal]
        public delegate void MovementDone();
        [Signal]
        public delegate void WasAttacked();


        // Свойства
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
                if (value < 0)
                    _health = 0;
                else if (value > FullHeath)
                    _health = value;
                else
                    _health = value;
            } 
        }
        public int FullHeath
        {
            get; private set;
        }
        public  int MoveRadius
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
        public int Damage
        {
            get => _damage;
            set => _damage = value;
        }
        public bool IsAlive { get; private set; }



        // Загрузка существа
        protected virtual void InitializeVariables()
        {
            Speed = 100;
            MoveRadius = 3;
            Damage = 50;
            FullHeath = 100;
            Health = FullHeath;
            IsAlive = true;
            AnimatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
            Path = new List<Vector2>();
            Memory = new File();
        }


        // Обработка сигналов
        public void Death()
        {
            IsAlive = false;
            Hide();
            // Отключение столкновения игрока может привести к
            // ошибке, поэтому мы используем SetDegerred. Он говорит Годот ждать
            // отключения, пока не будет безопасно.
            GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred("disabled", true);
            SetPhysicsProcess(false);
        }


        // Обработка действий в данный момент
        public override void _PhysicsProcess(float delta)
        {
            if (Path.Count != 0)
                Move(delta);
            else 
                AnimatedSprite.Play("stay");
        }



        // Прочие функции
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