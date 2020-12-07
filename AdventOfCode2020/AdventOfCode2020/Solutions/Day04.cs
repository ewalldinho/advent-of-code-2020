using AdventOfCode2020.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode2020.Solutions
{
    public static class Day04
    {
        public static string CalculateSolution(Parts part, string inputData)
        {
            var passportSeparator = string.Concat(Environment.NewLine, string.Empty, Environment.NewLine);
            var passports = inputData.Split(passportSeparator).Select(Passport.Parse).ToList();

            switch (part)
            {
                case Parts.Part1:
                    var realPassportsCount = passports.Count(p => p.HasRequiredFields());
                    return realPassportsCount.ToString();
                case Parts.Part2:
                    var validPassportsCount = passports.Count(p => p.IsValid());
                    return validPassportsCount.ToString();

                default:
                    throw new ApplicationException($"Invalid parameter {nameof(part)} value ({part})");
            }
        }
    }

    public class Passport
    {
        private const string FieldDataSeparator = ":";
        private static readonly Regex PassportFieldSeparator = new Regex(@"\s+");
        private static readonly string[] RequiredKeys = {
            "byr", // birth year
            "iyr", // issued year
            "eyr", // expiration year
            "hgt", // height
            "hcl", // hair color
            "ecl", // eye color
            "pid"  // passport ID
            //,"cid"  // country ID (optional)
        };
        private static readonly string[] ValidEyeColors = { "amb", "blu", "brn", "gry", "grn", "hzl", "oth" };

        private Dictionary<string, string> Fields { get; set; }

        public static Passport Parse(string passportString)
        {
            var passportFields = new Dictionary<string, string>();
            var parsedFields = PassportFieldSeparator.Split(passportString);
            foreach (var field in parsedFields)
            {
                var keyValue = field.Split(FieldDataSeparator);
                if (keyValue.Length != 2)
                    throw new InvalidDataException($"Invalid key-value pair for password field: '{field}'.");
                
                passportFields.Add(keyValue[0], keyValue[1]);
            }

            var passport = new Passport
            {
                Fields = passportFields
            };
            return passport;
        }

        public bool HasRequiredFields()
        {
            return RequiredKeys.All(key => Fields.Keys.Contains(key));
        }

        public bool IsValid()
        {
            if (!HasRequiredFields())
                return false;

            foreach (var (fieldName, fieldValue) in Fields)
            {
                switch (fieldName)
                {
                    case "byr": // four digits; at least 1920 and at most 2002
                        if (!IsValidNumberInRange(fieldValue, 1920, 2002))
                            return false;

                        break;
                    case "iyr": // four digits; at least 2010 and at most 2020
                        if (!IsValidNumberInRange(fieldValue, 2010, 2020)) 
                            return false;

                        break;
                    case "eyr": // four digits; at least 2020 and at most 2030
                        if (!IsValidNumberInRange(fieldValue, 2020, 2030)) 
                            return false;

                        break;
                    case "hgt": // a number followed by either cm or in
                        // if cm, the number must be at least 150 and at most 193
                        var m = Regex.Match(fieldValue, @"^(\d{3})(cm)$|^(\d{2})(in)$");
                        if (!m.Success)
                            return false;

                        switch (m.Groups[2].Value)
                        {
                            case "cm":
                                if (!IsValidNumberInRange(m.Groups[1].Value, 150, 193))
                                    return false;
                                break;
                            case "in":
                                if (!IsValidNumberInRange(m.Groups[1].Value, 59, 76))
                                    return false;
                                break;
                        }

                        break;
                        
                    case "hcl": // Hair Color - a # followed by exactly six characters 0-9 or a-f.
                        if (!Regex.IsMatch(fieldValue, "#[0-9a-f]{6}")) return false;
                        break;
                    case "ecl": //Eye Color - exactly one of: amb blu brn gry grn hzl oth
                        if (!ValidEyeColors.Contains(fieldValue)) return false;
                        break;
                    case "pid": //  passport ID - a nine - digit number, including leading zeroes.
                        if (!Regex.IsMatch(fieldValue, @"^\d{9}$")) return false;
                        break;
                    case "cid": // Country ID - ignored, missing or not.
                        break;
                }
            }

            return true;
        }

        private bool IsValidNumberInRange(string stringValue, int rangeMin, int rangeMax)
        {
            if (!int.TryParse(stringValue, out var number)) return false;

            return rangeMin <= number && number <= rangeMax;
        }

        public static bool IsValidPassportString(string passportString)
        {
            return RequiredKeys.All(k => new Regex($"\\b{k}:").IsMatch(passportString));
        }

        public override string ToString()
        {
            var fields = new List<string>();
            foreach (var requiredKey in RequiredKeys)
            {
                if (Fields.Keys.Contains(requiredKey))
                {
                    fields.Add(Fields[requiredKey]);
                }
            }

            if (Fields.Keys.Contains("cid"))
            {
                fields.Add(Fields["cid"]);
            }
            else
            {
                fields.Add("-NA-");
            }

            return string.Join("\t", fields);
        }

    }
}
