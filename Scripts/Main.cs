using Godot;
using System;

namespace MathRPG
{
    public class Main : Node2D
    {
        PathFinder pathFinder;
        Player player;

        public override void _Ready()
        {
            pathFinder = new PathFinder(GetNode<TileMap>("Ground"));

            player = GetNode<Player>("Player");
            player.Position = pathFinder.GetClosestPosition(player.Position); // Прикрепление позиции игрока к сетке
        }

        public override void _Input(InputEvent @event)
        {
            if (!(@event is InputEventMouseButton) || !(@event.IsPressed()))
                return;
            
            var mousePosition = GetGlobalMousePosition();
            var path = pathFinder.GetMovePath(player.Position, mousePosition);
            player.SetPath(path);
        }
    }
}