namespace MathRPG.Scripts.Logic.Sets
{
    public class IntegerNumber: NaturalNumber
    {
        private readonly NumericSet.Values _set = NumericSet.Values.Integer;
        
        public IntegerNumber()
        {
            _value = Utilities.Random.RandomNegativeInt(RandomMax); // Generates a number in range of (-RandomMax; 1)
        }
        
        public override NumericSet.Values GetSet()
        {
            return _set;
        }
    }
}