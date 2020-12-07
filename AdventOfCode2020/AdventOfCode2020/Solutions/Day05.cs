using AdventOfCode2020.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2020.Solutions
{
    public static class Day05
    {
        public static string CalculateSolution(Parts part, string inputData)
        {
            var seatDescriptions = inputData.Split(Environment.NewLine);

            var seatIds = seatDescriptions.Select(seat => CalculateSeatId(seat));

            switch (part)
            {
                case Parts.Part1:
                    var maxSeatId = seatIds.Max();
                    return maxSeatId.ToString();
                case Parts.Part2:
                    var availableSeatIds = new List<int>();
                    for (var seatId = TotalRows; seatId < TotalRows * (TotalColumns - 1); seatId++)
                    {
                        if (!seatIds.Contains(seatId))
                            availableSeatIds.Add(seatId);
                    }
                    return string.Join(", ", availableSeatIds);

                default:
                    throw new ApplicationException($"Invalid parameter {nameof(part)} value ({part})");
            }
        }

        /*
         * A seat might be specified like FBFBBFFRLR, where F means "front", B means "back", L means "left", and R means "right".
         *
         * The first 7 characters will either be F or B; these specify exactly one of the 128 rows on the plane (numbered 0 through 127).
         * Each letter tells you which half of a region the given seat is in. Start with the whole list of rows; the first letter
         * indicates whether the seat is in the front (0 through 63) or the back (64 through 127). The next letter indicates which
         * half of that region the seat is in, and so on until you're left with exactly one row.
         */

        private const int TotalRows = 128;
        private const int TotalColumns = 8;
        private static int CalculateSeatId(string seat)
        {
            var rowDef = seat.Substring(0, 7);
            var columnDef = seat.Substring(7, 3);
            var rowMin = 0;
            var rowMax = TotalRows - 1;
            foreach (var letter in rowDef)
            {
                var difference = rowMax - rowMin + 1;
                switch (letter)
                {
                    case 'F':
                        rowMax -= difference / 2;
                        break;
                    case 'B':
                        rowMin += difference / 2; 
                        break;
                    default:
                        throw new ArgumentException($"Invalid argument value: {seat}");
                }
            }

            var colMin = 0;
            var colMax = TotalColumns - 1;
            foreach (var letter in columnDef)
            {
                var difference = colMax - colMin + 1;
                switch (letter)
                {
                    case 'L':
                        colMax = colMax - difference / 2;
                        break;
                    case 'R':
                        colMin = colMin + difference / 2;
                        break;
                    default:
                        throw new ArgumentException($"Invalid argument value: {seat}");
                }
            }

            if (rowMin == rowMax && colMin == colMax)
            {
                return rowMin * TotalColumns + colMin;
            }

            throw new ApplicationException($"Invalid seat description");
        }
    }
}
