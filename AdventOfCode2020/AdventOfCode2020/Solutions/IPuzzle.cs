using System;
using System.Collections.Generic;
using System.Text;
using AdventOfCode2020.Utils;

namespace AdventOfCode2020.Solutions
{
    public interface IPuzzle
    {
        string CalculateSolution(Parts part, string inputData);
    }
}
