using AdventOfCode2020.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2020.Solutions
{
    public class Day21 : IPuzzle
    {
        public string CalculateSolution(Parts part, string inputData)
        {
            var listOfFood = ParseInputData(inputData);

            switch (part)
            {
                case Parts.Part1:
                    return CountNonAllergicIngredients(listOfFood).ToString();

                case Parts.Part2:
                    return FindCanonicalDangerousIngredientList(listOfFood);

                default:
                    throw new ApplicationException($"Invalid parameter {nameof(part)} value ({part})");
            }
        }

        private static long CountNonAllergicIngredients(List<Food> listOfFood)
        {
            var allergens = new Dictionary<string, List<string>>();

            foreach (var food in listOfFood)
            {
                foreach (var allergen in food.Allergens)
                {
                    if (allergens.ContainsKey(allergen))
                    {
                        allergens[allergen] = allergens[allergen].Intersect(food.Ingredients).ToList();
                    }
                    else
                    {
                        allergens.Add(allergen, food.Ingredients.ToList());
                    }
                }
            }

            var allergicIngredients = allergens.SelectMany(a => a.Value);
            var nonAllergicIngredients = listOfFood.SelectMany(food => food.Ingredients)
                .Where(ingredient => !allergicIngredients.Contains(ingredient));

            return nonAllergicIngredients.Count();
        }

        private static string FindCanonicalDangerousIngredientList(List<Food> listOfFood)
        {
            var allergens = new Dictionary<string, List<string>>();

            foreach (var food in listOfFood)
            {
                foreach (var allergen in food.Allergens)
                {
                    if (allergens.ContainsKey(allergen))
                        allergens[allergen] = allergens[allergen].Intersect(food.Ingredients).ToList();
                    else
                        allergens.Add(allergen, food.Ingredients.ToList());
                }
            }

            var solvedAllergens = new List<KeyValuePair<string, string>>();
            while (allergens.Any())
            {
                var solved = allergens.Where(a => a.Value.Count == 1).Select(a => new KeyValuePair<string, string>(a.Key, a.Value.Single())).ToList();
                solvedAllergens.AddRange(solved);

                foreach (var (allergen, _) in solved)
                {
                    if (allergens.ContainsKey(allergen))
                        allergens.Remove(allergen);
                }

                var allergenKeys = allergens.Keys.ToArray();
                foreach (var allergenKey in allergenKeys)
                {
                    allergens[allergenKey] = allergens[allergenKey].Where(ing => solvedAllergens.All(sa => sa.Value != ing)).ToList();
                }
            }

            var canonicalDangerousIngredientList = solvedAllergens.OrderBy(sa => sa.Key)
                .Select(sa => sa.Value);

            return string.Join(",", canonicalDangerousIngredientList);
        }

        private static List<Food> ParseInputData(string inputData)
        {
            var foodDescriptions = inputData.Split(Environment.NewLine);
            var listOfFoods = foodDescriptions.Select(food => new Food(food))
                .ToList();
            return listOfFoods;
        }

        private class Food
        {
            public string[] Ingredients { get; set; }
            public string[] Allergens { get; set; }

            public Food(string foodDescription)
            {
                var food = foodDescription.Split(" (contains ");
                Ingredients = food[0].Split(' ');
                Allergens = food[1][..^1].Split(", ");
            }
        }
    }

}
