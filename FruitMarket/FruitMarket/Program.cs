using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FruitMarket
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8; //We want the console to recognize the € symbol
            double maxWeight = 1, packageFee = 1;
            string fruitTypeString = "", fruitString = "";

            Console.WriteLine("Hello! Please enter the fruit types list according to the following format: [name] [price]€/Kg, [name] [price]€/Kg ...");
            Console.WriteLine(" Or press return without typing anything if you want to run an example.");

            fruitTypeString = Console.ReadLine();

            if (fruitTypeString != "")
            {
                try
                {
                    Console.WriteLine("Please, enter the package fee.");
                    var feeString = Console.ReadLine();

                    Regex patternFee = new Regex(@"\s?(?<fee>\d+)\s?\€?\s?");
                    Match matchFee = patternFee.Match(feeString);
                    packageFee = double.Parse(matchFee.Groups["fee"].Value.Replace('.', ','));
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error! The provided package fee type does not match the expected format. Default value: 1€.");
                    packageFee = 1;
                }

                Console.WriteLine("Now enter the fruit list according to the following format: [name] [weight]g, [name] [weight]g ...");
                fruitString = Console.ReadLine();
                
                try
                {
                    Console.WriteLine("Please, enter the package max weight in Kg.");
                    var weightString = Console.ReadLine();

                    Regex patternWeight = new Regex(@"\s?(?<weight>[\d,.]+)\s?([K,k][G,g])?\s?");
                    Match matchWeight = patternWeight.Match(weightString);
                    maxWeight = double.Parse(matchWeight.Groups["weight"].Value.Replace('.', ','));
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error! The provided package max weight type does not match the expected format. Default value: 1Kg.");
                    maxWeight = 1;
                }
            }
            else
            {
                fruitTypeString = "apple 1€/Kg , avocado 10€/Kg, banana 3€ / KG , melon 1.2€/kg ";
                Console.WriteLine("fruit types: apple 1€/Kg , avocado 10€/Kg, banana 3€ / KG , melon 1.2€/kg ");

                packageFee = 1.25;
                Console.WriteLine("packaging fee: 1.25€.");

                maxWeight = 1.100;
                Console.WriteLine("max weight: 1.100Kg.");

                fruitString = "apple 300g, avocado 100g, apple 400g, apple 800g, avocado 600g, banana 500g, melon 1300g, banana 100g";
                Console.WriteLine("fruits: apple 300g, avocado 100g, apple 400g, apple 800g, avocado 600g, banana 500g, melon 1300g, banana 100g");


            }

            Console.WriteLine("");
            
            /*
             * Now we will assign fruits their type and put them in a box
             */ 
            var types = makeFruitTypeList(fruitTypeString);
            var fruits = makeFruitList(types, fruitString); 
            var boxes = fillBoxes(fruits, packageFee, maxWeight); // If a fruit is too heavy to be in a box, we will discard it

            Console.WriteLine("");


            /*
             * Now we will show all the boxes
             */ 
            int i = 1;
            if(boxes == null)
            {
                Console.WriteLine("No boxes where filled.");
            }
            else
            {
                foreach (var box in boxes)
                {
                    Console.WriteLine(" \u2022 Box " + i + ": " + box.getAllFruits() + " - " + box.getWeight() + "g - " + string.Format("{0:N2}", box.getPrice()) + "€ ");
                    i++;
                }
            }

            Console.WriteLine("");
            Console.WriteLine("Press return to exit.");
            var exit = Console.ReadLine();

        }

        public static List<FruitType> makeFruitTypeList(string fruitTypes)
        {
            List<FruitType> fruitsTypesList = new List<FruitType>();

            foreach (var typeString in fruitTypes.Split(','))
            {
                try
                {
                FruitType type = new FruitType();

                    // This regex expression will help us to get the name and the price of a fruit type
                Regex pattern = new Regex(@"\s?(?<name>\w+)\s(?<price>[\d.,]+)\s?[\€,\?]?\s?\/\s?[K,k][G,g]\s?");
                Match match = pattern.Match(typeString);

                type.name = match.Groups["name"].Value;
                type.price = double.Parse(match.Groups["price"].Value.Replace('.',','));

                fruitsTypesList.Add(type);
                
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error! The provided fruit type does not match the expected format.");
                }
            }

            return fruitsTypesList;

        }

        public static List<Fruit> makeFruitList (List<FruitType> fruitsTypesList, string fruits)
        {

            List<Fruit> fruitsList = new List<Fruit>();

            foreach(var fruitString in fruits.Split(','))
            {
                try
                {
                    Fruit fruit = new Fruit();

                        // This regex expression will help us to get the name and the weight of a fruit
                    Regex pattern = new Regex(@"\s?(?<name>\w+)\s(?<weight>[\d.,]+)\s?[G,g]");
                    Match match = pattern.Match(fruitString);

                        // Knowing the name of the fruit we will search their type
                    fruit.type = getFruitTypeFromList(match.Groups["name"].Value, fruitsTypesList);

                        // If the fruit name does not exists in the fruit type list provided we will throw an exception and discard it
                    if(fruit.type == null)
                    {
                        throw new Exception("Type: " + match.Groups["name"].Value);
                    }

                    fruit.weight = double.Parse(match.Groups["weight"].Value.Replace('.',','));

                    fruitsList.Add(fruit);
                }
                catch (Exception e)
                {
                    if(e.Message.ToString().Contains("Type: "))
                    {
                        Console.WriteLine("Error! " + e.Message.ToString() + " does not exist in the fruits type list provided.");
                    }
                    else
                    {
                        Console.WriteLine("Error! One of the provided fruit does not match the expected format.");
                    }

                }
            }

            return fruitsList;
        }

        public static List<Box> fillBoxes(List<Fruit> fruits, double packagingFee, double maxWeight)
        {
            if(fruits == null || fruits.Count == 0)
            {
                return null;
            }

            List<Box> boxes = new List<Box>();
            Box box = new Box(packagingFee, maxWeight);

            foreach (var fruit in fruits)
            {
                    // If a fruit is too heavy to be in a box, we will discard it
                if (fruit.weight > box.maxWeight * 1000)
                {
                    Console.WriteLine("Error! This " + fruit.type.name + " is too big for a package (max weight " + maxWeight + "Kg).");
                }
                else
                {
                        // If the fruit fits in the box, we add it, if not, we add the box to the list and open a new one
                    if (box.getWeight() + fruit.weight > box.maxWeight * 1000)
                    {
                        boxes.Add(box);
                        box = new Box(packagingFee, maxWeight);
                    }

                        // Despite if it is a new box or an old one, we add the fruit
                    box.fruits.Add(fruit);
                }               

            }

                // If we are out of the loop we add the last box to the list because it has at least a fruit
            boxes.Add(box);

            return boxes;
        }
        
        public static FruitType getFruitTypeFromList(string name, List<FruitType> fruitsTypesList)
        {
            foreach(var type in fruitsTypesList)
            {
                if(type.name == name)
                {
                    return type;
                }
            }

            return null;
        }

    }
}
