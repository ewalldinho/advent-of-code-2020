using AdventOfCode2020.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AdventOfCode2020.Solutions
{
    public class Day17 : IPuzzle
    {
        public string CalculateSolution(Parts part, string inputData)
        {
            string[] pattern = inputData.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
            const int cyclesCount = 6;


            switch (part)
            {
                case Parts.Part1:
                    
                    var game = new ConwayCubeGame(pattern);
                    for (var i = 0; i < cyclesCount; i++)
                    {
                        game.SimulateCycle();
                    }
                    var activeCubes = game.ActiveCubesCount;
                    return activeCubes.ToString();
                
                case Parts.Part2:

                    var hyperGame = new HypercubeGame(pattern);
                    for (var i = 0; i < cyclesCount; i++)
                    {
                        hyperGame.SimulateCycle();
                    }
                    var activeHypercubes = hyperGame.ActiveCubesCount;
                    return activeHypercubes.ToString();

                default:
                    throw new ApplicationException($"Invalid parameter {nameof(part)} value ({part})");

            }
        }

        
        
    }

    public class ConwayCubeGame
    {
        private HashSet<(int, int, int)> _cubes;

        public ConwayCubeGame(string[] initialPattern)
        {
            _cubes = initialPattern.SelectMany((line, row) => line.Select((c, col) => c == '#' ? (col, row, 0) : (-1, -1, -1)))
                .Where(coordinates => coordinates != (-1, -1, -1))
                .ToHashSet();
        }

        public int ActiveCubesCount => _cubes.Count;

        private bool IsActive(int x, int y, int z)
        {
            return _cubes.Contains((x, y, z));
        }

        private bool ShouldActivate(int x, int y, int z)
        {
            var isActive = IsActive(x, y, z);

            var neighborCount = 0;
            for (var i = x - 1; i <= x + 1; i++)
            {
                for (var j = y - 1; j <= y + 1; j++)
                {
                    for (var k = z - 1; k <= z + 1; k++)
                    {
                        if (i == x && j == y && k == z)
                            continue;
                        
                        if (_cubes.Contains((i, j, k)))
                        {
                            neighborCount++;
                        }
                    }
                }
            }

            switch (isActive)
            {
                case true when 2 <= neighborCount && neighborCount <= 3:
                case false when neighborCount == 3:
                    return true;
                default:
                    return false;
            }
        }

        private SpaceCoordsRanges GetRanges()
        {
            var ranges = new SpaceCoordsRanges()
            {
                XMin = _cubes.Min(c => c.Item1),
                XMax = _cubes.Max(c => c.Item1),
                YMin = _cubes.Min(c => c.Item2),
                YMax = _cubes.Max(c => c.Item2),
                ZMin = _cubes.Min(c => c.Item3),
                ZMax = _cubes.Max(c => c.Item3)
            };
            
            return ranges;
        }

        private SpaceCoordsRanges GetExpansionRanges()
        {
            var ranges = GetRanges();
            return new SpaceCoordsRanges
            {
                XMin = ranges.XMin - 1,
                XMax = ranges.XMax + 1,
                YMin = ranges.YMin - 1,
                YMax = ranges.YMax + 1,
                ZMin = ranges.ZMin - 1,
                ZMax = ranges.ZMax + 1
            };
        }

        public void SimulateCycle()
        {
            var newState = new HashSet<(int, int, int)>();

            var beforeCycle = ActiveCubesCount;

            var ranges = GetExpansionRanges();
            for (var x = ranges.XMin; x <= ranges.XMax; x++)
            {
                for (var y = ranges.YMin; y <= ranges.YMax; y++)
                {
                    for (var z = ranges.ZMin; z <= ranges.ZMax; z++)
                    {
                        if (ShouldActivate(x, y, z))
                        {
                            newState.Add((x, y, z));
                        }
                    }
                }
            }
            
            _cubes = newState;

            var afterCycle = ActiveCubesCount;

            Debug.WriteLine($"{beforeCycle} -> {afterCycle}");
        }

        public void Simulate(int cyclesCount)
        {
            for (var i = 0; i < cyclesCount; i++)
            {
                SimulateCycle();
            }
        }
    }


    public class SpaceCoordsRanges
    {
        public int XMin { get; set; }
        public int XMax { get; set; }

        public int YMin { get; set; }
        public int YMax { get; set; }

        public int ZMin { get; set; }
        public int ZMax { get; set; }
    }


    public class HypercubeGame
    {
        private HashSet<(int, int, int, int)> _cubes;

        public HypercubeGame(string[] initialPattern)
        {
            _cubes = initialPattern.SelectMany((line, row) => line.Select((c, col) => c == '#' ? (col, row, 0, 0) : (-1, -1, -1, -1)))
                .Where(coordinates => coordinates != (-1, -1, -1, -1))
                .ToHashSet();
        }

        public int ActiveCubesCount => _cubes.Count;

        private bool IsActive(int x, int y, int z, int w)
        {
            return _cubes.Contains((x, y, z, w));
        }

        private bool ShouldActivate(int x, int y, int z, int w)
        {
            var isActive = IsActive(x, y, z, w);

            var neighborCount = 0;
            for (var i = x - 1; i <= x + 1; i++)
            {
                for (var j = y - 1; j <= y + 1; j++)
                {
                    for (var k = z - 1; k <= z + 1; k++)
                    {
                        for (var m = w - 1; m <= w + 1; m++)
                        {
                            if (i == x && j == y && k == z && m == w)
                                continue;

                            if (_cubes.Contains((i, j, k, m)))
                            {
                                neighborCount++;
                            }
                        }
                    }
                }
            }

            switch (isActive)
            {
                case true when 2 <= neighborCount && neighborCount <= 3:
                case false when neighborCount == 3:
                    return true;
                default:
                    return false;
            }
        }

        private HyperSpaceCoordsRanges GetRanges()
        {
            var ranges = new HyperSpaceCoordsRanges()
            {
                XMin = _cubes.Min(c => c.Item1),
                XMax = _cubes.Max(c => c.Item1),
                YMin = _cubes.Min(c => c.Item2),
                YMax = _cubes.Max(c => c.Item2),
                ZMin = _cubes.Min(c => c.Item3),
                ZMax = _cubes.Max(c => c.Item3),
                WMin = _cubes.Min(c => c.Item4),
                WMax = _cubes.Max(c => c.Item4)
            };

            return ranges;
        }

        public void SimulateCycle()
        {
            var newState = new HashSet<(int, int, int, int)>();

            var beforeCycle = ActiveCubesCount;

            var ranges = GetRanges();
            for (var x = ranges.XMin-1; x <= ranges.XMax+1; x++)
            {
                for (var y = ranges.YMin-1; y <= ranges.YMax+1; y++)
                {
                    for (var z = ranges.ZMin-1; z <= ranges.ZMax+1; z++)
                    {
                        for (var w = ranges.WMin-1; w <= ranges.WMax+1; w++)
                        {
                            if (ShouldActivate(x, y, z, w))
                            {
                                newState.Add((x, y, z, w));
                            }
                        }
                    }
                }
            }

            _cubes = newState;

            var afterCycle = ActiveCubesCount;

            Debug.WriteLine($"{beforeCycle} -> {afterCycle}");
        }

    }


    public class HyperSpaceCoordsRanges : SpaceCoordsRanges
    {
        public int WMin { get; set; }
        public int WMax { get; set; }
    }


}
