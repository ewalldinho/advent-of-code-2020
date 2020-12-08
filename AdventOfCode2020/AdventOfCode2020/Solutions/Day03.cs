using AdventOfCode2020.Utils;
using System;
using System.Collections.Generic;

namespace AdventOfCode2020.Solutions
{
    public class Day03 : IPuzzle
    {
        private const char OpenSquare = '.';
        private const char TreeSquare = '#';

        public string CalculateSolution(Parts part, string inputData)
        {
            var treeMap = inputData.Split(Environment.NewLine);
            
            switch (part)
            {
                case Parts.Part1:
                    var slope1 = new Position(3, 1);
                    var encounteredTreesCount = CalculateTreesDownTheSlope(treeMap, slope1.X, slope1.Y);
                    return encounteredTreesCount.ToString();
                case Parts.Part2:
                    var slopes = new List<Position>()
                    {
                        new Position(1, 1),
                        new Position(3, 1),
                        new Position(5, 1),
                        new Position(7, 1),
                        new Position(1, 2),
                    };

                    var encounteredTreesMul = 1L;
                    var list = new List<int>();
                    foreach (var slope in slopes)
                    {
                        var encounteredTrees = CalculateTreesDownTheSlope(treeMap, slope.X, slope.Y);
                        list.Add(encounteredTrees);
                        encounteredTreesMul *= encounteredTrees;
                    }

                    return encounteredTreesMul.ToString() + " = " + string.Join(" × ", list); 

                default:
                    throw new ApplicationException($"Invalid parameter {nameof(part)} value ({part})");
            }
        }

        private static int CalculateTreesDownTheSlope(IReadOnlyList<string> treeMap, int slopeStepsRight, int slopeStepsDown)
        {
            var totalColumns = treeMap[0].Length;
            var totalRows = treeMap.Count;
            var currentPosition = new Position(0, 0);
            var treesCount = 0;

            while (currentPosition.Y < totalRows)
            {
                var x = currentPosition.X % totalColumns;

                switch (treeMap[currentPosition.Y][x])
                {
                    case OpenSquare:
                        treesCount += 0;
                        break;
                    case TreeSquare:
                        treesCount += 1;
                        break;
                }

                currentPosition.Move(slopeStepsRight, slopeStepsDown);
            }

            return treesCount;
        }

    }


    public class Position
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }

        public void Move(int dx, int dy)
        {
            X += dx;
            Y += dy;
        }
    }
}
