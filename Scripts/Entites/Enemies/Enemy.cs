using System;
using System.Collections.Generic;
using Godot;

using MathRPG.Entities.Heroes;

namespace MathRPG.Entities.Enemies
{
    public abstract class Enemy : Entity
    {
        // Доп св-ва у врага
        Area2D _area;
        int _viewRadius;
        double _learningSpeed;
        
        // Нейроны ИИ
        double [,] neurons_0_1;
        double [,] neurons_1_2;

        // Проверки на то, что произошло
        bool _isKilled;
        bool _isHurt;
        bool _isDead;
        bool _isInjured;

        // Запоминание
        Vector2 _move;
        double[,] _dropoutMask;
        double[,] _layer0, _layer1, _layer2;


        // Доп св-ва у врага
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

            LoadMemory();
            IsKilled = false;
            IsHurt = false;
            IsDead = false;
            IsInjured = false;
        }

        public void Killing(Node body)
        {
            if (body is Hero)
                AnimatedSprite.Play("kills");
        }

        protected void OnEnemyMovementDone() // Когда завершает ход
        {
            Education();
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

            Memory.StoreVar(Neurons_0_1);
            Memory.StoreVar(Neurons_1_2);

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

            Neurons_0_1 = (double[,])Memory.GetVar();
            Neurons_1_2 = (double[,])Memory.GetVar();

            Memory.Close();
        }


        // ИИ
        protected Vector2 Thinking(Vector2 playerPosition)
        {
            LoadMemory();

            int startSize = 1 * 2 + 1 * 2; // Игрок + еще себя не забываем
            int hiddenSize = 17;
            int finishSize = 2;

            if (
                Neurons_0_1 == null || Neurons_1_2 == null 
                || Neurons_0_1.GetLength(0) != startSize || Neurons_0_1.GetLength(1) != hiddenSize  
                || Neurons_1_2.GetLength(0) != hiddenSize || Neurons_1_2.GetLength(1) != finishSize 
            )
            {
                _RandomInitNeurons(startSize, hiddenSize, finishSize);
                SaveMemory();
            }

            _layer0 = _FromInfoToDoubleLayer(playerPosition, Position);
            
            _layer1 = _Relu(_MatrixMultiplication(_layer0, Neurons_0_1));
            _dropoutMask = DropoutMask(_layer1.Length);
            _ArraysMultiplication(_layer1, _dropoutMask);
            
            _layer2 = _MatrixMultiplication(_layer1, Neurons_1_2);

            Vector2 result = new Vector2((float)_layer2[1,0], (float)_layer2[1,1]);
            _move = result;

            return result;
        }
        
        protected void Education()
        {
            double[,] layer2Delta;
            double[,] layer1Delta;

            layer2Delta = Loss();

            layer1Delta = _MatrixMultiplication(layer2Delta, _T(Neurons_1_2));
            layer1Delta = _ArraysMultiplication(layer1Delta, _Relu(_layer1));
            layer1Delta = _ArraysMultiplication(layer1Delta, _dropoutMask);

            Neurons_1_2 = _ArraysAddtion(Neurons_1_2, _Alpha( _MatrixMultiplication( _T(_layer1), layer2Delta ) ) );
            Neurons_0_1 = _ArraysAddtion(Neurons_0_1, _Alpha( _MatrixMultiplication( _T(_layer0), layer1Delta ) ) );
        }

        protected double[,] Loss() // Функция потерь для оценки правильности действий
        {
            double score = 0;
            
            if (IsKilled)
                score += 0.5;
            if (IsHurt)
                score += 0.5;
            if (IsDead)
                score -= 0.5;
            if (IsDead)
                score -= 0.5;
            
            double[,] loss = {{score * Math.Log(_move.x), score * Math.Log(_move.y)}};

            return loss;
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
            Random random = new Random();

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
            double[,] _matrix = new double[matrix.GetLength(1), matrix.GetLength(0)];

            for (int j = 0; j < matrix.GetLength(1); j++)
                for (int i = 0; i < matrix.GetLength(0); i++)
                    _matrix[j, i] = matrix[i, j];
            
            return _matrix;
        }

        // Операции с массивами и матицами
        double[,] _MatrixMultiplication(double[,] matrix1, double[,] matrix2)
        {
            double[,] result = new double[matrix1.GetLength(0), matrix2.GetLength(1)];

            for (int i = 0; i < matrix1.GetLength(0); i++)
                for (int j = 0; j < matrix2.GetLength(1); j++)
                    for (int k = 0; k < matrix1.GetLength(0); k++)
                        result[i, j] += matrix1[i, k] * matrix2[k, j];

            return result;
        } 
        double[,] _ArraysMultiplication(double[,] arr1, double[,] arr2)
        {
            if (arr1.Length == arr2.Length)
                throw new ArgumentException("Lengths are not equal.");
            
            for (int i = 0; i < arr1.GetLength(1); i++)
                arr1[0, i] *= arr2[0, i];

            return arr1;
        }
        double[,] _ArraysAddtion(double[,] arr1, double[,] arr2)
        {
            if (arr1.Length == arr2.Length)
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
    }
}
