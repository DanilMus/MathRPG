using Godot;
using System;

namespace MathRPG.Entities.Heroes
{
    public class FirstFriend : AnimatedSprite
    {
        [Signal]
        public delegate void MousePressed(); // Доп копия, т.к. в главная сцена не видит объекты этой сцены
    
        public void OnArea2DMouseEntered()
        {
            EmitSignal(nameof(MousePressed));
        }
    }
}