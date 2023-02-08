using Godot;
// using System;
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
            List<Vector2> cells = FromGodotArrayToList(tileMap.GetUsedCells());
            cells.Sort();
            
            var toTheCentreOfTheCell = tileMap.CellSize / 2;

            for (int i = 0; i < cells.Count; i++)
            {
                var cell = (Vector2)cells[i];
                // конвентируем ячейки из локальных координат сетки в глобальные
                // + смещаем центр ячейки в середину
                astar.AddPoint(i, tileMap.MapToWorld(cell) + toTheCentreOfTheCell);

                // законектить достаточно только с левым и верхник соседом
                var neighbours = new List<Vector2>() {
                    new Vector2(cell.x, cell.y - 1),
                    new Vector2(cell.x - 1, cell.y)
                };

                // соединяем все  это дело и получаем сетку
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

        // вспомогательная функция для перевода из списка годота в список с#
        // (разницы м/у этими списками почти нет, но у с# есть сортировка)
        private List<Vector2> FromGodotArrayToList(Array godotArray)
        {
            List<Vector2> result = new List<Vector2>();
            foreach (Vector2 vector2 in godotArray)
            {
                result.Add(vector2);
            }
            return result;
        }
    }   
}