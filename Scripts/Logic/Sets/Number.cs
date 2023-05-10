namespace MathRPG.Scripts.Logic.Sets
{
    public abstract class Number
    {
        protected const int RandomMax = 100;

        public abstract NumericSet.Values GetSet();

        public abstract override bool Equals(object obj);
    }
}
