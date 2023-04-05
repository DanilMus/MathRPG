using System;
using Godot;
using MathRPG.Entities.Heroes;

namespace MathRPG.Entities.Enemies
{
    public abstract class NumEnemy : Enemy
    {
        public override void _Ready()
        {
            InitializeVariables();
        }

        protected override void InitializeVariables()
        {
            base.InitializeVariables();
            MemoryPath = "res://Saves/NumEnemy";
        }
    }
}
