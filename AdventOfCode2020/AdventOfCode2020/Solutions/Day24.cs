using AdventOfCode2020.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
#if DEBUG
using System.Diagnostics;
#endif


namespace AdventOfCode2020.Solutions
{
    public class Day24 : IPuzzle
    {
        public string CalculateSolution(Parts part, string inputData)
        {
            var directionsList = inputData.Split(Environment.NewLine);

            switch (part)
            {
                case Parts.Part1:
                    return SolvePart1(directionsList).ToString();

                case Parts.Part2:
                    return SolvePart2(directionsList, 100).ToString();

                default:
                    throw new ApplicationException($"Invalid parameter {nameof(part)} value ({part})");
            }
        }

        private static int SolvePart1(IEnumerable<string> directionsList)
        {
            var tiles = new Dictionary<(int x, int y), bool>();
            foreach (var direction in directionsList)
            {
                Debug.WriteLine(direction);
                var hexTileCoords = LobbyLayout.TrackHexTileCoords(direction);
                Debug.WriteLine(hexTileCoords);


                if (tiles.ContainsKey(hexTileCoords))
                {
                    tiles[hexTileCoords] = !tiles[hexTileCoords];
                }
                else
                {
                    tiles.Add(hexTileCoords, true);
                }
            }

            return tiles.Count(tile => tile.Value);
        }

        private static int SolvePart2(string[] directionsList, int daysCount)
        {
            var hexTiles = new Dictionary<(int x, int y), bool>();
            foreach (var direction in directionsList)
            {
#if DEBUG
                Debug.WriteLine(direction);
#endif
                var hexTileCoords = LobbyLayout.TrackHexTileCoords(direction);
#if DEBUG
                Debug.WriteLine(hexTileCoords);
#endif

                if (hexTiles.ContainsKey(hexTileCoords))
                {
                    hexTiles[hexTileCoords] = !hexTiles[hexTileCoords];
                }
                else
                {
                    hexTiles.Add(hexTileCoords, true);
                }
            }

            var flippedTiles = hexTiles;
            var blackTilesCount = hexTiles.Count(tile => tile.Value);
            for (var day = 0; day < daysCount; day++)
            {
                flippedTiles = LobbyLayout.MakeDailyFlip(flippedTiles);
                blackTilesCount = flippedTiles.Count(tile => tile.Value);
#if DEBUG
                Debug.WriteLine($"Black tiles count on day {day:3}: {blackTilesCount}");
#endif
            }
            



            
            return blackTilesCount;
        }


/* *************************************************************
 *   / \ /0\ /1\ /2\ /3\ /
 *  |   | -1| -1| -1| -1|        ?: m = y % 2, n = (y+1) % 2 
 *   \ / \ / \ / \ / \ / \      NW: x = x - m, y = y - 1 
 *    |   |0,0|1,0|2,0|3,0|      W: x = x - 1, y = y - 0 
 *   / \ / \ / \ / \ / \ /      SW: x = x + m, y = y + 1
 *  |   |0,1|1,1|2,1|3,1|       NE: x = x + n, y = y - 1  
 *   \ / \ / \ / \ / \ / \       E: x = x + 1, y = y + 0
 *    |   |0,2|1,2|2,2|3,2|     SE: x = x + n, y = y + 1
 *   / \ / \ / \ / \ / \ / \     
 *      |0,3|1,3|2,3|3,3|4,3|
 ************************************************************** */

        private static class LobbyLayout
        {
            private static readonly string[] Directions = { "nw", "w", "sw", "ne", "e", "se" };

            public static (int x, int y) TrackHexTileCoords(string path)
            {
                foreach (var dir in Directions)
                {
                    path = Regex.Replace(path, dir + "(?![\\s])", dir + " ");
                }

                var pathDirections = path.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var x = 0;
                var y = 0;
                foreach (var direction in pathDirections)
                {
                    int mod;
                    switch (direction)
                    {
                        case "nw":
                            mod = Math.Abs(y % 2);
                            x -= mod;
                            y--;
                            break;
                        case "w":
                            x--;
                            break;
                        case "sw":
                            mod = Math.Abs(y % 2);
                            x -= mod;
                            y++;
                            break;
                        case "ne":
                            mod = Math.Abs((y + 1) % 2);
                            x += mod;
                            y--;
                            break;
                        case "e":
                            x++;
                            break;
                        case "se":
                            mod = Math.Abs((y + 1) % 2);
                            x += mod;
                            y++;
                            break;
                        default:
                            throw new ArgumentException($"Invalid direction value: {direction}");
                    }
                }

                return (x, y);
            }

            public static Dictionary<(int x, int y), bool> MakeDailyFlip(IReadOnlyDictionary<(int x, int y), bool> hexTiles)
            {
                var x0 = hexTiles.Min(tile => tile.Key.x) - 1;
                var y0 = hexTiles.Min(tile => tile.Key.y) - 1;
                var xMax = hexTiles.Max(tile => tile.Key.x) + 1;
                var yMax = hexTiles.Max(tile => tile.Key.y) + 1;

                var flippedTiles = new Dictionary<(int x, int y), bool>();

                for (var y = y0; y <= yMax; y++)
                {
                    for (var x = x0; x <= xMax; x++)
                    {
                        var currentTileCoords = (x, y);
                        var adjacentBlackTilesCount = 0;
                        var mod0 = Math.Abs(y % 2);
                        var mod1 = Math.Abs((y + 1) % 2);
                        var adjacentTiles = new List<(int x, int y)>()
                        {
                            (x - mod0, y - 1), 
                            (x - 1, y),
                            (x - mod0, y + 1),
                            (x + mod1, y - 1),
                            (x + 1, y),
                            (x + mod1, y + 1),
                        };

                        foreach (var tileCoords in adjacentTiles)
                        {
                            if (hexTiles.ContainsKey(tileCoords) && hexTiles[tileCoords])
                            {
                                adjacentBlackTilesCount++;
                            }
                        }

                        // black tile
                        if (hexTiles.ContainsKey(currentTileCoords) && hexTiles[currentTileCoords])
                        {
                            if (0 < adjacentBlackTilesCount && adjacentBlackTilesCount <= 2)
                                flippedTiles.Add(currentTileCoords, true);
                            // ignore white tiles
                            //else 
                            //    flippedTiles.Add(currentTileCoords, false);
                        }
                        // white tile
                        else
                        {
                            if (adjacentBlackTilesCount == 2)
                                flippedTiles.Add(currentTileCoords, true);
                            // ignore white tiles
                            //else
                            //    flippedTiles.Add(currentTileCoords, false);
                        }
                    }
                }

                return flippedTiles;
            }
        }
        
    }

}
