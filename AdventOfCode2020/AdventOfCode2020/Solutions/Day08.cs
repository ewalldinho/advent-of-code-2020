using AdventOfCode2020.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2020.Solutions
{
    public class Day08 : IPuzzle
    {
        public string CalculateSolution(Parts part, string inputData)
        {
            var bootCode = inputData.Split(Environment.NewLine)
                .Select(ParseInstruction)
                .ToArray();

            switch (part)
            {
                case Parts.Part1:
                    var executionResult = ExecuteBootCode(bootCode);
                    return executionResult.Accumulator.ToString();

                case Parts.Part2:
                    foreach (var instruction in bootCode)
                    {
                        string originalOp;
                        switch (instruction.Operation)
                        {
                            case "nop":
                                originalOp = "nop";
                                instruction.Operation = "jmp";
                                break;
                            case "jmp":
                                originalOp = "jmp";
                                instruction.Operation = "nop";
                                break;
                            default:
                                continue;
                        }

                        var result = ExecuteBootCode(bootCode);
                        if (result.HasCompleted)
                            return result.Accumulator.ToString();

                        instruction.Operation = originalOp;
                    }

                    return "There is no solution for this boot code";

                default:
                    throw new ApplicationException($"Invalid parameter {nameof(part)} value ({part})");
            }
        }

        private static Instruction ParseInstruction(string instructionString)
        {
            var instructionWithArgument = instructionString.Split(' ');
            
            return new Instruction
            {
                Operation = instructionWithArgument[0],
                Argument = int.Parse(instructionWithArgument[1])
            };
        }

        private static ExecutionResult ExecuteBootCode(Instruction[] bootCode)
        {
            var accumulator = 0;
            var executedInstructions = new List<int>();
            var ip = 0;
            var infiniteLoopDetected = false;
            var completed = false;
            while (!infiniteLoopDetected && !completed)
            {
                completed = ip >= bootCode.Length;
                infiniteLoopDetected = executedInstructions.Contains(ip);
                if (!completed && !infiniteLoopDetected)
                {
                    executedInstructions.Add(ip);

                    switch (bootCode[ip].Operation)
                    {
                        case "acc":
                            accumulator += bootCode[ip].Argument;
                            ip++;
                            break;
                        case "jmp":
                            ip += bootCode[ip].Argument;
                            break;
                        case "nop":
                            ip++;
                            break;
                        default:
                            throw new InvalidOperationException($"Unknown operation: '{bootCode[ip]}'");
                    }
                }
            }

            return new ExecutionResult
            {
                HasCompleted = completed,
                Accumulator = accumulator
            };

        }
    }


    public class Instruction
    {
        public string Operation { get; set; }
        public int Argument { get; set; }
    }

    public class ExecutionResult
    {
        public bool HasCompleted { get; set; }
        public int Accumulator { get; set; }
    }

}
