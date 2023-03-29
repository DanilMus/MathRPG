namespace MathRPG.Entities.Heroes
{
    public class Player: Entity
    {
        
        public override void _Ready()
        {
            InitializeVariables();
            MoveRadius = 4;
        }
    }
}