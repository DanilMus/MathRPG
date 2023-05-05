using Godot;
//using System;
using System.Collections.Generic;

namespace MathRPG
{
    public class Main : Node2D
    {
        PathFinder pathFinder; // Класс нахождения пути
        Player player; // Игрок
        List<Vector2> prevPlayerPath = null; //Предыдущий путь персонажа(запоминается с целью возврата к нему, если окажется, что мы нажали на кнопку)
        [Export]
        public PackedScene pathCellScene; // переменная для хранения нашей сцены
        Node2D moveArea; // для хранения сцен pathCellScene

        

        public override void _Ready() // Первая инициализация сцены
        {
            pathFinder = new PathFinder(GetNode<TileMap>("Ground"));

            player = GetNode<Player>("Player");
            player.Position = pathFinder.GetClosestPosition(player.Position); // Прикрепление позиции игрока к сетке

            DrawMoveArea(pathFinder.GetAreaInRadius(player.Position , 4)); // Рисуем пути
        }

        public override void _Input(InputEvent @event)
        {
            if (!(@event is InputEventMouseButton) || !(@event.IsPressed()))
            {
                return;
            }
            prevPlayerPath=GetNode<Player>("Player").path;  //Запоминаем старый путь
            CleanMoveArea();
            SetPath();
        }

        public void SetPath()
        {
            var mousePosition = GetGlobalMousePosition();
            var areaInRadius = pathFinder.GetAreaInRadius(player.Position , 4);
            var nextCell = pathFinder.GetClosestPositionFromList(mousePosition, areaInRadius);

            var path = pathFinder.GetMovePath(player.Position, nextCell);
            player.SetPath(path);
        }

        public void OnPlayerMovementDone() // Вызывается, когда игрок закончил движение
        {
            DrawMoveArea(pathFinder.GetAreaInRadius(player.Position , 4));
        }

        private void DrawMoveArea(List<Vector2> area)
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

        private void CleanMoveArea()
        {
            if (moveArea != null)
            {
                RemoveChild(moveArea);
                moveArea.QueueFree();
                moveArea = null;
            }
        }
        
        public void _on_BigPotionButton_pressed() //Вызывается, когда нажата кнопка с большим зельем
        {
            if(prevPlayerPath!=null)
            {
               GetNode<Player>("Player").path=prevPlayerPath;  //Возвращаемся к старому пути
            }
        }
        
        public void _on_SmallPotionButton_pressed() //Вызывается, когда нажата кнопка с малым зельем
        {
            if(prevPlayerPath!=null)
            {
               GetNode<Player>("Player").path=prevPlayerPath;  //Возвращаемся к старому пути
            }
        }
    }
}
