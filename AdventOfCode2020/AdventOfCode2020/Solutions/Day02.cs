using AdventOfCode2020.Utils;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode2020.Solutions
{
    public class Day02 : IPuzzle
    {
        public string CalculateSolution(Parts part, string inputData)
        {
            var passwordsWithPolicies = inputData.Split(Environment.NewLine)
                .Select(x =>
                {
                    var pwdWithPolicy = x.Split(": ");
                    var policy = PasswordPolicy.Parse(pwdWithPolicy[0]);
                    return new { Policy = policy, Password = pwdWithPolicy[1] };
                });

            switch (part)
            {
                case Parts.Part1:
                    var validPasswordsCount1 = passwordsWithPolicies
                        .Count(pp => pp.Policy.IsValid1(pp.Password));
                    return validPasswordsCount1.ToString();
                case Parts.Part2:
                    var validPasswordsCount2 = passwordsWithPolicies
                        .Count(pp => pp.Policy.IsValid2(pp.Password));
                    return validPasswordsCount2.ToString();

                default:
                    throw new ApplicationException($"Invalid parameter {nameof(part)} value ({part})");
            }
        }

    }


    public class PasswordPolicy
    {
        private char _character;
        private int _min;
        private int _max;

        public bool IsValid1(string password)
        {
            var count = password.Count(c => c == _character);
            return _min <= count && count <= _max;
        }

        public bool IsValid2(string password)
        {
            var has1 = password[_min - 1] == _character;
            var has2 = password[_max - 1] == _character;
            return has1 ^ has2;
        }

        // parses "X-Y c"
        public static PasswordPolicy Parse(string passwordPolicy)
        {
            var regEx = new Regex(@"(\d+)-(\d+) ([a-z]+)");
            var m = regEx.Match(passwordPolicy);
            
            return new PasswordPolicy
            {
                _min = int.Parse(m.Groups[1].Value),
                _max = int.Parse(m.Groups[2].Value),
                _character = m.Groups[3].Value[0]
            };
        }
    }


}
