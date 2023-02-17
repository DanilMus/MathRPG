using Godot;
// using System;
using System.Collections.Generic;
using Godot.Collections;

namespace MathRPG
{
    public class PathFinder
    {
        private TileMap groundTileMap; // Хранение карты тайлов
        private AStar2D astar; // Хранение графа 

        public PathFinder(TileMap _groundTileMap)
        {
            groundTileMap = _groundTileMap;
            InitAStar(groundTileMap);
        }

        // Настройка графа
        private void InitAStar(TileMap tileMap)
        {
            astar = new AStar2D();
            
            List<Vector2> cells = FromGodotArrayToList(tileMap.GetUsedCells()); // получаем массив с кооридинатами клеток

            cells.Sort((a, b) => { // сортировка (да, да, знаю тяжелая, но она работает, это уже что-то)
                if (a.x != b.x)
                    return a.x.CompareTo(b.x);
                else
                    return a.y.CompareTo(b.y);
            });

            var toCentreOfCell = tileMap.CellSize / 2; // Lля смещение центров ячеек

            for (int i = 0; i < cells.Count; i++)
            {
                var cell = (Vector2)cells[i];

                astar.AddPoint(i, tileMap.MapToWorld(cell) + toCentreOfCell); // Rонвентируем ячейки из локальных координат сетки в глобальные + смещаем центр ячейки в середину

                var neighbours = new List<Vector2>() { //  _ _ Законектить достаточно только с левым и верхник соседом
                    new Vector2(cell.x, cell.y - 1),   // |x|_|
                    new Vector2(cell.x - 1, cell.y)    // |_|
                };

                foreach (var neighbour in neighbours) // Соединяем все  это дело и получаем сетку
                {
                    if (tileMap.GetCellv(neighbour) == TileMap.InvalidCell) // Спасает от многих ошибок     _ _ _
                        continue;                                                                       // |_|_|_|
                    var toId = astar.GetClosestPoint(tileMap.MapToWorld(neighbour) + toCentreOfCell);   // |_|_|_|
                    astar.ConnectPoints(i, toId);                                                       // |_|_|_|
                }
            }
        }


        // Метод получения пути
        public List<Vector2> GetMovePath(Vector2 from, Vector2 to)
        {
            var path = astar.GetPointPath(
                astar.GetClosestPoint(from),
                astar.GetClosestPoint(to)
            );
            return FromVector2ArrayToList(path);
        }

        // Получаем массив клеток в определенном радиусе
        public List<Vector2> GetAreaInRadius(Vector2 cell, int radius)
        {
            var area = new List<Vector2>(); // Результат

            var toCentreOfCell = groundTileMap.CellSize / 2; // Для смещения центров ячеек

            var cellsWereAnalised = new Dictionary(); // Хранение проанализированных ячеек
            var cellsWillBeAnalised = new List<Vector2>() { // Ячейки для анализа
                groundTileMap.MapToWorld(cell)
            }; 

            for (int i = 0; i < radius; i++) // Проходимся по радиусу
            {
                var cellsForBeAnalised = new List<Vector2>(); // Запоминание ячейек для будущего анализа 

                for (int j = 0; j < cellsWillBeAnalised.Count; j++)
                {   
                    var cellToAnalise = cellsWillBeAnalised[j];

                    if (groundTileMap.GetCellv(cellToAnalise) != TileMap.InvalidCell // Необходимая проверка, которая поможет избежать кучи ошибок
                        || cellsWereAnalised.Contains(cellToAnalise)) // И проверка на то, чтобы не делать работу по второму кругу
                        continue;
                    
                    var neighbours = new List<Vector2>() { // Соседние ячеки с нашей (мы как бы расходимся в ширь)
                        new Vector2(cellToAnalise.x + 1, cellToAnalise.y), //    _
                        new Vector2(cellToAnalise.x - 1, cellToAnalise.y), //   |_|
                        new Vector2(cellToAnalise.x, cellToAnalise.y + 1), // |_|x|_|
                        new Vector2(cellToAnalise.x, cellToAnalise.y - 1)  //   |_|
                    };

                    cellsForBeAnalised.AddRange(neighbours); // Добавляем соседей в список для анализа
                    cellsWereAnalised[cellToAnalise] = true; // Данную ячеку считаем уже проанализированной
                    
                    area.Add(groundTileMap.MapToWorld(cellToAnalise) + toCentreOfCell); // Проанализированную ячеку добавляем в ответ
                }

                cellsWillBeAnalised = cellsForBeAnalised; // получаем новые ячейки(соседей) для анализа
            }

            return area;
        }

        // Вспомогательная функция для перевода из списка годота в список с# 
        private List<Vector2> FromGodotArrayToList(Array godotArray) // Рразницы м/у этими списками почти нет, но у с# есть сортировка
        {
            List<Vector2> result = new List<Vector2>();
            foreach (Vector2 vector2 in godotArray)
            {
                result.Add(vector2);
            }
            return result;
        }

        // Еще одна функция для перевода из списка годот в нужный нам от C#
        private List<Vector2> FromVector2ArrayToList(Vector2[] godotArray) 
        {
            List<Vector2> result = new List<Vector2>();
            foreach (Vector2 vector2 in godotArray)
            {
                result.Add(vector2);
            }
            return result;
        }

        // Вспомогательная функция, чтобы выровнять что-то по сетке тайла
        public Vector2 GetClosestPosition(Vector2 to) // Например, чтобы персонаж находился по центру ячейки
        {
            return astar.GetPointPosition(astar.GetClosestPoint(to));
        }

        // Вспомогательная функция для получение ближайщей позиции из массива
        public Vector2 GetClosestPositionFromList(Vector2 to, List<Vector2> listv) // Например, чтобы найти точку ближайщую к персонажу изходя из его радиуса движения
        {
            Vector2 ClosestPosition = listv[0];

            foreach (Vector2 pos in listv)
                if (pos.DistanceTo(to) < ClosestPosition.DistanceTo(to))
                    ClosestPosition = pos;
            
            return ClosestPosition;
        }
    }   
}