using Godot;
using System;
using System.Collections.Generic;
using Godot.Collections;

namespace MathRPG.Player
{
    public class PathFinder
    {
        // хранение карты тайлов
        private TileMap groundTileMap;
        // хранение графа 
        private AStar2D astar;

        public PathFinder(TileMap _groundTileMap)
        {
            groundTileMap = _groundTileMap;
        }

        // настройка графа
        private void InitAStar(TileMap tileMap)
        {
            astar = new AStar2D();
            // получаем массив с кооридинатами клеток
            var cells = tileMap.GetUsedCells();
            
            var toTheCentreOfTheCell = tileMap.CellSize / 2;

            for (int i = 0; i <= cells.Count; i++)
            {
                var cell = (Vector2)cells[i];
                // конвентируем ячейки из локальных координат сетки в глобальные
                // + смещаем центр ячейки в середину
                astar.AddPoint(i, tileMap.MapToWorld(cell) + toTheCentreOfTheCell);

                // законектить достаточно только с левым и верхник соседом
                var neighbours = new List<Vector2>() {
                    new Vector2(cell.x, cell.y--),
                    new Vector2(cell.x--, cell.y)
                };

                // соединяем все это дело и получаем сетку
                //  _ _ _
                // |_|_|_|
                // |_|_|_|
                // |_|_|_|
                foreach (var neighbour in neighbours)
                {
                    if (tileMap.GetCellv(neighbour) == TileMap.InvalidCell)
                        continue;
                    var toId = astar.GetClosestPoint(tileMap.MapToWorld(neighbour) + toTheCentreOfTheCell);
                    astar.ConnectPoints(i, toId);
                }
            }
        }
    }   
}