namespace MathRPG.Attack.Logic.Sets
{
    public class NaturalNumber: Number
    {
        protected int _value;
        private readonly NumericSet.Values _set = NumericSet.Values.Natural;

        public NaturalNumber()
        {
            _value = Utilities.Random.RandomPositiveInt(RandomMax); // Generates a value in range of (0; RandomMax)
        }
        
        public override string ToString()
        {
            return _value.ToString();
        }

        public override bool Equals(object obj)
        {
            return obj is NaturalNumber && _value == ( (NaturalNumber) obj)._value;
        }

        public override NumericSet.Values GetSet()
        {
            return _set;
        }
    }
}