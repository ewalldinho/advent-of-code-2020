using AdventOfCode2020.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2020.Solutions
{
    public class Day13 : IPuzzle
    {
        public string CalculateSolution(Parts part, string inputData)
        {
            var data = inputData.Split(Environment.NewLine);
            var earliestTimestamp = int.Parse(data[0]);
            var buses = data[1].Split(',')
                .Select(id => id == "x" ? "0" : id)
                .Select(int.Parse).ToList();

            switch (part)
            {
                case Parts.Part1:
                    var (busId, minutesToWait) = FindEarliestBus(earliestTimestamp, buses.Where(id => id != 0));
                    return (busId * minutesToWait).ToString();

                case Parts.Part2:
                    var timestamp = FindTimestampForDepartsWithOffset(buses);
                    return timestamp.ToString();

                default:
                    throw new ApplicationException($"Invalid parameter {nameof(part)} value ({part})");
            }
        }


        private static (int busId, int minutesToWait) FindEarliestBus(int earliestTimestamp, IEnumerable<int> buses)
        {
            var earliestBusId = int.MaxValue;
            var earliestBusDeparture = int.MaxValue;
            foreach (var busId in buses)
            {
                var currentBusTimeStamp = earliestTimestamp;
                while (currentBusTimeStamp % busId != 0)
                    currentBusTimeStamp++;

                if (currentBusTimeStamp < earliestBusDeparture)
                {
                    earliestBusDeparture = currentBusTimeStamp;
                    earliestBusId = busId;
                }
                
            }

            return (earliestBusId, earliestBusDeparture - earliestTimestamp);
        }

        private static long FindTimestampForDepartsWithOffset(IReadOnlyList<int> buses)
        {
            var departureOffset = 0L;
            long increment = buses.FirstOrDefault(id => id != 0);
            for (var index = 0; index < buses.Count; index++)
            {
                if (buses[index] == 0) continue;
                
                while (0 != (departureOffset + index) % buses[index])
                    departureOffset += increment;

                var leastCommonMultiple = FindLeastCommonMultiple(increment, buses[index]);
                increment = leastCommonMultiple;
            }
            
            return departureOffset;
        }

        private static long FindLeastCommonMultiple(long a, long b)
        {
            return a * b / FindGreatestCommonDivisor(a, b);
        }

        private static long FindGreatestCommonDivisor(long a, long b)
        {
            var dividend = Math.Max(a, b);
            var divisor = Math.Min(a, b);
            var remainder = dividend % divisor;
            while (remainder > 0)
            {
                dividend = divisor;
                divisor = remainder;
                remainder = dividend % divisor;
            }

            return divisor;
        }

        // inefficient brute force method
        private long FindTimestampForDepartsWithOffset_Inefficient(List<int> buses)
        {
            var maxId = buses.Max();
            var index = buses.IndexOf(maxId);
            long timestamp = maxId;
            var found = false;
            while (!found)
            {
                var inOrder = true;
                for (var i = 0; i < buses.Count; i++)
                {
                    if (buses[i] == 0)
                        continue;

                    if ((timestamp + (i - index)) % buses[i] != 0)
                    {
                        inOrder = false;
                        break;
                    }
                }

                if (!inOrder)
                    timestamp += maxId;
                else
                    found = true;
            }

            return timestamp - index;
        }


    }
}
