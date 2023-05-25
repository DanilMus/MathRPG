using System;
using Godot;
using System.Collections.Generic;
using MathRPG.Scripts.Player;

namespace MathRPG
{
	public class Main : Node2D
	{
		private PathFinder pathFinder; // Класс нахождения пути
		private Player player; // Игрок
		
		[Export]
		public PackedScene pathCellScene; // переменная для хранения нашей сцены
		private Node2D moveArea; // для хранения сцен pathCellScene
		

		public override void _Ready() // Первая инициализация сцены
		{
			pathFinder = new PathFinder(GetNode<TileMap>("Ground"));

			var hud = ResourceLoader.Load<PackedScene>("res://Scenes/HUD.tscn").Instance();
			AddChild(hud);

			player = GetNode<Player>("Player");
			player.Position = pathFinder.GetClosestPosition(player.Position); // Прикрепление позиции игрока к сетке

			DrawMoveArea(pathFinder.GetAreaInRadius(player.Position , 4)); // Рисуем пути
		}

		public override void _Input(InputEvent @event)
		{
			if (!(@event is InputEventMouseButton) || !@event.IsPressed())
				return;

			CleanMoveArea();
			SetPath();
		}

		public void SetPath()
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

		private void DrawMoveArea(List<Vector2> area)
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
		private void CleanMoveArea()
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
