
namespace MathRPG.Entities.Heroes
{
	public class Player: Hero
	{
		public override void _Ready()
		{
			InitializeVariables();
			MoveRadius = 4;
			Damage = 100;
		}
	}
}
