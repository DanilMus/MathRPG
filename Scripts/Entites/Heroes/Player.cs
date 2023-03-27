using System;
using MathRPG.Entities;


namespace MathRPG.Entities.Heroes
{
    public class Player: Entity
    {
        public override void _Ready()
        {
            InitializeVariables();
        }
    }
}