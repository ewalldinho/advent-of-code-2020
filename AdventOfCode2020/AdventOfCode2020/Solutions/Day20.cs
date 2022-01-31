using AdventOfCode2020.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2020.Solutions
{
    public class Day20 : IPuzzle
    {
        public string CalculateSolution(Parts part, string inputData)
        {
            var data = ParseInputData(inputData);

            switch (part)
            {
                case Parts.Part1:
                    return CalcPart1(data).ToString();

                case Parts.Part2:
                    return CalcPart2(data).ToString();

                default:
                    throw new ApplicationException($"Invalid parameter {nameof(part)} value ({part})");
            }
        }

        private long CalcPart1(List<ImageTile> tiles)
        {
            for (var t1 = 0; t1 < tiles.Count - 1; t1++)
            {
                for (var t2 = t1 + 1; t2 < tiles.Count; t2++)
                {
                    if (tiles[t1].CheckMatch(tiles[t2]))
                    {
                        tiles[t1].CompatibleTiles.Add(tiles[t2].Id);
                        tiles[t2].CompatibleTiles.Add(tiles[t1].Id);
                    }
                }
            }

            var cornerTiles = tiles.Where(tile => tile.CompatibleTiles.Count == 2);
            var multipliedCornerTilesIds = cornerTiles.Aggregate(1L, (product, tile) => product * tile.Id);
            return multipliedCornerTilesIds;
        }

        private int CalcPart2(List<ImageTile> tiles)
        {
            return 0;
        }

        private static List<ImageTile> ParseInputData(string inputData)
        {
            var labeledTiles = inputData.Split(Environment.NewLine + Environment.NewLine);
            var imageTiles = labeledTiles.Select(tile => ImageTile.Parse(tile.Trim().Split(Environment.NewLine, 2)))
                .ToList();
            return imageTiles;
        }

        private class ImageTile
        {
            private const int Top = 0;
            private const int Left = 1;
            private const int Bottom = 2;
            private const int Right = 3;
            private const int TopReversed = 4;
            private const int LeftReversed = 5;
            private const int BottomReversed = 6;
            private const int RightReversed = 7;

            public int Id { get; set; }
            public char[][] MonochromeImage { get; set; }
            private int _rotation = 0; // 0 - 0, 1 - 90, 2 - 180, 3 - 270
            private bool _flipped = false;
            private readonly int _size;
            private readonly string[] _borders;

            public List<int> CompatibleTiles { get; set; } = new List<int>();

            private ImageTile(int id, char[][] monochromeImage)
            {
                Id = id;
                MonochromeImage = monochromeImage;
                _size = monochromeImage.Length;
                _borders = new string[8];
                _borders[Top] = new string(monochromeImage[0]);
                _borders[Bottom] = new string(monochromeImage[_size-1]);
                _borders[Right] = new string(monochromeImage.Select(line => line.Last()).ToArray());
                _borders[Left] = new string(monochromeImage.Select(line => line.First()).ToArray());
                _borders[TopReversed] = new string(_borders[Top].Reverse().ToArray());
                _borders[BottomReversed] = new string(_borders[Bottom].Reverse().ToArray());
                _borders[RightReversed] = new string(_borders[Right].Reverse().ToArray());
                _borders[LeftReversed] = new string(_borders[Left].Reverse().ToArray());
            }

            public static ImageTile Parse(IReadOnlyList<string> labeledTile)
            {
                var id = int.Parse(labeledTile[0][5..^1]);
                var monochromeSquare = labeledTile[1].Split(Environment.NewLine).Select(line => line.ToCharArray()).ToArray();
                var imageTile = new ImageTile(id, monochromeSquare);
                return imageTile;
            }

            public void Rotate()
            {
                _rotation = (_rotation + 1) % 4;
            }

            public void Flip()
            {
                _flipped = !_flipped;
            }

            public bool CheckMatch(ImageTile tile)
            {
                for (var tileBorder = Top; tileBorder <= RightReversed; tileBorder++)
                {
                    for (var tile2Border = Top; tile2Border <= Right; tile2Border++)
                    {
                        if (_borders[tileBorder] == tile._borders[tile2Border])
                            return true;
                    }
                }
                
                return false;
            }

        }
    }

}
