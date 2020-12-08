using AdventOfCode2020.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2020.Solutions
{
    public class Day06 : IPuzzle
    {
        public string CalculateSolution(Parts part, string inputData)
        {
            var formsStatsData = inputData.Split(Environment.NewLine + Environment.NewLine);

            switch (part)
            {
                case Parts.Part1:
                    var anyYesAnswerCount = formsStatsData
                        .Select(groupData => groupData.Distinct().Count(c => !char.IsWhiteSpace(c)))
                        .Sum();
                    return anyYesAnswerCount.ToString();
                case Parts.Part2:
                    var allYesAnswerCount = formsStatsData.Select(groupData => CalculateGroupStats(groupData)).Sum();
                    return allYesAnswerCount.ToString();

                default:
                    throw new ApplicationException($"Invalid parameter {nameof(part)} value ({part})");
            }
        }

        private static int CalculateGroupStats(string stats)
        {
            var statPerPerson = stats.Split(Environment.NewLine);
            
            if (statPerPerson.Length == 1)
                return statPerPerson[0].Length;

            var commonAnswers = new List<char>();

            foreach (var questionLetter in statPerPerson[0])
            {
                if (statPerPerson.All(answeredQuestions => answeredQuestions.Contains(questionLetter))) 
                    commonAnswers.Add(questionLetter);
            }

            return commonAnswers.Count;
        }
    }

    public class FormYesToAnyStats
    {
        public int PersonCount { get; set; }
        public int DistinctAnswerCount { get; set; }
    }

}
