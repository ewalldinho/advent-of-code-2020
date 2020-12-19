using AdventOfCode2020.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode2020.Solutions
{
    public class Day14 : IPuzzle
    {
        public string CalculateSolution(Parts part, string inputData)
        {
            var initProgram = inputData.Split(Environment.NewLine);
            

            switch (part)
            {
                case Parts.Part1:
                    var program = new SeaPortComputerVer1();
                    var output = program.Initialize(initProgram);
                    return output.ToString();

                case Parts.Part2:
                    var program2 = new SeaPortComputerVer2();
                    var output2 = program2.Initialize(initProgram);
                    return output2.ToString();

                default:
                    throw new ApplicationException($"Invalid parameter {nameof(part)} value ({part})");
            }
        }

    }

    public abstract class SeaPortComputer
    {
        protected readonly Regex MaskRegex = new Regex(@"^mask = ([X01]+)$");
        protected readonly Regex MemRegex = new Regex(@"^mem\[(\d+)\] = (\d+)$");

        public abstract long Initialize(IEnumerable<string> initializationProgram);
    }

    public class SeaPortComputerVer1 : SeaPortComputer
    {
        public override long Initialize(IEnumerable<string> initializationProgram)
        {
            var currentMask = string.Empty;
            var memory = new Dictionary<int, long>();
            foreach (var command in initializationProgram)
            {
                var mem = MemRegex.Match(command);
                if (mem.Success)
                {
                    var memAddress = int.Parse(mem.Groups[1].Value);
                    var value = int.Parse(mem.Groups[2].Value);
                    var maskedValue = ApplyMask(value, currentMask);

                    if (memory.ContainsKey(memAddress))
                        memory[memAddress] = maskedValue;
                    else 
                        memory.Add(memAddress, maskedValue);
                }
                else
                {
                    var mask = MaskRegex.Match(command);
                    if (mask.Success)
                    {
                        currentMask = mask.Groups[1].Value;
                    }
                }

            }

            var sum = memory.Aggregate(0L, (currentSum, memCell) => currentSum + memCell.Value);

            return sum;
        }

        private static long ApplyMask(in int value, string mask)
        {
            var binary = Convert.ToString(value, 2).PadLeft(36, '0');
            var maskedValue = new string(mask.Select((ch, i) => ch == 'X' ? binary[i] : ch).ToArray());
            maskedValue.TrimStart('0');
            var result = Convert.ToInt64(maskedValue, 2);
            return result;
        }
    }

    public class SeaPortComputerVer2 : SeaPortComputer
    {
        private readonly Regex _maskRegex = new Regex(@"^mask = ([X01]+)$");
        private readonly Regex _memRegex = new Regex(@"^mem\[(\d+)\] = (\d+)$");

        public override long Initialize(IEnumerable<string> initializationProgram)
        {
            var currentMask = string.Empty;
            var memory = new Dictionary<long, int>();
            foreach (var command in initializationProgram)
            {
                var mem = _memRegex.Match(command);
                if (mem.Success)
                {
                    var memAddress = int.Parse(mem.Groups[1].Value);
                    var value = int.Parse(mem.Groups[2].Value);
                    var addressSpread = GetAddressSpread(memAddress, currentMask);

                    foreach (var address in addressSpread)
                    {
                        if (memory.ContainsKey(address))
                            memory[address] = value;
                        else
                            memory.Add(address, value);
                    }
                }
                else
                {
                    var mask = _maskRegex.Match(command);
                    if (mask.Success)
                    {
                        currentMask = mask.Groups[1].Value;
                    }
                }

            }

            var sum = memory.Aggregate(0L, (currentSum, memCell) => currentSum + memCell.Value);

            return sum;
        }

        private static IEnumerable<long> GetAddressSpread(int value, string mask)
        {
            var binary = Convert.ToString(value, 2).PadLeft(36, '0');
            var maskedAddress = new string(mask.Select((ch, i) => ch == '0' ? binary[i] : ch).ToArray());
            maskedAddress.TrimStart('0');

            var addresses = new HashSet<long>() { 0 };

            foreach (var bit in maskedAddress)
            {
                addresses = bit switch
                {
                    'X' => addresses.SelectMany(a => new HashSet<long>() {a << 1, (a << 1) + 1}).ToHashSet(),
                    '0' => addresses.Select(a => a << 1).ToHashSet(),
                    '1' => addresses.Select(a => (a << 1) + 1).ToHashSet(),
                    _ => addresses
                };
            }
            
            return addresses;
        }
    }

}
