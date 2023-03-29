using System;
using MathRPG.Entities;

namespace MathRPG.Entities.Enemies
{
    public class Enemy : Entity
    {
        int _viewRadius;

        protected int ViewRadius
        {
            get => _viewRadius;
            set 
            {
                if (value < 0) throw new ArgumentException("ViewRadius value should be more than zero");

                _viewRadius = value;
            }
        }

        public override void _Ready()
        {
            InitializeVariables();
        }
    }
}
