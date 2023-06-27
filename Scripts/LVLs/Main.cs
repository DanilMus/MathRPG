using Godot;
using System.Collections.Generic;

using MathRPG.Path;
using MathRPG.Health;
using MathRPG.Entities;
using MathRPG.Entities.Heroes;
using MathRPG.Entities.Enemies;
using MathRPG.MenuHUD;
using MathRPG.Attack;

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
                public PackedScene pathCellScene; // Переменная для хранения нашей сцены
                protected Node2D moveArea; // Для хранения сцен pathCellScene
                List<Vector2> lastMoveArea = new List<Vector2>(); // Сможем проверять, изменились ли возможности пути

            // Другие существа
                protected List<Entity> entities = new List<Entity>();
                protected List<Vector2> entitiesPositions = new List<Vector2>(); // Позиции других существ для отрисовки пути
        
            // Работа с катсценами
                protected AnimationPlayer cutscenes; // Для катсцены
                int cutsceneNum = 0; // Номер текущей катсцены
                string[] cutscenesNames; // Хранение названий катсцен

            // Работа с врагами
                protected List<Enemy> enemies = new List<Enemy>();

            // Работа со здоровьем
                protected HealthBar healthBar;
                protected SmallPotion smallPotion;
                protected BigPotion bigPotion;

            // Меню 
                public Menu menu;

            // Сцена боя
                public Fighting fighting;
                [Export]
                public PackedScene fightingScene;

        

        // Здесь происходит загрузка
        public override void _Ready() // Первая инициализация сцены
        {
            InitializeVariables();
        }
        protected virtual void InitializeVariables()
        {
            // Подключение навигации
            pathFinder = new PathFinder(GetNode<TileMap>("Ground"));

            // Подключение игрока и всем связанным с ним
            player = GetNode<Player>("Player");
            player.Position = pathFinder.GetClosestPosition(player.Position); // Прикрепление позиции игрока к сетке
            DrawMoveArea(pathFinder.GetAreaInRadius(player.Position , player.MoveRadius, entitiesPositions)); // Рисуем пути
            player.Connect("WasAttacked", this, nameof(OnPlayerWasAttacked));

            // Подключение катсцен
            cutscenes = GetNode<AnimationPlayer>("Cutscenes");
            cutscenesNames = cutscenes.GetAnimationList();

            // Подключение врагов
            var _enemies = GetNode<Node2D>("Enemies");
            foreach (Entity entity in _enemies.GetChildren())
            {
                entities.Add(entity);
                enemies.Add((Enemy)entity);
            }
            PrepareEntities();
            

            // Подключение здоровья и все связонного с ним
            healthBar = player.GetNode<CanvasLayer>("CanvasLayer").GetChild<HealthBar>(0);
            healthBar.UpdateHealthBar(player.Health, player.FullHeath);
            
            bigPotion = player.GetNode<CanvasLayer>("CanvasLayer").GetChild<BigPotion>(1);
            bigPotion.Connect("pressedWithHealCount", this, nameof(OnPotionButtonPressed));
            smallPotion = player.GetNode<CanvasLayer>("CanvasLayer").GetChild<SmallPotion>(2);
            smallPotion.Connect("pressedWithHealCount", this, nameof(OnPotionButtonPressed));

            // Подключение menu
            menu = player.GetNode<Menu>("Menu");
        }



        // Здесь обработка полученных нажатий
        public override void _Input(InputEvent @event)
        {
            if (player.IsAlive && @event is InputEventMouseButton && @event.IsPressed())
            {
                var mousePosition = GetGlobalMousePosition();
                if (player.Position.DistanceTo(mousePosition) <= player.MoveRadius * 32)
                {
                    CleanMoveArea();
                    SetPath(mousePosition, player);
                }
            }
        }
        


        // Отработка сигналов
        public void OnPlayerMovementDone() // Вызывается, когда игрок закончил движение
        {
            if (player.IsAlive)
                DrawMoveArea(pathFinder.GetAreaInRadius(player.Position , player.MoveRadius, entitiesPositions));
            else
                CleanMoveArea();
        }
        public void OnPlayerWasAttacked()
        {
            healthBar.UpdateHealthBar(player.Health, player.FullHeath);
        }
        public void OnEntityMovementDone()
        {
            if (player.IsAlive)
            {
                LoadEnitiesPositions();
                DrawMoveArea(pathFinder.GetAreaInRadius(player.Position , player.MoveRadius, entitiesPositions));
            }
            else 
                CleanMoveArea();
        }
        public void OnPotionButtonPressed(int heal)
        {
            player.Health += heal;
            healthBar.UpdateHealthBar(player.Health, player.FullHeath);
        }
        public void OnTimerForMoveTimeout() // Высчитвает время, чтобы враг сходил
        {
            foreach (NumEnemy enemy in enemies)
            {
                // Враг делает свой ход
                Vector2 move = enemy.Thinking(player.Position);
                SetPath(move, enemy);
            }
        }
        public void OnEnemyInputEvent()
        {

        }

        // Прочие функции
        public void SetPath(Vector2 whereGo, Entity whoGo)
        {
            LoadEnitiesPositions();
            var path = pathFinder.GetMovePathInRadius(whoGo.Position, whereGo, whoGo.MoveRadius, entitiesPositions);
            whoGo.Path = path;
        }
        protected void DrawMoveArea(List<Vector2> area)
        {
            // Проверка, чтобы не рисовать то же самое 
            if (area.Count == lastMoveArea.Count)
            {
                bool isTheSame = true;
                for (int i = 0; i < area.Count; i++)
                    if (area[i] != lastMoveArea[i])
                    {
                        isTheSame = false;
                        break;
                    }

                if (isTheSame)
                    return;
            }


            CleanMoveArea();
            lastMoveArea = area;

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
            foreach (Entity entity in entities)
                entitiesPositions.Add(entity.Position);
        }
        protected void PrepareEntities()
        {
            foreach (Entity entity in entities)
            {
                entity.Position = pathFinder.GetClosestPosition(entity.Position);
                entitiesPositions.Add(entity.Position);
                entity.Connect("MovementDone", this, nameof(OnEntityMovementDone));
            }    
        }
        protected void PrepareEnemies()
        {
            // foreach (Enemy entity in entities)
            //     entity.Connect("InputEvent", this, nameof)
        }
    }
}