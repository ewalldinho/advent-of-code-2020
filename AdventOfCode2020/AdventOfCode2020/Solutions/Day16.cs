using AdventOfCode2020.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace AdventOfCode2020.Solutions
{
    public class Day16 : IPuzzle
    {
        public string CalculateSolution(Parts part, string inputData)
        {
            var lines = inputData.Split(Environment.NewLine + Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
            var ticketValidationRules = lines[0].Split(Environment.NewLine).Select(rule => new TicketValidationRule(rule)).ToList();
            var yourTicket = new Ticket(lines[1].Split(Environment.NewLine)[1]);
            var nearbyTickets = lines[2].Split(Environment.NewLine).Where(line => !line.StartsWith("nearby tickets"))
                .Select(ticket => new Ticket(ticket));

            switch (part)
            {
                case Parts.Part1:
                    var errorRate = CalculateTicketScanErrorRate(nearbyTickets.ToList(), ticketValidationRules);
                    return errorRate.ToString();
                
                case Parts.Part2:
                    var possibleFields = SortOutTheFields(yourTicket, nearbyTickets.ToList(), ticketValidationRules);

                    var departureIndexes = possibleFields.Where(f => f.Key.StartsWith("departure"))
                        .SelectMany(f => f.Value);

                    var product = 1L;
                    foreach (var index in departureIndexes)
                    {
                        product *= yourTicket.Fields[index];
                    }

                    return product.ToString();

                default:
                    throw new ApplicationException($"Invalid parameter {nameof(part)} value ({part})");
            }
        }

        
        private static int CalculateTicketScanErrorRate(IEnumerable<Ticket> tickets, IEnumerable<TicketValidationRule> rules)
        {
            var ticketValidationRules = rules as TicketValidationRule[] ?? rules.ToArray();
            var errorRate = 0;
            foreach (var ticket in tickets)
            {
                errorRate += ticket.CheckErrorRate(ticketValidationRules);
            }
            return errorRate;
        }

        private Dictionary<string, List<int>> SortOutTheFields(Ticket yourTicket, IReadOnlyCollection<Ticket> tickets, IEnumerable<TicketValidationRule> ticketValidationRules)
        {
            var rules = ticketValidationRules as TicketValidationRule[] ?? ticketValidationRules.ToArray();
            var fieldsCount = rules.Length;
            var possibleFields = rules.Select(rule => rule.FieldName)
                .ToDictionary(fieldName => fieldName, _ => Enumerable.Range(0, fieldsCount).ToList());

            var validTickets = tickets.Where(ticket => ticket.IsValid(rules))
                .Prepend(yourTicket).ToList();
            
            for (var index = 0; index < rules.Length; index++)
            {
                var rule = rules[index];
                var ticketIndex = 0;
                while (ticketIndex < validTickets.Count && possibleFields[rule.FieldName].Count > 1)
                {
                    var applicableFieldIndexes = validTickets[ticketIndex].FindApplicableFields(rule);
                    possibleFields[rule.FieldName].RemoveAll(fieldIndex => !applicableFieldIndexes.Contains(fieldIndex));
                    
                    ticketIndex++;
                }
            }

            // for debugging
            PossibleFieldsToString(possibleFields);

            var isSolving = true;
            while (isSolving && possibleFields.Any(f => f.Value.Count > 1))
            {
                var solvedIndexes = possibleFields.Where(f => f.Value.Count == 1).Select(f => f.Value.Single());
                var removedIndexes = 0;
                foreach (var (_, value) in possibleFields)
                {
                    if (value.Count > 1)
                        removedIndexes += value.RemoveAll(index => solvedIndexes.Contains(index));
                }

                isSolving = removedIndexes > 0;
            }

            return possibleFields;
        }

        private static void PossibleFieldsToString(Dictionary<string, List<int>> possibleFields)
        {
            var debugString = new StringBuilder();
            debugString.AppendLine("Possible field values:");
            foreach (var (fieldName, possibleIndexes) in possibleFields)
            {
                debugString.AppendLine($"{fieldName}: {string.Join(", ", possibleIndexes)} ");
            }
            
            Debug.WriteLine(debugString.ToString());
        }
    }

    public class TicketValidationRule
    {
        public string FieldName { get; }
        public Range[] Ranges { get; }

        public TicketValidationRule(string rule)
        {
            var values = rule.Split(": ");
            FieldName = values[0];
            Ranges = values[1].Split(" or ", StringSplitOptions.RemoveEmptyEntries)
                .Select(range =>
                {
                    var startEnd = range.Split('-');
                    var start = int.Parse(startEnd[0]);
                    var end = int.Parse(startEnd[1]);
                    return new Range(start, end);
                })
                .ToArray();
        }
    }

    public class Ticket
    {
        public List<int> Fields { get; }
        
        public Ticket(string ticket)
        {
            Fields = ticket.Split(',').Select(int.Parse).ToList();
        }

        public int CheckErrorRate(IEnumerable<TicketValidationRule> rules)
        {
            var errorFields = Fields.Where(f => rules.All(r => r.Ranges.All(rr => f < rr.Start.Value || rr.End.Value < f)));
            return errorFields.Sum();
        }

        public bool IsValid(TicketValidationRule[] rules)
        {
            var applicableFields = rules.Select(r => r.FieldName)
                .ToDictionary(fieldName => fieldName, _ => new List<int>());

            foreach (var rule in rules)
            {
                var list = applicableFields[rule.FieldName]; 
                list.AddRange(FindApplicableFields(rule));
                applicableFields[rule.FieldName] = list.Distinct().ToList();
            }

            var isValid = applicableFields.Values.All(v => v.Count > 0) 
                && Enumerable.Range(0, rules.Length).All(index => applicableFields.Values.Any(v => v.Contains(index)));

            return isValid;
        }

        public List<int> FindApplicableFields(TicketValidationRule rule)
        {
            var applicableIndexes = new List<int>();
            for (var i = 0; i < Fields.Count; i++)
            {
                if (rule.Ranges.Any(rr => rr.Start.Value <= Fields[i] && Fields[i]  <= rr.End.Value))
                    applicableIndexes.Add(i);
            }
           
            return applicableIndexes;
        }
        
    }
}
