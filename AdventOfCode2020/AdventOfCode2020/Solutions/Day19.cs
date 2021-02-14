using AdventOfCode2020.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode2020.Solutions
{
    public class Day19 : IPuzzle
    {
        public string CalculateSolution(Parts part, string inputData)
        {
            var data = inputData.Split(Environment.NewLine + Environment.NewLine);
            var rules = data[0].Split(Environment.NewLine);
            var messages = data[1].Split(Environment.NewLine);

            var rule = new MessageRule(rules);
            //Debug.WriteLine(rule);

            switch (part)
            {
                case Parts.Part1:
                    var ruleRegex = rule.ToRegex();
                    var matchingMessagesCount = 0;
                    foreach (var message in messages)
                    {
                        if (ruleRegex.IsMatch(message))
                        {
                            matchingMessagesCount++;
                        }
                    }
                    return matchingMessagesCount.ToString();

                case Parts.Part2:
                    var (expression42, expression31) = rule.ToExpressionsFor42And31();
                    var matchedMessagesCount = CountMatchingMessagesWithException(expression42, expression31, messages);
                    return matchedMessagesCount.ToString();

                default:
                    throw new ApplicationException($"Invalid parameter {nameof(part)} value ({part})");
            }
        }

        private static int CountMatchingMessagesWithException(string expression42, string expression31, string[] messages)
        {
            // 0: 8 11
            // 8: 42
            // 11: 42 31
            // ..
            // Change rules to:
            // 8: 42 | 42 8   -> regex (42)+
            // 11: 42 31 | 42 11 31   -> recursive regex (42(42(42)++(31)++31)31) -> (42{q}31{q})
            // 0: 42+ (42++ 31++) 

            List<string> passedMessages = new List<string>();
            var generalRegex0 = new Regex($"^(({expression42})+)(({expression42})+({expression31})+)$");
            foreach (var message in messages)
            {
                if (generalRegex0.IsMatch(message))
                {
                    passedMessages.Add(message);
                }
            }

            var matchingMessagesCount = 0;
            foreach (var message in passedMessages)
            {
                for (var quantity = 1; quantity <= 10; quantity++)
                {
                    var regex0 = new Regex($"^(({expression42})+)({expression42}){{{quantity}}}({expression31}){{{quantity}}}$");

                    if (regex0.IsMatch(message))
                    {
                        Debug.WriteLine(message);
                        matchingMessagesCount++;
                        
                        break;
                    }
                }
            }

            return matchingMessagesCount;
        }
    }

    public class MessageRule
    {
        private readonly Dictionary<int, RuleNode> _nodes;
        private static readonly Regex ValueRex = new Regex("(?:\"([ab])\")");

        public MessageRule(string[] rules)
        {
            var ruleNodeList = new List<RuleNode>();
            foreach (var rule in rules)
            {
                ruleNodeList.Add(ParseRuleNode(rule));
            }

            _nodes = ruleNodeList.ToDictionary(node => node.Id);
        }

        private static RuleNode ParseRuleNode(string rule)
        {
            var parts = rule.Split(": ", StringSplitOptions.RemoveEmptyEntries);
            var ruleId = int.Parse(parts[0]);

            var m = ValueRex.Match(parts[1]);
            if (m.Success)
            {
                var valueNode = new RuleNode(ruleId, m.Groups[1].Value);
                return valueNode;
            }
            
            var parallelBranches = parts[1].Split(" | ", StringSplitOptions.RemoveEmptyEntries);
            if (parallelBranches.Length >= 1)
            {
                var branches = new List<List<int>>();
                foreach (var branch in parallelBranches)
                {
                    var references = branch.Split(" ", StringSplitOptions.RemoveEmptyEntries)
                        .Select(int.Parse).ToList();
                    branches.Add(references);
                }

                var refNode = new RuleNode(ruleId, branches);
                return refNode;
            }
            
            throw new ArgumentException($"Can not recognize rule '{rule}'");
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var (i, rule) in _nodes)
            {
                sb.AppendLine($"{i}: {rule}");
            }

            return sb.ToString();
        }

        public Regex ToRegex()
        {
            var expression = $"^{ToRegularExpression(0)}$";
            return new Regex(expression);
        }

        public (string rex42, string rex31) ToExpressionsFor42And31()
        {
            var regExpression42 = ToRegularExpression(42);
            var regExpression31 = ToRegularExpression(31);

            return (rex42: regExpression42, rex31: regExpression31);
        }

        private string ToRegularExpression(int index)
        {
            if (_nodes[index].Value != null)
            {
                return _nodes[index].Value;
            }

            var list = new List<string>();
            foreach (var branch in _nodes[index].Branches)
            {
                var references = "(";
                foreach (var refIndex in branch)
                {
                    references += $"({ToRegularExpression(refIndex)})";
                }
                references += ")";

                list.Add(references);
            }

            return string.Join("|", list);
        }
    }

    public class RuleNode
    {
        public int Id { get; }
        public string Value { get; }
        public List<List<int>> Branches { get; }

        public RuleNode(int id, string value)
        {
            Id = id;
            Value = value;
        }

        public RuleNode(int id, List<List<int>> references)
        {
            Id = id;
            Branches = references;
        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Value))
                return Value;

            var str = new List<string>();
            foreach (var branch in Branches)
            {
                str.Add(string.Join(" ", branch));
            }

            return string.Join(" | ", str);
        }
    }

}
