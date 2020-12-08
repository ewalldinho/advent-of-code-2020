using AdventOfCode2020.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AdventOfCode2020.Solutions
{
    public class Day01 : IPuzzle
    {
        public string CalculateSolution(Parts part, string inputData)
        {
            var expenses = inputData.Split(Environment.NewLine).Select(int.Parse).ToList();

            switch (part)
            {
                case Parts.Part1:
                    var multiplicationOfTwo = FindProductOfTwo(expenses);
                    return multiplicationOfTwo.ToString();
                case Parts.Part2:
                    var multiplicationOfThree = FindProductOfThree(expenses);
                    return multiplicationOfThree.ToString();

                default:
                    throw new ApplicationException($"Invalid parameter {nameof(part)} value ({part})");
            }
        }

        private static int FindProductOfTwo(List<int> expenses)
        {
            for (var i = 0; i < expenses.Count - 1; i++)
            for (var j = i + 1; j < expenses.Count; j++)
                if (expenses[i] + expenses[j] == 2020)
                {
                    Debug.WriteLine($"n1 = {expenses[i]}");
                    Debug.WriteLine($"n2 = {expenses[j]}");
                    return expenses[i] * expenses[j];
                }

            throw new ApplicationException("Can not solve it!");
        }

        private static int FindProductOfThree(List<int> expenses)
        {
            for (var i = 0; i < expenses.Count - 2; i++)
            for (var j = i + 1; j < expenses.Count - 1; j++)
            for (var k = j + 1; k < expenses.Count; k++)
                if (expenses[i] + expenses[j] + expenses[k] == 2020)
                {
                    Debug.WriteLine($"n1 = {expenses[i]}");
                    Debug.WriteLine($"n2 = {expenses[j]}");
                    Debug.WriteLine($"n3 = {expenses[k]}");
                    return expenses[i] * expenses[j] * expenses[k];
                }

            throw new ApplicationException("Can not solve it!");
        }

	}
}
