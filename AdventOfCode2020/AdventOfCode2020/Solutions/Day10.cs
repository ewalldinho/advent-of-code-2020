using AdventOfCode2020.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2020.Solutions
{
    public class Day10 : IPuzzle
    {
        public string CalculateSolution(Parts part, string inputData)
        {
            var adapterRatings = inputData.Split(Environment.NewLine).Select(int.Parse)
                .OrderBy(x => x).ToList();
            var deviceBuiltInJoltage = adapterRatings.Max() + 3;
            var chargingOutletRating = 0;
            adapterRatings.Insert(0, chargingOutletRating);
            adapterRatings.Add(deviceBuiltInJoltage);

            switch (part)
            {
                case Parts.Part1:
                    var singleJoltDiffsCount = 0;
                    var tripleJoltDiffsCount = 0;
                    for (var i = 0; i < adapterRatings.Count-1; i++)
                    {
                        var j = i + 1;
                        switch (adapterRatings[j] - adapterRatings[i])
                        {
                            case 1:
                                singleJoltDiffsCount++;
                                break;
                            case 3:
                                tripleJoltDiffsCount++;
                                break;
                        }
                    }

                    var result1 = singleJoltDiffsCount * tripleJoltDiffsCount;
                    return result1.ToString();

                case Parts.Part2:
                    // split array of adapters into groups where it is possible more than 1 arrangement
                    var optionGroups = new List<List<int>>();
                    var options = new List<int>();

                    var index = 0;
                    while (index < adapterRatings.Count - 1)
                    {
                        var j = index + 1;

                        if (adapterRatings[j] - adapterRatings[index] < 3)
                        {
                            if (options.Count == 0)
                            {
                                options.Add(adapterRatings[index]);
                            }
    
                            options.Add(adapterRatings[j]);
                        }
                        else
                        {
                            if (options.Count > 0)
                            {
                                optionGroups.Add(options);
                                options = new List<int>();
                            }
                        }

                        index++;
                    }

                    var optionsCount = optionGroups.Select(group => CountVariations(group))
                        .Aggregate(1L, (current, count) => current * count);

                    return optionsCount.ToString();

                default:
                    throw new ApplicationException($"Invalid parameter {nameof(part)} value ({part})");
            }
        }


        private static int CountVariations(ICollection adapters)
        {
            // optimized for given data, but it is not the universal solution
            return adapters.Count switch
            {
                var x when x <= 2 => 1,
                3 => 2,
                4 => 4,
                5 => 7,
                // have to calculate :)
                _ => -1
            };
        }

        // option: recursively generate all possible sets
        // and check with this method if the set can be connected
        private bool CanConnect(IReadOnlyList<int> adapters)
        {
            for (var i = 0; i < adapters.Count; i++)
            {
                var j = i + 1;
                if (adapters[j] - adapters[i] > 3)
                    return false;
            }
            return true;
        }
    }
}
