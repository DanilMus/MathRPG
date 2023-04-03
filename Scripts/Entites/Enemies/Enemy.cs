using System;
using Godot;
using MathRPG.Entities.Heroes;

namespace MathRPG.Entities.Enemies
{
    public class Enemy : Entity
    {
        Area2D _area;
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
        protected Area2D Area
        {
            get => _area;
            set
            {
                if (value == null) throw new ArgumentNullException();

                _area = value;
            }
        }

        public override void _Ready()
        {
            InitializeVariables();
        }

        protected override void InitializeVariables()
        {
            base.InitializeVariables();
            Area = GetNode<Area2D>("Area2D");
        }

        public void Killing(Node body)
        {
            if (body is Hero)
                AnimatedSprite.Play("kills");
        }
    }
}
