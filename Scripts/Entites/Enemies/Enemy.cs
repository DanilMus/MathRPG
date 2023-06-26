using System;
using System.Collections.Generic;
using Godot;

using MathRPG.Entities.Heroes;

namespace MathRPG.Entities.Enemies
{
    public abstract class Enemy : Entity
    {
        // Переменные у врага
        static Random random = new Random();
        // Доп св-ва у врага
        Area2D _area;
        int _viewRadius;
        double _learningSpeed;
        // Нейроны ИИ
        double [,] neurons_0_1;
        double [,] neurons_1_2;
        // Размеры ИИ
        int startSize = 1 * 2 + 1 * 2; // Игрок + еще себя не забываем
        int hiddenSize = 17;
        int finishSize = 2;
        // Проверки на то, что произошло
        bool _isKilled;
        bool _isHurt;
        bool _isDead;
        bool _isInjured;
        // Запоминание
        Vector2 _move;
        double[,] _dropoutMask;
        double[,] _layer0, _layer1, _layer2;



        // Свойства у врага
        protected int ViewRadius
        {
            get => _viewRadius;
            set 
            {
                if (value < 0) throw new ArgumentException("ViewRadius value should be more than zero");

                _viewRadius = value;
            }
        }
        protected Area2D Area
        {
            get => _area;
            set
            {
                if (value == null) throw new ArgumentNullException();

                _area = value;
            }
        }
        protected double LearningSpeed
        {
            get => _learningSpeed;
            set
            {
                if (value < 0) throw new ArgumentException("LearningSpeed value should be more than zero");
            
                _learningSpeed = value;
            }
        }
       // Нейроны ИИ
        protected double[,] Neurons_0_1
        {
            get => neurons_0_1;
            set
            {
                if (value == null || value.GetUpperBound(0) < 1 || value.GetUpperBound(1) < 1 )
                    throw new ArgumentNullException();
                
                neurons_0_1 = value;
            }
        }
        protected double[,] Neurons_1_2
        {
            get => neurons_1_2;
            set
            {
                if (value == null || value.GetUpperBound(0) < 1 || value.GetUpperBound(1) < 1 )
                    throw new ArgumentNullException();
                
                neurons_1_2 = value;
            }
        }
        // Проверки на то, что произошло
        protected bool IsKilled 
        { get => _isKilled; set {_isKilled = value;} }
        protected bool IsHurt 
        { get => _isHurt; set {_isHurt = value;} }
        protected bool IsDead 
        { get => _isDead; set {_isDead = value;} }
        protected bool IsInjured 
        { get => _isInjured; set {_isInjured = value;} }



        // Загрузка врага
        public override void _Ready()
        {
            InitializeVariables();
        }
        protected override void InitializeVariables()
        {
            base.InitializeVariables();
            ViewRadius = 7;
            Area = GetNode<Area2D>("Area2D");
            LearningSpeed = 0.001705;

            IsKilled = false;
            IsHurt = false;
            IsDead = false;
            IsInjured = false;

            LoadMemory();
        }



        // Обработка сигналов
        public void Killing(Entity body)
        {
            if (body is Player)
            {
                SetPhysicsProcess(false);
                AnimatedSprite.FlipH = Position.x - body.Position.x > 2;
                AnimatedSprite.Play("kills");

                Timer timer = new Timer();
                timer.WaitTime = 2.0f; timer.OneShot = true;
                AddChild(timer);
                timer.Connect("timeout", this, nameof(ReturnPhysicsProcess));
                timer.Start();

                IsInjured = true;
                if (!body.IsAlive)
                {
                    IsKilled = true;
                    GD.Print("Killed");
                }
            }
        }
        public void OnEnemyMovementDone() // Когда завершает ход
        {
            Education();
        }
        public void ReturnPhysicsProcess()
        {
            SetPhysicsProcess(true);
        }



        // Дальше идет ИИ и функции для его работы
        public Vector2 Thinking(Vector2 playerPosition) // ИИ
        {
            LoadMemory();

            if (
                Neurons_0_1 == null || Neurons_1_2 == null 
                || Neurons_0_1.GetLength(0) != startSize || Neurons_0_1.GetLength(1) != hiddenSize  
                || Neurons_1_2.GetLength(0) != hiddenSize || Neurons_1_2.GetLength(1) != finishSize 
            )
                _RandomInitNeurons(startSize, hiddenSize, finishSize);

            _layer0 = _FromInfoToDoubleLayer(playerPosition, Position);
            
            _layer1 = _Relu(_MatrixMultiplication(_layer0, Neurons_0_1));
            _dropoutMask = DropoutMask(_layer1.Length);
            _ArraysMultiplication(_layer1, _dropoutMask);
            
            _layer2 = _MatrixMultiplication(_layer1, Neurons_1_2);

            Vector2 result = new Vector2((float)_layer2[0,0], (float)_layer2[0,1]);
            _move = result;

            SaveMemory();
            
            return result;
        }
        public void Education() // Обучение ИИ
        {
            LoadMemory();

            double[,] layer2Delta;
            double[,] layer1Delta;
            
            // Оценка ошибки
            layer2Delta = Loss();

            layer1Delta = _MatrixMultiplication(layer2Delta, _T(Neurons_1_2));
            layer1Delta = _ArraysMultiplication(layer1Delta, _Relu2Deriv(_layer1));
            layer1Delta = _ArraysMultiplication(layer1Delta, _dropoutMask);

            // Регулировка ошибки
            Neurons_1_2 = _ArraysAddtion(Neurons_1_2, _Alpha( _MatrixMultiplication( _T(_layer1), layer2Delta ) ) );
            Neurons_0_1 = _ArraysAddtion(Neurons_0_1, _Alpha( _MatrixMultiplication( _T(_layer0), layer1Delta ) ) );

            SaveMemory();
        }
        protected double[,] Loss() // Функция потерь, кот. показывает, как ИИ стоит обучаться
        {
            double score = 0;
            
            if (IsKilled)
            {
                score += 0.5;
                IsKilled = false;
            }
            if (IsHurt)
            {
                score += 0.5;
                IsHurt = false;
            }
            if (IsDead)
            {
                score -= 0.5;
                IsDead = false;
            }
            if (IsInjured)
            {
                score -= 0.5;
                IsInjured = false;
            }
            
            double[,] loss = {{score * Math.Log(_move.x), score * Math.Log(_move.y)}};

            return loss;
        }
        // Работа с памятью
        protected void SaveMemory()
        {
            Error error = Memory.Open(MemoryPath, File.ModeFlags.Write);

            if (error != Error.Ok)
            {
                GD.PrintErr("Failed to open file for writting: ", MemoryPath);
                return;
            }

            // загружаем нейроны
            for (int i = 0; i < startSize; i++)
                for (int j = 0; j < hiddenSize; j++)
                    Memory.StoreDouble(Neurons_0_1[i,j]);
            for (int i = 0; i < hiddenSize; i++)
                for (int j = 0; j < finishSize; j++)
                    Memory.StoreDouble(Neurons_1_2[i,j]);

            // загружаем слои 
            for (int i = 0; i < startSize; i++)
                Memory.StoreDouble(_layer0[0,i]);
            for (int i = 0; i < hiddenSize; i++)
                Memory.StoreDouble(_layer1[0,i]);
            for (int i = 0; i < finishSize; i++)
                Memory.StoreDouble(_layer2[0,i]);
            
            // про dropoutmask не забываем
            for (int i = 0; i < hiddenSize; i++)
                Memory.StoreDouble(_dropoutMask[0,i]);

            Memory.Close();
        }
        protected void LoadMemory()
        {
            Error error = Memory.Open(MemoryPath, File.ModeFlags.Read);

            if (error != Error.Ok)
            {
                GD.PrintErr("Failed to open file for reading: ", MemoryPath);
                return;
            }

            // загружаем нейроны
            double[,] matrix = new double[startSize, hiddenSize];
            for (int i = 0; i < startSize; i++)
                for (int j = 0; j < hiddenSize; j++)
                {
                    matrix[i,j] = Memory.GetDouble();
                    if (double.IsNaN(matrix[i,j]))
                        matrix[i,j] = random.NextDouble();
                }
            Neurons_0_1 = matrix;

            matrix = new double[hiddenSize, finishSize];
            for (int i = 0; i < hiddenSize; i++)
                for (int j = 0; j < finishSize; j++)
                {
                    matrix[i,j] = Memory.GetDouble();
                    if (double.IsNaN(matrix[i,j]))
                        matrix[i,j] = random.NextDouble();
                }
            Neurons_1_2 = matrix;

            // загружаем слои
            double[,] layer = new double[1, startSize];
            for (int i = 0; i < startSize; i++)
                layer[0,i] = Memory.GetDouble();
            _layer0 = layer;

            layer = new double[1, hiddenSize];
            for (int i = 0; i < hiddenSize; i++)
                layer[0,i] = Memory.GetDouble();
            _layer1 = layer;

            layer = new double[1, finishSize];
            for (int i = 0; i < finishSize; i++)
                layer[0,i] = Memory.GetDouble();
            _layer2 = layer;

            // и dropoutMask
            layer = new double[1, hiddenSize];
            for (int i = 0; i < hiddenSize; i++)
                layer[0,i] = Memory.GetDouble();
            _dropoutMask = layer;

            Memory.Close();
        }
        // Подготовки для данных
        double[,] _FromInfoToDoubleLayer(Vector2 playerPosition, Vector2 Position)
        {
            double[,] layer = new double[1, 1 * 2 + 1 * 2];

            if (playerPosition != null)
            {
                layer[0, 0] = playerPosition.x;
                layer[0, 1] = playerPosition.y;
            }
            else
            {
                layer[0, 0] = 0;
                layer[0, 1] = 0;
            }
            layer[0, 2] = Position.x;
            layer[0, 3] = Position.y;

            return layer;
        }
        void _RandomInitNeurons(int startSize, int hiddenSize, int finishSize)
        {
            Neurons_0_1 = new double[startSize, hiddenSize];
            Neurons_1_2 = new double[hiddenSize, finishSize];

            for (int i = 0; i < startSize; i++)
                for (int j = 0; j < hiddenSize; j++)
                    Neurons_0_1[i,j] = random.NextDouble();

            for (int i = 0; i < hiddenSize; i++)
                for (int j = 0; j < finishSize; j++)
                    Neurons_1_2[i,j] = random.NextDouble();
        }
        double[,] _T(double[,] matrix) // инверитровать
        {
            int size0 = matrix.GetLength(0); int size1 = matrix.GetLength(1);
            double[,] _matrix = new double[size1, size0];

            for (int j = 0; j < size1; j++)
                for (int i = 0; i < matrix.GetLength(0); i++)
                    _matrix[j, i] = matrix[i, j];
            
            return _matrix;
        }
        // Операции с массивами и матицами
        double[,] _MatrixMultiplication(double[,] matrix1, double[,] matrix2)
        {
            if (matrix1.GetLength(1) != matrix2.GetLength(0))
                throw new ArgumentException(
                    $"Lengths of the matrixes are not equal. {matrix1.GetLength(1)} != {matrix2.GetLength(0)}"
                );

            double[,] result = new double[matrix1.GetLength(0), matrix2.GetLength(1)];

            for (int i = 0; i < matrix1.GetLength(0); i++)
                for (int j = 0; j < matrix2.GetLength(1); j++)
                    for (int k = 0; k < matrix1.GetLength(1); k++)
                        result[i, j] += matrix1[i, k] * matrix2[k, j];

            return result;
        } 
        double[,] _ArraysMultiplication(double[,] arr1, double[,] arr2)
        {
            if (arr1.Length != arr2.Length)
                throw new ArgumentException("Lengths of the arrays are not equal.");
            
            for (int i = 0; i < arr1.GetLength(1); i++)
                arr1[0, i] *= arr2[0, i];

            return arr1;
        }
        double[,] _ArraysAddtion(double[,] arr1, double[,] arr2)
        {
            if (arr1.Length != arr2.Length)
                throw new ArgumentException("Lengths are not equal.");
            
            for (int i = 0; i < arr1.GetLength(1); i++)
                arr1[0, i] *= arr2[0, i];

            return arr1;
        }
        // ReLu и DropoutMask - создание своих корреляций в среднем слое
        double[,] _Relu(double[,] layer)
        {
            for (int i = 0; i < layer.GetLength(1); i++)
                if (layer[0, i] < 0)
                    layer[0, i] = 0;
            return layer;
        }
        double[,] _Relu2Deriv(double[,] layer)
        {
            for (int i = 0; i < layer.GetLength(1); i++)
            {
                if (layer[0, i] > 0)
                    layer[0, i] = 1;
                else
                    layer[0, i] = 0;
            }
            return layer;
        }      
        double[,] DropoutMask(int len)
        {
            Random rnd = new Random();

            double[,] dropoutMask = new double[1, len];

            for (int i = 0; i < len; i++)
                dropoutMask[0, i] = rnd.Next(2);
            
            return dropoutMask;
        }
        // Alpha - скорость обучения
        double[,] _Alpha(double[,] layer, double alpha = 0.001705200401052023)
        {
            for (int i = 0; i < layer.GetLength(1); i++)
                layer[0, i] *= alpha;
            
            return layer;
        }
        // Демонстрация
        public void ShowInfo()
        {
            GD.Print("Нейроны м/у 0 и 1 слоями");
            for (int i = 0; i < startSize; i++)
                for (int j = 0; j < hiddenSize; j++)
                    GD.Print(Neurons_0_1[i,j]);
            GD.Print("Нейроны м/у 1 и 2 слоями");
            for (int i = 0; i < hiddenSize; i++)
                for (int j = 0; j < finishSize; j++)
                    GD.Print(Neurons_1_2[i,j]);
            // GD.Print("Слой 0");
            // for (int i = 0; i < startSize; i++)
            //     GD.Print(_layer0[0,i]);
            // GD.Print("Слой 1");
            // for (int i = 0; i < hiddenSize; i++)
            //     GD.Print(_layer1[0,i]);
            // GD.Print("Слой 2");
            // for (int i = 0; i < finishSize; i++)
            //     GD.Print(_layer2[0,i]);
        }
    }
}
