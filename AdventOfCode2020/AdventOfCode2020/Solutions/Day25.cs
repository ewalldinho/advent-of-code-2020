using AdventOfCode2020.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2020.Solutions
{
    public class Day25 : IPuzzle
    {
        
        public string CalculateSolution(Parts part, string inputData)
        {
            const bool goSlow = false;
            var publicKeys = inputData.Split(Environment.NewLine).Select(int.Parse).ToArray();
            
            switch (part)
            {
                case Parts.Part1:
                    return SolvePart1(publicKeys, goSlow).ToString();

                case Parts.Part2:
                    return SolvePart2(publicKeys);

                default:
                    throw new ApplicationException($"Invalid parameter {nameof(part)} value ({part})");
            }
        }

        private static long SolvePart1(IReadOnlyList<int> publicKeys, bool goSlow)
        {
            var cardPublicKey = publicKeys[0];
            var doorPublicKey = publicKeys[1];

            if (goSlow)
            {
                var cardLoop = ComboBreaker.FindLoopSize(7, cardPublicKey);
                var doorLoop = ComboBreaker.FindLoopSizeOptimized(7, doorPublicKey);
                var cardEncKey = ComboBreaker.TransformSubjectNumber(doorPublicKey, cardLoop);
                var doorEncKey = ComboBreaker.TransformSubjectNumber(cardPublicKey, doorLoop);
                return cardEncKey == doorEncKey ? cardEncKey : -1;
            }

            // calc/guess secret loop sizes
            var cardLoopSize = ComboBreaker.FindLoopSizeOptimized(7, cardPublicKey);
            var doorLoopSize = ComboBreaker.FindLoopSizeOptimized(7, doorPublicKey);
            // calculate encryption keys
            var cardEncryptionKey = ComboBreaker.TransformSubjectNumber(doorPublicKey, cardLoopSize);
            var doorEncryptionKey = ComboBreaker.TransformSubjectNumber(cardPublicKey, doorLoopSize);
            // check if the keys match
            if (cardEncryptionKey == doorEncryptionKey)
                return cardEncryptionKey;
            // return if failure encountered
            return -1;
        }

        private static string SolvePart2(int[] publicKeys)
        {
            if (publicKeys.Length != 2)
                return "invalid data";

            return "You need 49 stars to get there...";
        }


        private static class ComboBreaker
        {
            // inefficient brute force
            public static int FindLoopSize(int subjectNumber, int publicKey)
            {
                
                for (var loopSize = 1; loopSize < 1000000; loopSize++)
                {
                    var number = 1;
                    for (var i = 0; i < loopSize; i++)
                    {
                        number = number * subjectNumber;
                        number = number % 20201227;
                    }

                    if (number == publicKey)
                        return loopSize;
                }

                return 0;
            }

            public static int FindLoopSizeOptimized(int subjectNumber, int encryptionKey)
            {
                /* *** *** *** *
                 * Encrypt
                 *  n = 1
                 * repeat (loopSize):
                 *   n = n * subjNum
                 *   n = n % 20201227   --> while (n >= 20201227) n-= 20201227;
                 * return n -> publicKey / encryptionKey
                 *
                 * *** *** *** *
                 * Decrypt:
                 *   subjNum = 7
                 *   loop = 0
                 *   m = publicKey
                 *   repeat:
                 *     while (m % 7 != 0):
                 *       m += 20201227
                 *     while (m > 1) && (m % 7 == 0):
                 *       m = m / 7
                 *       loop++;
                 *   until m==1
                 *
                 * *** */

                var loopSize = 0;
                var number = encryptionKey;
                while (number != 1)
                {
                    while (number % subjectNumber != 0)
                    {
                        number += 20201227;
                    }

                    //var m = number;
                    while (number > 1 && number % 7 == 0)
                    {
                        number = number / 7;
                        //m = m / 7;
                        loopSize++;
                    }
                }

                return loopSize;
            }

            public static long TransformSubjectNumber(int subjectNumber, int loopSize)
            {
                var number = 1L;
                for (var i = 0; i < loopSize; i++)
                {
                    number *= subjectNumber;
                    number %= 20201227;
                }
                
                return number;
            }
        }
    }

}
