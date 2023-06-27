namespace MathRPG.Attack.Logic.Sets
{
    public abstract class NumericSet
    {
        public static readonly string[] Strings =
        {
            "N",
            "Z",
            "Q",
            "R",
            "C"
        };
        
        public enum Values
        {
            Natural,
            Integer,
            Rational,
            Real,
            Complex
        }
    }
}