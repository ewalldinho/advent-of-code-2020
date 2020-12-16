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
                    var d = FindTimestampForDepartsWithOffset(buses);
                    return d.ToString();

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

        private long FindTimestampForDepartsWithOffset(List<int> buses)
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
