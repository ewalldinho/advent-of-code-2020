using AdventOfCode2020.Solutions;
using System;
using System.Linq;
using System.Reflection;

namespace AdventOfCode2020.Utils
{
    public static class PuzzleSolutionFactory
    {
        public static IPuzzle GetPuzzleSolution(int day)
        {
            if (day < 1 || day > 25)
                throw new ArgumentException($"Argument {nameof(day)} must be in range [1..25]");

            var className = $"Day{day:00}";
            var assembly = Assembly.GetExecutingAssembly();
            var solutionClass = assembly.GetTypes()
                .Where(t => t.IsClass).Where(t => t.GetInterfaces().Contains(typeof(IPuzzle)))
                .FirstOrDefault(t => t.Name == className);

            if (solutionClass != null)
            {
                return (IPuzzle)Activator.CreateInstance(solutionClass);
            }

            throw new ApplicationException($"There is no solution for day {day}");
        }
    }
}
