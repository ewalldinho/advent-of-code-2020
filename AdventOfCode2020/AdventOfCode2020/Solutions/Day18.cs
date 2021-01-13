using AdventOfCode2020.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace AdventOfCode2020.Solutions
{
    public class Day18 : IPuzzle
    {
        private const string OrderedOperators = "*+";

        public string CalculateSolution(Parts part, string inputData)
        {
            var expressions = inputData.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

            switch (part)
            {
                case Parts.Part1:
                    var sumOfValues1 = 0L;
                    foreach (var expression in expressions)
                    {
                        var value = EvaluateExpressionPart1(expression);
                        Debug.WriteLine($"{expression} -> {value}");
                        sumOfValues1 += value;
                    }
                    return sumOfValues1.ToString();

                case Parts.Part2:
                    var sumOfValues2 = 0L;
                    foreach (var expression in expressions)
                    {
                        var value = EvaluateExpressionPart2(expression);
                        Debug.WriteLine($"{expression} -> {value}");
                        sumOfValues2 += value;
                    }
                    return sumOfValues2.ToString();

                default:
                    throw new ApplicationException($"Invalid parameter {nameof(part)} value ({part})");

            }
        }

        private static long EvaluateExpressionPart1(string expression)
        {
            expression = expression.Replace(" ", string.Empty);

            var currentOperator = '\0';
            var isOperationActive = false;
            var stack = new Stack();
            
            foreach (var c in expression)
            {
                switch (c)
                {
                    case '(':
                        if (isOperationActive)
                        {
                            stack.Push(currentOperator);
                            isOperationActive = false;
                            currentOperator = '\0';
                        }

                        stack.Push('(');
                        break;
                    case ')':
                        var item = stack.Pop();

                        if (! (item is long number))
                            throw new ArgumentException($"Expected a number, found '{item}'");

                        // skip open parentheses
                        stack.Pop();
                       
                        if (stack.Count >= 2 && $"{stack.Peek()}" != "(")
                        {
                            var popOperator = stack.Pop();
                            if (!(popOperator is char op))
                                throw new InvalidOperationException($"Expected operator character, found '{popOperator}'");

                            var popAccumulator = stack.Pop();
                            if (!(popAccumulator is long accumulatedNumber))
                                throw new InvalidOperationException($"Expected a number, found '{popAccumulator}'");

                            number = Evaluate(accumulatedNumber, number, op);
                        }

                        // push back simplified evaluation
                        stack.Push(number);
                        break;

                    case var n when '0' <= n && n <= '9':
                        var digit = long.Parse(n.ToString());
                        if (isOperationActive)
                        {
                            var popPrevNumber = stack.Pop();
                            if (!(popPrevNumber is long prevNumber))
                                throw new InvalidOperationException($"Expected a number, found '{popPrevNumber}'");

                            var num1 = Evaluate(prevNumber, digit, currentOperator);
                            stack.Push(num1);

                            isOperationActive = false;
                        }
                        else
                        {
                            stack.Push(digit);
                        }
                        break;

                    case var op when OrderedOperators.Contains(op):
                        isOperationActive = true;
                        currentOperator = op;
                        break;

                    default:
                        throw new ArgumentException($"Expression '{expression}' contains invalid character '{c}'");
                }
            }

            var value = stack.Pop();
            if (!(value is long popValue))
                throw new ArgumentException($"Expression evaluated to '{value}', which is not a valid number");

            return popValue;
        }
    
        private static long EvaluateExpressionPart2(string expression)
        {
            expression = expression.Replace(" ", string.Empty);
            var expressionQueue = ParseExpression(expression);
            var value = EvaluateExpression(expressionQueue);
            return value;
        }

        private static long Evaluate(long a, long b, char operation)
        {
            var result = operation switch
            {
                '+' => a + b,
                '*' => a * b,
                _ => throw new InvalidOperationException($"Operator '{operation}' is not known ")
            };

            return result;
        }

        // uses Shunting-yard algorithm for mathematical infix notation expression parsing into postfix notation 
        private static Queue<char> ParseExpression(string expression)
        {
            var operatorStack = new Stack<char>();
            var outputQueue = new Queue<char>();

            foreach (var item in expression)
            {
                switch (item)
                {
                    case '(':
                        operatorStack.Push(item);
                        break;
                    case ')':
                        while (operatorStack.Peek() != '(')
                        {
                            var popOp = operatorStack.Pop();
                            outputQueue.Enqueue(popOp);
                        }
                        // discard left parenthesis
                        operatorStack.Pop();
                        break;

                    case { } n when '0' <= n && n <= '9':
                        outputQueue.Enqueue(n);
                        break;

                    case { } op when OrderedOperators.Contains(op):
                        var opIndex = OrderedOperators.IndexOf(op);
                        while (operatorStack.Count > 0 && opIndex <= OrderedOperators.IndexOf(operatorStack.Peek()))
                        {
                            var popOperator = operatorStack.Pop();
                            outputQueue.Enqueue(popOperator);
                        }
                        operatorStack.Push(item);
                        break;
                    default:
                        throw new ArgumentException($"Expression '{expression}' contains invalid character '{item}'");
                }
            }

            while (operatorStack.Count > 0)
                outputQueue.Enqueue(operatorStack.Pop());

            return outputQueue;
        }

        private static long EvaluateExpression(Queue<char> expression)
        {
            const string digits = "0123456789";
            var evalStack = new Stack<long>();
            while (expression.Count > 0)
            {
                var item = expression.Dequeue();

                if (digits.Contains(item))
                {
                    var number = item - '0';
                    evalStack.Push(number);
                }
                else if (OrderedOperators.Contains(item))
                {
                    var arg2 = evalStack.Pop();
                    var arg1 = evalStack.Pop();
                    var value = Evaluate(arg1, arg2, item);
                    evalStack.Push(value);
                }
                else
                {
                    throw new ArgumentException($"Expression contains invalid item '{item}'");
                }
            }

            return evalStack.Pop();
        }
    }
}
