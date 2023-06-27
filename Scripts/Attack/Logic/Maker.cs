using System;
using System.Collections.Generic;
using MathRPG.Attack.Logic.Sets;

namespace MathRPG.Attack.Logic
{
    public static class Maker
    {
        // TODO: Make fighting GUI work with different GenerateAmount values. For now, it can only work with 6
        private const int GenerateAmount = 6; // I do not recommend changing this, it can easily break everything
    
        public static (NumericSet.Values, List<Number>) CreateSetTask(bool isComplexAvailable)
        {
            List<Number> numbers = new List<Number>();

            while (numbers.Count < GenerateAmount)
            {
                Number number = GenerateNumber(isComplexAvailable);
                if (!numbers.Contains(number)) numbers.Add(number);
            }

            NumericSet.Values toFind = PickRandomSet(numbers, isComplexAvailable);

            return (toFind, numbers);
        }

        private static NumericSet.Values PickRandomSet(List<Number> numbers, bool isComplexAvailable)
        {
            List<NumericSet.Values> sets = new List<NumericSet.Values>();

            foreach (Number number in numbers)
            {
                if (number.GetType() == typeof(IntegerNumber))
                {
                    if (!sets.Contains(NumericSet.Values.Integer)) sets.Add(NumericSet.Values.Integer);
                }
                else if (number.GetType() == typeof(NaturalNumber))
                {
                    if (!sets.Contains(NumericSet.Values.Natural)) sets.Add(NumericSet.Values.Natural);
                }
                else if (number.GetType() == typeof(RationalNumber))
                {
                    if (!sets.Contains(NumericSet.Values.Rational)) sets.Add(NumericSet.Values.Rational);
                }
                else if (number.GetType() == typeof(RealNumber))
                {
                    if (!sets.Contains(NumericSet.Values.Real)) sets.Add(NumericSet.Values.Real);
                }
                else
                {
                    if (isComplexAvailable && !sets.Contains(NumericSet.Values.Complex)) sets.Add(NumericSet.Values.Complex);
                }
            }

            return sets[ Math.Abs( Utilities.Random.RandomInt(sets.Count) ) ];
        }

        private static Number GenerateNumber(bool isComplexAvailable)
        {
            int divider = 4;

            if (isComplexAvailable) divider++;
        
            switch (Math.Abs(Utilities.Random.RandomInt()) % divider)
            {
                case 0: 
                    return new NaturalNumber();
            
                case 1:
                    return new IntegerNumber();
            
                case 2:
                    return new RationalNumber();
            
                case 3:
                    return new RealNumber();
            
                case 4:
                    return new ComplexNumber();
            }

            return null;
        }
    }
}
