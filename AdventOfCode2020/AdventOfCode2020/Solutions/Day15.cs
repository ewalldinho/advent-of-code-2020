using AdventOfCode2020.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2020.Solutions
{
    public class Day15 : IPuzzle
    {
        public string CalculateSolution(Parts part, string inputData)
        {
            var startingNumbers = inputData.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);
            
            switch (part)
            {
                case Parts.Part1:
                    var number2020 = PlaySimpleGame(startingNumbers, 2020);
                    return number2020.ToString();

                case Parts.Part2:
                    var memGame = new MemoryGame();
                    var number30M = memGame.PlayGame(startingNumbers, 30_000_000);
                    return number30M.ToString();

                default:
                    throw new ApplicationException($"Invalid parameter {nameof(part)} value ({part})");
            }
        }

        private static int PlaySimpleGame(IEnumerable<int> numbers, int turnsCount)
        {
            var spokenNumbers = numbers.ToList();
            while (spokenNumbers.Count < turnsCount)
            {
                var lastNumber = spokenNumbers.Last();
                var lastSpokenTurn = spokenNumbers.LastIndexOf(lastNumber, spokenNumbers.Count - 2);
                if (lastSpokenTurn >= 0)
                {
                    var age = spokenNumbers.Count - 1 - lastSpokenTurn;
                    spokenNumbers.Add(age);
                }
                else
                {
                    spokenNumbers.Add(0);
                }
            }

            return spokenNumbers.Last();
        }
    }

    public class SpokenNumber
    {
        public int Value { get; set; }
        public int PrevTurnUsed { get; set; }
        public int LastTurnUsed { get; set; }
        public int MentionsCount { get; set; }
    }

    public class MemoryGame
    {
        private Dictionary<int, SpokenNumber> _spokenNumbers;
        private int _totalTurnsCount;
        private int _lastSpokenNumber;
        private int _currentTurn;

        public int PlayGame(IEnumerable<int> startingNumbers, int turnsCount)
        {
            Initialize(startingNumbers, turnsCount);

            while ( ! LastTurn())
            {
                var lastNumber = GetMostRecentlySpokenNumber();
                if (lastNumber.MentionsCount == 1)
                {
                    AddNumber(0);
                    _lastSpokenNumber = 0;
                }
                else
                {
                    var numberAge = CalculateAge(_lastSpokenNumber);
                    AddNumber(numberAge);
                    _lastSpokenNumber = numberAge;
                }

                _currentTurn++;
            }

            return _lastSpokenNumber;
        }

        private void AddNumber(int number)
        {
            if (_spokenNumbers.ContainsKey(number))
            {
                _spokenNumbers[number].MentionsCount++;
                _spokenNumbers[number].PrevTurnUsed = _spokenNumbers[number].LastTurnUsed;
                _spokenNumbers[number].LastTurnUsed = _currentTurn;
            }
            else
            {
                _spokenNumbers.Add(number, new SpokenNumber
                {
                    MentionsCount = 1,
                    LastTurnUsed = _currentTurn,
                    PrevTurnUsed = -1,
                    Value = number
                });
            }
        }

        private void Initialize(IEnumerable<int> startingNumbers, int turnsCount)
        {
            var numbers = startingNumbers as int[] ?? startingNumbers.ToArray();
            _spokenNumbers = numbers.Select((n, index) => new SpokenNumber
                {
                    Value = n,
                    LastTurnUsed = index + 1,
                    PrevTurnUsed = -1,
                    MentionsCount = 1
                })
                .ToDictionary(x => x.Value);

            _totalTurnsCount = turnsCount;
            _lastSpokenNumber = numbers.Last();
            _currentTurn = numbers.Length + 1;
        }

        private bool LastTurn()
        {
            return _currentTurn > _totalTurnsCount;
        }

        private SpokenNumber GetMostRecentlySpokenNumber()
        {
            return _spokenNumbers[_lastSpokenNumber];
        }

        private int CalculateAge(int number)
        {
            return _spokenNumbers[number].LastTurnUsed - _spokenNumbers[number].PrevTurnUsed;
        }

    }
    
}
