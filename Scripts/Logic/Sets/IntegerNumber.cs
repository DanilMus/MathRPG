namespace MathRPG.Scripts.Logic.Sets
{
    public class IntegerNumber: NaturalNumber
    {
        private readonly NumericSet.Values _set = NumericSet.Values.Integer;
        
        public IntegerNumber()
        {
            _value = Utilities.Random.RandomInt(RandomMax); // Generates a number in range of (-RandomMax; RandomMax)
        }
        
        public override NumericSet.Values GetSet()
        {
            return _set;
        }
    }
}