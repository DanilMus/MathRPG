using System;
using Godot;
using MathRPG.Entities.Heroes;

namespace MathRPG.Entities.Enemies
{
    public abstract class NumEnemy : Enemy
    {
        // Загрузка данного класса врага
        public override void _Ready()
        {
            InitializeVariables();
        }
        protected override void InitializeVariables()
        {
            MemoryPath = "res://Saves/NumEnemy";
            base.InitializeVariables();
        }
    }
}
