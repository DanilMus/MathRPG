using System;
using System.Text;

namespace MathRPG.Scripts.Logic.Sets
{
    public class ComplexNumber: Number
    {
        private string _value;
        private readonly NumericSet.Values _set = NumericSet.Values.Complex;

        public ComplexNumber()
        {
            StringBuilder stringBuilder = new StringBuilder();

            int x = Utilities.Random.RandomInt(RandomMax);
            int y = Utilities.Random.RandomInt(RandomMax);
            
            if (y > 0)
            {
                stringBuilder.Append($"{x} + {y}i");
            }
            else
            {
                y++;
                
                stringBuilder.Append($"{x} - {Math.Abs(y)}i");
            }

            _value = stringBuilder.ToString();
        }

        public override string ToString()
        {
            return _value;
        }

        public override bool Equals(object obj)
        {
            return obj is ComplexNumber && _value.Equals(( (ComplexNumber) obj)._value);
        }

        public override NumericSet.Values GetSet()
        {
            return _set;
        }
    }
}