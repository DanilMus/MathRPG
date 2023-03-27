using Godot;
using Godot.Collections;
using System.Collections.Generic;

using MathRPG.Path;
using MathRPG.Entities.Heroes;
using MathRPG.Entities.Enemies;

namespace MathRPG.LVL
{
    public class lvl0 : Node2D
    {
        PathFinder pathFinder; // Класс нахождения пути
        Player player; // Игрок
        [Export]
        public PackedScene pathCellScene; // переменная для хранения нашей сцены
        Node2D moveArea; // для хранения сцен pathCellScene
        AnimationPlayer cutscenes; // для катсцен
        AnimatedSprite friend; // для друга
        int cutsceneNum = 0; // номер текущей катсцены
        string[] cutscenesNames; // хранение названий катсцен
        

        public override void _Ready() // Первая инициализация сцены
        {
            pathFinder = new PathFinder(GetNode<TileMap>("Ground"));

            player = GetNode<Player>("Player");
            player.Position = pathFinder.GetClosestPosition(player.Position); // Прикрепление позиции игрока к сетке

            DrawMoveArea(pathFinder.GetAreaInRadius(player.Position , 4)); // Рисуем пути

            cutscenes = GetNode<AnimationPlayer>("Cutscenes");
            cutscenesNames = cutscenes.GetAnimationList();

            friend = GetNode<AnimatedSprite>("FirstFriend");
            friend.Position = pathFinder.GetClosestPosition(friend.Position);
            friend.Connect("MousePressed", this, nameof(PlayScene));
        }

        public override void _Input(InputEvent @event)
        {
            if (!(@event is InputEventMouseButton) || !(@event.IsPressed()))
                return;
            
            CleanMoveArea();
            SetPath();
        }

        public void PlayScene()
        {
            if (cutsceneNum < cutscenesNames.Length && !cutscenes.IsPlaying())
                cutscenes.Play(cutscenesNames[ cutsceneNum++ ]);
        }
        

        void SetPath()
        {
            var mousePosition = GetGlobalMousePosition();
            var areaInRadius = pathFinder.GetAreaInRadius(player.Position , 4);
            var nextCell = pathFinder.GetClosestPositionFromList(mousePosition, areaInRadius);

            var path = pathFinder.GetMovePath(player.Position, nextCell);
            player.Path = path;
        }

        public void OnPlayerMovementDone() // Вызывается, когда игрок закончил движение
        {
            DrawMoveArea(pathFinder.GetAreaInRadius(player.Position , 4));
        }

        void DrawMoveArea(List<Vector2> area)
        {
            moveArea = new Node2D();
            for (int i = 1; i < area.Count; i++) // пропускаяем ячейку, где стоит персонаж
            {
                Vector2 cell = area[i];
                var pathCell = (PathCell)pathCellScene.Instance();
                pathCell.Position = cell;
                moveArea.AddChild(pathCell);
            }
            AddChild(moveArea);
        }

        void CleanMoveArea()
        {
            if (moveArea != null)
            {
                RemoveChild(moveArea);
                moveArea.QueueFree();
                moveArea = null;
            }
        }
    }
}