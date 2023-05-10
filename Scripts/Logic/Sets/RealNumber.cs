using System;
using System.Text;

namespace MathRPG.Scripts.Logic.Sets
{
    public class RealNumber : Number
    {
        private readonly NumericSet.Values _set = NumericSet.Values.Real;

        public static string[] _specialValues =
        {
            "π",
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
            
            int integerPart = Utilities.Random.RandomInt(RandomMax) + 1;
            int fractionalPartLength = Math.Abs(Utilities.Random.RandomInt(RandomMax / 20) + 1);

            stringBuilder.Append(integerPart.ToString());
            stringBuilder.Append(".");
            for (int i = 0; i < fractionalPartLength; i++)
            {
                stringBuilder.Append(Utilities.Random.RandomInt(0, 10));
            }

            stringBuilder = new StringBuilder(DeleteZeros(stringBuilder.ToString()));

            stringBuilder.Append("...");

            return stringBuilder.ToString();
        }

        private string DeleteZeros(string value)
        {
            StringBuilder stringBuilder = new StringBuilder(value);

            int zeroCount = 0;
            char[] array = stringBuilder.ToString().ToCharArray();
            for (int i = stringBuilder.ToString().Length - 1; ; i--)
            {
                if (array[i] == '0') zeroCount++;
                else break;
            }

            stringBuilder.Remove(stringBuilder.ToString().Length - zeroCount, zeroCount);

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