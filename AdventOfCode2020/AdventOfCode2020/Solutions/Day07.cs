using AdventOfCode2020.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode2020.Solutions
{
    public class Day07 : IPuzzle
    {
        private const string MyBagColor = "shiny gold";

        public string CalculateSolution(Parts part, string inputData)
        {
            var listOfBagRules = inputData.Split(Environment.NewLine)
                .Select(ParseBagRuleFromString)
                .ToDictionary(b => b.Color);

            switch (part)
            {
                case Parts.Part1:
                    var count = 0;
                    foreach (var color in listOfBagRules.Keys)
                        if (CanContainBag(color, MyBagColor, listOfBagRules))
                            count++;
                    return count.ToString();
                case Parts.Part2:
                    var requiredCount = CountContainedBags(MyBagColor, listOfBagRules);
                    
                    return requiredCount.ToString();

                default:
                    throw new ApplicationException($"Invalid parameter {nameof(part)} value ({part})");
            }
        }

        private static Bag ParseBagRuleFromString(string bagRule)
        {
            var bagColors = bagRule[0..^1].Split(" bags contain ");
            var containedBags = bagColors[1].Split(", ")
                .Select(bagDef =>
                {
                    var parsedBag = Regex.Match(bagDef, @"^(\d+) (\w+ \w+) bag[s]?$");
                    return parsedBag.Groups.Count == 3 ? new ContainedBag { Count = int.Parse(parsedBag.Groups[1].Value), Color = parsedBag.Groups[2].Value }
                        : new ContainedBag { Count = 0 };
                }).Where(b => b.Count > 0)
                .ToList();
            
            var bag = new Bag
            {
                Color = bagColors[0], 
                ContainedBags = containedBags
            };

            return bag;
        }

        private static bool CanContainBag(string containerColor, string lookupColor, Dictionary<string, Bag> bagRules)
        {
            if (bagRules[containerColor].ContainedBags.Count == 0)
                return false;

            if (bagRules[containerColor].ContainedBags.Any(b => b.Color == lookupColor))
                return true;

            return bagRules[containerColor].ContainedBags.Any(containedBag => CanContainBag(containedBag.Color, lookupColor, bagRules));
        }

        private static int CountContainedBags(string containerColor, Dictionary<string, Bag> bagRules)
        {
            if (bagRules[containerColor].ContainedBags.Count == 0)
                return 0;

            var count = 0;
            foreach (var containedBag in bagRules[containerColor].ContainedBags)
            {
                count += containedBag.Count;
                count += containedBag.Count * CountContainedBags(containedBag.Color, bagRules);
            }
            
            return count;
        }

    }


    public class Bag
    {
        public string Color { get; set; }
        public List<ContainedBag> ContainedBags { get; set; }
    }

    public class ContainedBag
    {
        public int Count { get; set; }
        public string Color { get; set; }
    }

}
