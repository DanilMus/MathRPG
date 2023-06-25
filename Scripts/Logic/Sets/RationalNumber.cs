using System;
using System.Text;

namespace MathRPG.Scripts.Logic.Sets
{
    public class RationalNumber: Number
    {
        private readonly NumericSet.Values _set = NumericSet.Values.Rational;
        
        private int _numerator;
        private int _denominator;

        // TODO: It works better now, but i think it still needs to be improved
        public RationalNumber()
        {
            while (!CheckValue())
            {
                _numerator = Utilities.Random.RandomInt(RandomMax); // Generates a value in range of (-RandomMax; RandomMax)
                _denominator = Math.Abs(Utilities.Random.RandomInt(RandomMax));
            }
        }
        
        private bool CheckValue() // returns false if the value is wrong
        {
            if (_denominator == 0) return false;
            if (_numerator == 0) return false;
            if (_denominator <= _numerator) return false;
            if (_numerator % _denominator == 0) return false;

            return true;
        }
        
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            
            stringBuilder.Append(_numerator);
            stringBuilder.Append(" / ");
            stringBuilder.Append(_denominator);
            
            return stringBuilder.ToString();
        }

        public override bool Equals(object obj)
        {
            return obj is RationalNumber && ((RationalNumber)obj)._numerator == _numerator &&
                   ((RationalNumber)obj)._denominator == _denominator;
        }

        public override NumericSet.Values GetSet()
        {
            return _set;
        }
    }
}