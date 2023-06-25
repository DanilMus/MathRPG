using System;
using System.Text;

namespace MathRPG.Scripts.Logic.Sets
{
    public class RealNumber : Number
    {
        private readonly NumericSet.Values _set = NumericSet.Values.Real;

        public static string[] _specialValues =
        {
            // It can not display Pi symbol lol
            "3,1415...",
            "e" 
        };
        
        private string _value;
        
        public RealNumber()
        {
            _value = Utilities.Random.RandomPositiveInt() % 2 == 0
                ? _specialValues[Utilities.Random.RandomPositiveInt() % 2]
                : GenerateValue();
        }

        // TODO: Check this method, i do not think it works right
        private string GenerateValue()
        {
            StringBuilder stringBuilder = new StringBuilder();
            
            int integerPart = Utilities.Random.RandomInt(RandomMax);
            int fractionalPartLength = integerPart < 0 ? 3 : 4;

            stringBuilder.Append(integerPart.ToString());
            stringBuilder.Append(".");
            for (int i = 0; i < fractionalPartLength; i++)
            {
                stringBuilder.Append(Utilities.Random.RandomInt(1, 10));
            }

            stringBuilder = new StringBuilder(stringBuilder.ToString());

            stringBuilder.Append("...");

            return stringBuilder.ToString();
        }

        public override string ToString()
        {
            return _value;
        }

        public override bool Equals(object obj)
        {
            return obj is RealNumber && ( (RealNumber) obj)._value.Equals(_value);
        }

        public override NumericSet.Values GetSet()
        {
            return _set;
        }
    }
}