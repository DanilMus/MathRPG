using Godot;
using System.Collections.Generic;

using MathRPG.Path;
using MathRPG.Entities;
using MathRPG.Entities.Heroes;

namespace MathRPG
{
    public class Main : Node2D
    {
        protected PathFinder pathFinder; // Класс нахождения пути
        protected Player player; // Игрок
        [Export]
        public PackedScene pathCellScene; // переменная для хранения нашей сцены
        protected Node2D moveArea; // для хранения сцен pathCellScene
        protected AnimationPlayer cutscenes; // для катсцены
        protected Node2D entities;
        protected List<Vector2> entitiesPositions = new List<Vector2>();
        int cutsceneNum = 0; // номер текущей катсцены
        string[] cutscenesNames; // хранение названий катсцен

        

        public override void _Ready() // Первая инициализация сцены
        {
            InitializeVariables();
        }

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
        

        protected void InitializeVariables()
        {
            pathFinder = new PathFinder(GetNode<TileMap>("Ground"));

            // Работа с другими существами
            entities = GetNode<Node2D>("Entities");
            foreach(Entity entity in entities.GetChildren())
            {
                entity.Position = pathFinder.GetClosestPosition(entity.Position);
                entitiesPositions.Add(entity.Position);
                entity.Connect("MovementDone", this, nameof(OnEitityMovementDone));
            }

            player = GetNode<Player>("Player");
            player.Position = pathFinder.GetClosestPosition(player.Position); // Прикрепление позиции игрока к сетке

            DrawMoveArea(pathFinder.GetAreaInRadius(player.Position , player.MoveRadius, entitiesPositions)); // Рисуем пути

            cutscenes = GetNode<AnimationPlayer>("Cutscenes");
            cutscenesNames = cutscenes.GetAnimationList();
        }

        public void SetPath(Vector2 whereGo, Entity whoGo)
        {
            // LoadEnitiesPositions();
            var path = pathFinder.GetMovePathInRadius(whoGo.Position, whereGo, whoGo.MoveRadius, entitiesPositions);
            whoGo.Path = path;
        }

        public void OnPlayerMovementDone() // Вызывается, когда игрок закончил движение
        {
            DrawMoveArea(pathFinder.GetAreaInRadius(player.Position , player.MoveRadius, entitiesPositions));
        }

        public void OnEitityMovementDone()
        {
            CleanMoveArea();
            LoadEnitiesPositions();
            DrawMoveArea(pathFinder.GetAreaInRadius(player.Position , player.MoveRadius, entitiesPositions));
        }

        protected void DrawMoveArea(List<Vector2> area)
        {
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
            entitiesPositions.Clear();
            foreach(Entity entity in entities.GetChildren())
            {
                entitiesPositions.Add(entity.Position);
            }
        }
    }
}