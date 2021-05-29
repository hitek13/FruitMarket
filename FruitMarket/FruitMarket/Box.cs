using System;
using System.Collections.Generic;
using System.Text;

namespace FruitMarket
{
    class Box
    {
        public Box(double packagingFee, double maxWeight)
        {
            this.fruits = new List<Fruit>();
            this.packagingFee = packagingFee;
            this.maxWeight = maxWeight;
        }

        public List<Fruit> fruits { set; get; }

        public double packagingFee { set; get; }

        public double maxWeight { set; get; }

        public double getWeight()
        {
            double weight = 0;

            foreach(var fruit in fruits)
            {
                weight += fruit.weight;
            }

            return weight;
        }

        /*
         * Returns the total cost of the box including the packaging fee
         */
        public double getPrice()
        {
            double price = 0;

            foreach (var fruit in fruits)
            {
                price = price + fruit.getPrice();
            }

            return price + packagingFee;
        }

        /*
         * Returns a string with all the fruits names in the box
         */
        public string getAllFruits()
        {
            string allFruits = "";

            foreach (var fruit in fruits)
            {
                allFruits += fruit.type.name + ", ";
            }

            if (allFruits.Length < 2)
            {

                return "";
            }
            else
            {
                return allFruits.Substring(0, allFruits.Length-2);
            }
        }
    }
}
