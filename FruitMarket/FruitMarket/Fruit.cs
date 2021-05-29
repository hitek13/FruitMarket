using System;
using System.Collections.Generic;
using System.Text;

namespace FruitMarket
{
    class Fruit
    {
        public FruitType type { set; get; }

        public double weight { set; get; }

        public double getPrice()
        {
            if(type != null)
            {
                return Math.Round( (weight / 1000) * type.price , 2);
            }
            return 0.00;
        }
    }
}
