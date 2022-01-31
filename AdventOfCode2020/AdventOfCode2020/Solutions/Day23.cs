using AdventOfCode2020.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
#if DEBUG
using System.Diagnostics;
#endif

namespace AdventOfCode2020.Solutions
{
    public class Day23 : IPuzzle
    {
        public string CalculateSolution(Parts part, string inputData)
        {
            var cupLabels = inputData.ToCharArray().Select(c => c.ToString()).Select(int.Parse).ToArray();
            switch (part)
            {
                case Parts.Part1:
                    var movesCount = 100;
                    var useLinkedList = true;
                    var finalCupsOrder = useLinkedList ? PlayCrabCupsUsingLinkedList(cupLabels, movesCount) : PlayCrabCups(cupLabels, movesCount);
                    var indexOf1 = finalCupsOrder.IndexOf(1);
                    var result = finalCupsOrder.Skip(indexOf1 + 1).ToList();
                    result.AddRange(finalCupsOrder.Take(indexOf1));
                    return string.Join("", result);

                case Parts.Part2:
                    var movesCountPart2 = 10000000;
                    return PlayCrabCupsPart2(cupLabels, 10);

                default:
                    throw new ApplicationException($"Invalid parameter {nameof(part)} value ({part})");
            }
        }

        private static List<int> PlayCrabCups(IEnumerable<int> cupLabels, int movesCount)
        {
            var cupsCircle = new CupCircle(cupLabels);
#if DEBUG
            Debug.WriteLine(cupsCircle.ToString());
#endif

            for (var move = 0; move < movesCount; move++)
            {
                cupsCircle.MakeAMove();
#if DEBUG
                Debug.WriteLine(cupsCircle.ToString());
#endif
            }

            return cupsCircle.GetCurrentCupsOrder();
        }

        private static List<int> PlayCrabCupsUsingLinkedList(IEnumerable<int> cupLabels, int movesCount)
        {
            var cupsCircle = new CupLinkedCircle(cupLabels);
#if DEBUG
            Debug.WriteLine(cupsCircle.ToString());
#endif

            for (var move = 0; move < movesCount; move++)
            {
                cupsCircle.MakeAMove();
#if DEBUG
                Debug.WriteLine(cupsCircle.ToString());
#endif
            }

            return cupsCircle.GetCurrentCupsOrder();
        }

        private static string PlayCrabCupsPart2(IEnumerable<int> cupLabels, int movesCount)
        {
            var memos = new Dictionary<string, int>();

            var cupsCircle = new CupLinkedCircle(cupLabels);
#if DEBUG
            Debug.WriteLine(cupsCircle.ToString());
#endif

            for (var move = 0; move < movesCount; move++)
            {
                var key = cupsCircle.ToString();
#if DEBUG
                Debug.WriteLine(key);
#endif
                if (!memos.ContainsKey(key))
                {
                    memos.Add(key, move);
                }
                else
                {
                    var movesLoopLength = move - memos[key];
                    Debug.WriteLine($"Moves loop: {movesLoopLength}");
                    break;
                }

                cupsCircle.MakeAMove();
            }

            return "0";
        }


        private class CupCircle
        {
            private int _currentIndex;
            private readonly List<int> _cups;

            public CupCircle(IEnumerable<int> cupLabels)
            {
                _cups = new List<int>(cupLabels);
                _currentIndex = 0;
            }

            public void MakeAMove()
            {
                var currentCup = _cups[_currentIndex];
                var pickedCups = Pick3Cups();
                // current cup might have moved
                _currentIndex = _cups.IndexOf(currentCup);
                var destinationCupIndex = PickDestinationCupIndex();
                _cups.InsertRange(destinationCupIndex + 1, pickedCups);
                // current cup might have moved
                _currentIndex = _cups.IndexOf(currentCup);
                _currentIndex = (_currentIndex + 1) % _cups.Count;
            }

            private int GetCurrentCup()
            {
                return _cups[_currentIndex];
            }

            public int GetNext()
            {
                _currentIndex = (_currentIndex + 1) % _cups.Count;
                return _cups[_currentIndex];
            }

            private IEnumerable<int> Pick3Cups()
            {
                var trio = new List<int>();
                var index = _currentIndex + 1;
                for (var i = 0; i < 3; i++)
                {
                    if (index >= _cups.Count)
                        index = 0;

                    trio.Add(_cups[index]);
                    _cups.RemoveAt(index);
                }
                return trio;
            }

            private int PickDestinationCupIndex()
            {
                var destinationCup = GetCurrentCup() - 1;
                while (_cups.IndexOf(destinationCup) == -1)
                {
                    destinationCup--;
                    if (destinationCup == 0)
                        destinationCup = _cups.Max();
                }
                var destinationCupIndex = _cups.IndexOf(destinationCup);
                return destinationCupIndex;
            }

            public List<int> GetCurrentCupsOrder()
            {
                return _cups;
            }

            public override string ToString()
            {
                var previous = _cups.Take(_currentIndex);
                var following = _cups.Skip(_currentIndex + 1);
                return $"{string.Join(" ", previous)} ({GetCurrentCup()}) {string.Join(" ", following)}";
            }

            
        }

        private class CupLinkedCircle
        {
            private LinkedListNode<int> _currentCup;
            private readonly LinkedList<int> _cups;

            public CupLinkedCircle(IEnumerable<int> cupLabels)
            {
                _cups = new LinkedList<int>(cupLabels);
                _currentCup = _cups.First;
            }

            public void MakeAMove()
            {
                var pickedCups = Pick3Cups();
                var destinationCup = PickDestinationCup();
                foreach (var cup in pickedCups)
                    _cups.AddAfter(destinationCup, cup);

                _currentCup = _currentCup.Next ?? _cups.First;
            }
            
            private IEnumerable<int> Pick3Cups()
            {
                var trio = new LinkedList<int>();
                var pickedCup = _currentCup.Next;
                for (var i = 0; i < 3; i++)
                {
                    if (pickedCup == null)
                    {
                        pickedCup = _cups.First;
                        if (pickedCup == null)
                            throw new ArgumentNullException(nameof(pickedCup));
                    }

                    var node = pickedCup;
                    pickedCup = pickedCup.Next;
                    
                    _cups.Remove(node);
                    trio.AddFirst(node);
                }

                return trio;
            }

            private LinkedListNode<int> PickDestinationCup()
            {
                var destinationCupLabel = _currentCup.Value - 1;
                while (!_cups.Contains(destinationCupLabel))
                {
                    destinationCupLabel--;
                    if (destinationCupLabel == 0)
                        destinationCupLabel = _cups.Max();
                }
                var destinationCup = _cups.Find(destinationCupLabel);
                return destinationCup;
            }

            public List<int> GetCurrentCupsOrder()
            {
                return _cups.ToList();
            }

            public override string ToString()
            {
                var cups = this._cups.ToList();
                var currentIndex = cups.IndexOf(_currentCup.Value);
                var previous = cups.Take(currentIndex);
                var following = cups.Skip(currentIndex + 1);
                return $"{string.Join(" ", previous)} ({cups[currentIndex]}) {string.Join(" ", following)}";
            }


        }


    }

}
