using AdventOfCode2020.Utils;
using System;
using System.Linq;
using System.Text;

namespace AdventOfCode2020.Solutions
{
    public class Day11 : IPuzzle
    {
        public string CalculateSolution(Parts part, string inputData)
        {
            var seats = inputData.Split(Environment.NewLine);

            var waitingArea = new WaitingArea(seats);
            bool wasChanged;
            do
            {
                wasChanged = waitingArea.RunSeatingRound(part);
            }
            while (wasChanged);

            return waitingArea.OccupiedSeatsCount.ToString();
        }
    }

    public class WaitingArea
    {
        private const char Floor = '.';
        private const char SeatEmpty = 'L';
        private const char SeatOccupied = '#';

        private string[] _seats;
        private readonly int _width;
        private readonly int _length;

        public WaitingArea(string[] seats)
        {
            _seats = seats;
            _length = _seats.Length;
            _width = _seats[0].Length;
        }

        public int OccupiedSeatsCount
        {
            get
            {
                return _seats.Sum(row => row.Count(s => s == SeatOccupied));
            }
        }
        

        public bool RunSeatingRound(Parts part)
        {
            var newSeating = new string[_seats.Length];
            var seatingChanged = false;

            for (var row = 0; row < _seats.Length; row++)
            {
                var newRow = new StringBuilder();
                for (var col = 0; col < _seats[row].Length; col++)
                {
                    switch (_seats[row][col])
                    {
                        case Floor:
                            newRow.Append(Floor);
                            break;
                        case SeatEmpty:
                        case SeatOccupied:
                            var newValue = part switch
                            {
                                Parts.Part1 => CalculateNewSeatValueTheoretical(row, col),
                                Parts.Part2 => CalculateNewSeatValueFactual(row, col),
                                _ => Floor
                            };
                            
                            if (newValue != _seats[row][col])
                            {
                                seatingChanged = true;
                            }
                            newRow.Append(newValue);
                            break;
                    }
                }

                newSeating[row] = newRow.ToString();
            }

            _seats = newSeating;
            return seatingChanged;
        }

        private char CalculateNewSeatValueTheoretical(int row, int col)
        {
            if (_seats[row][col] == Floor)
                return Floor;

            var takenAdjacentSeats = 0;
            for (var r = row - 1; r <= row + 1; r++)
            {
                if (r < 0 || _length <= r)
                    continue;

                for (var c = col - 1; c <= col + 1; c++)
                {
                    if (c < 0 || _width <= c || (r == row && c == col))
                        continue;
                    
                    if (_seats[r][c] == SeatOccupied)
                        takenAdjacentSeats++;
                }
            }

            return _seats[row][col] switch
            {
                SeatEmpty when takenAdjacentSeats == 0 => SeatOccupied,
                SeatOccupied when takenAdjacentSeats >= 4 => SeatEmpty,
                _ => _seats[row][col]
            };
        }

        private char CalculateNewSeatValueFactual(int row, int col)
        {
            if (_seats[row][col] == Floor)
                return Floor;

            var takenVisibleSeats = 0;
            for (var dirRow = -1; dirRow <= 1; dirRow++)
            {
                for (var dirCol = -1; dirCol <= 1; dirCol++)
                {
                    if (dirRow == 0 && dirCol == 0)
                        continue;

                    var firstSeat = FindFirstVisibleSeat(row, col, dirRow, dirCol);
                        
                    if (firstSeat == SeatOccupied)
                        takenVisibleSeats++;
                }
            }

            return _seats[row][col] switch
            {
                SeatEmpty when takenVisibleSeats == 0 => SeatOccupied,
                SeatOccupied when takenVisibleSeats >= 5 => SeatEmpty,
                _ => _seats[row][col]
            };
        }

        private char FindFirstVisibleSeat(int row, int col, int directionRow, int directionCol)
        {
            var rowIndex = row + directionRow;
            var colIndex = col + directionCol;
            while (0 <= rowIndex && rowIndex < _length && 0 <= colIndex && colIndex < _width)
            {
                if (_seats[rowIndex][colIndex] != Floor)
                    return _seats[rowIndex][colIndex];

                rowIndex += directionRow;
                colIndex += directionCol;
            }

            return Floor;
        }
    }
}
