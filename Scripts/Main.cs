using Godot;
using System.Collections.Generic;

using MathRPG.Path;
using MathRPG.Entities;
using MathRPG.Entities.Heroes;

namespace MathRPG
{
    public class Main : Node2D
    {
        // Здесь подготавливаем переменнные
        // Навигация по миру
        protected PathFinder pathFinder; // Класс нахождения пути

        // Игрок и все с ним связанное 
        protected Player player; // Игрок
        [Export]
        public PackedScene pathCellScene; // переменная для хранения нашей сцены
        protected Node2D moveArea; // для хранения сцен pathCellScene

        // Другие существа
        protected List<Entity> entities = new List<Entity>();
        protected List<Vector2> entitiesPositions = new List<Vector2>(); // позиции других существ для отрисовки пути
        
        // Работа с катсценами
        protected AnimationPlayer cutscenes; // для катсцены
        int cutsceneNum = 0; // номер текущей катсцены
        string[] cutscenesNames; // хранение названий катсцен

        

        // Здесь происходит загрузка
        public override void _Ready() // Первая инициализация сцены
        {
            InitializeVariables();
        }
        protected void InitializeVariables()
        {
            // Подключение навигации
            pathFinder = new PathFinder(GetNode<TileMap>("Ground"));

            // Подключение игрока и всем связанным с ним
            player = GetNode<Player>("Player");
            player.Position = pathFinder.GetClosestPosition(player.Position); // Прикрепление позиции игрока к сетке
            DrawMoveArea(pathFinder.GetAreaInRadius(player.Position , player.MoveRadius, entitiesPositions)); // Рисуем пути

            // Подключение катсцен
            cutscenes = GetNode<AnimationPlayer>("Cutscenes");
            cutscenesNames = cutscenes.GetAnimationList();
        }



        // Здесь обработка полученных нажатий
        public override void _Input(InputEvent @event)
        {
            if (@event is InputEventMouseButton && @event.IsPressed())
            {
                CleanMoveArea();
                var mousePosition = GetGlobalMousePosition();
                SetPath(mousePosition, player);
            }
            else if (@event is InputEventScreenTouch && @event.IsPressed())
            {
                // тут надо будет прописать для телефона
            }
        }
        


        // Отработка сигналов
        public void OnPlayerMovementDone() // Вызывается, когда игрок закончил движение
        {
            DrawMoveArea(pathFinder.GetAreaInRadius(player.Position , player.MoveRadius, entitiesPositions));
        }
        public void OnEntityMovementDone()
        {
            CleanMoveArea();
            LoadEnitiesPositions();
            DrawMoveArea(pathFinder.GetAreaInRadius(player.Position , player.MoveRadius, entitiesPositions));
        }


        // Прочие функции
        public void SetPath(Vector2 whereGo, Entity whoGo)
        {
            // LoadEnitiesPositions();
            var path = pathFinder.GetMovePathInRadius(whoGo.Position, whereGo, whoGo.MoveRadius, entitiesPositions);
            whoGo.Path = path;
        }
        protected void DrawMoveArea(List<Vector2> area)
        {
            CleanMoveArea();
            moveArea = new Node2D();
            
            for (int i = 1; i < area.Count; i++) // пропускаяем ячейку, где стоит персонаж
            {
                Vector2 cell = area[i];
                var pathCell = (PathCell) pathCellScene.Instance();
                pathCell.Position = cell;
                moveArea.AddChild(pathCell);
            }
            
            AddChild(moveArea);
        }
        protected void CleanMoveArea()
        {
            if (moveArea != null)
            {
                RemoveChild(moveArea);
                moveArea.QueueFree();
                moveArea = null;
            }
        }
        public void PlayScene()
        {
            if (cutsceneNum < cutscenesNames.Length && !cutscenes.IsPlaying())
            { 
                cutscenes.Play(cutscenesNames[ cutsceneNum++ ]);
                GD.Print(cutscenesNames[ cutsceneNum-1 ]);
            }
        }
        protected void LoadEnitiesPositions()
        {
            entitiesPositions = new List<Vector2>();
            foreach (Entity entity in entities)
                entitiesPositions.Add(entity.Position);
        }
        protected void PrepareEneities()
        {
            
        }
    }
}