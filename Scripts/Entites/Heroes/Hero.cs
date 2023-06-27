using Godot;

using MathRPG.Entities.Enemies;

namespace MathRPG.Entities.Heroes
{
    public abstract class Hero: Entity
    {
        public void Injured(Entity body)
        {
            if (body is Enemy)
            {
                Health -= body.Damage;
                EmitSignal(nameof(WasAttacked));
            }
        }
    }
}