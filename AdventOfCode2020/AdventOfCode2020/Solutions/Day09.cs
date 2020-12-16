using AdventOfCode2020.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2020.Solutions
{
    public class Day09 : IPuzzle
    {
        private const int PreambleSize = 25;

        public string CalculateSolution(Parts part, string inputData)
        {
            var numbers = inputData.Split(Environment.NewLine).Select(long.Parse).ToList();

            switch (part)
            {
                case Parts.Part1:
                    var invalidNumber = FindIncorrectNumber(numbers);
                    return invalidNumber.ToString();

                case Parts.Part2:
                    var checkSumNumber = FindIncorrectNumber(numbers);
                    var weakness = FindContiguousList(checkSumNumber, numbers);
                    return weakness.ToString();

                default:
                    throw new ApplicationException($"Invalid parameter {nameof(part)} value ({part})");
            }
            
        }

        private long FindIncorrectNumber(List<long> numbers)
        {
            for (var i = PreambleSize; i < numbers.Count; i++)
            {
                if (!CheckSum(numbers[i], numbers.GetRange(i - PreambleSize, PreambleSize)))
                {
                    return numbers[i];
                }
            }

            return 0;
        }

        private static bool CheckSum(long number, List<long> preamble)
        {
            for (var i = 0; i < preamble.Count-1; i++)
            for (var j = i + 1; j < preamble.Count; j++)
            {
                if (preamble[i] + preamble[j] == number)
                    return true;
            }

            return false;
        }

        private static long FindContiguousList(long checkSum, List<long> numbers)
        {
            for (var i = 0; i < numbers.Count; i++)
            {
                var tempSum = 0L;
                var len = 2;
                while (tempSum < checkSum)
                {
                    var contiguousRange = numbers.GetRange(i, len);
                    tempSum = contiguousRange.Sum();
                    if (tempSum == checkSum)
                    {
                        return contiguousRange.Min() + contiguousRange.Max();
                    }
                    len++;
                }
            }
            
            return 0;
        }
    }
}
