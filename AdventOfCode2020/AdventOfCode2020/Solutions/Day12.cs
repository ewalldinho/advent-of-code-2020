using AdventOfCode2020.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2020.Solutions
{
    public class Day12 : IPuzzle
    {
        public string CalculateSolution(Parts part, string inputData)
        {
            var navigationInstructions = inputData.Split(Environment.NewLine)
                .Select(line => new NavigationInstruction {
                    Action = line[0], 
                    Value = int.Parse(line.Substring(1))
                })
                .ToList();

            switch (part)
            {
                case Parts.Part1:
                    var distance1 = NavigateShip(navigationInstructions);
                    return distance1.ToString();
                    
                case Parts.Part2:
                    var distance2 = NavigateShipWithWaypoint(navigationInstructions);
                    return distance2.ToString();

                default:
                    throw new ApplicationException($"Invalid parameter {nameof(part)} value ({part})");
            }
        }

        private static int NavigateShip(List<NavigationInstruction> instructions)
        {
            var latitude = 0;
            var longitude = 0;
            var angle = 0;
            foreach (var instruction in instructions)
            {
                switch (instruction.Action)
                {
                    case 'N':
                        latitude += instruction.Value;
                        break;
                    case 'S':
                        latitude -= instruction.Value;
                        break;

                    case 'E':
                        longitude += instruction.Value;
                        break;
                    case 'W':
                        longitude -= instruction.Value;
                        break;

                    case 'L':
                        angle = (angle + instruction.Value) % 360;
                        break;
                    case 'R':
                        angle = (angle - instruction.Value + 360) % 360;
                        break;

                    case 'F':
                        if (angle % 180 == 0)
                        {
                            var longitudeSign = (int)Math.Cos(angle / 180.0 * Math.PI);
                            longitude += longitudeSign * instruction.Value;
                        }
                        else
                        {
                            var latitudeSign = (int)Math.Sin(angle / 180.0 * Math.PI);
                            latitude += latitudeSign * instruction.Value;
                        }
                        break;
                }
            }

            return Math.Abs(latitude) + Math.Abs(longitude);
        }


        private static int NavigateShipWithWaypoint(IEnumerable<NavigationInstruction> instructions)
        {
            var waypointLatitude = 1;    //  1 North  
            var waypointLongitude = 10;  // 10 East
            var latitude = 0;
            var longitude = 0;
            foreach (var instruction in instructions)
            {
                switch (instruction.Action)
                {
                    case 'N':
                        waypointLatitude += instruction.Value;
                        break;
                    case 'S':
                        waypointLatitude -= instruction.Value;
                        break;

                    case 'E':
                        waypointLongitude += instruction.Value;
                        break;
                    case 'W':
                        waypointLongitude -= instruction.Value;
                        break;

                    case 'L':
                        (waypointLatitude, waypointLongitude) = RotateWaypoint(waypointLatitude, waypointLongitude, instruction.Value);
                        break;
                    case 'R':
                        (waypointLatitude, waypointLongitude) = RotateWaypoint(waypointLatitude, waypointLongitude, -instruction.Value);
                        break;

                    case 'F':
                        latitude += waypointLatitude * instruction.Value;
                        longitude += waypointLongitude * instruction.Value;
                        break;
                }
            }

            return Math.Abs(latitude) + Math.Abs(longitude);
        }

        private static (int latitude, int longitude) RotateWaypoint(int latitude, int longitude, int angle)
        {
            var radians = angle / 180.0 * Math.PI;
            var latitudeRotated = (int)Math.Round(Math.Sin(radians) * longitude + Math.Cos(radians) * latitude);
            var longitudeRotated = (int)Math.Round(Math.Cos(radians) * longitude - Math.Sin(radians) * latitude);

            return (latitudeRotated, longitudeRotated);
        }

    }

    public class NavigationInstruction
    {
        public char Action { get; set; }
        public int Value { get; set; }
    }
}
