using System.IO;

namespace AdventOfCode2020.Utils
{
    public static class PuzzleData
    {
        public static string GetData(AdventDays day)
        {
            var textFilePath = $"data/day-{(int)day:00}.dat";

            using var reader = new StreamReader(File.OpenRead(textFilePath));
            var dataString = reader.ReadToEnd();
            return dataString;
        }
    }
}
