using AnnaWebKitchenFin.Data.Enums;
using AnnaWebKitchenFin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnnaWebKitchenFin.Data
{
    public class Menu
    {
        public List<Food> Values { get; set; }

        public Menu()
        {
            Values = new List<Food>();
            PrepareMenu();
        }

        public void PrepareMenu()
        {
            Values.AddRange(new Food[]
            {
              new Food(1, "Pizza", 20, 2, CookingApparatusType.Oven),
              new Food(2, "Salad", 10, 1, null),
              new Food(3, "Zeama", 7, 1, CookingApparatusType.Stove),
              new Food(4, "Scallop Sashimi with Meyer Lemon Confit", 32, 3, null),
              new Food(5, "Island Duck with Mulberry Mustard", 35, 3, CookingApparatusType.Oven),
              new Food(6, "Waffles", 10, 1, CookingApparatusType.Stove),
              new Food(7, "Aubergine", 20, 2,null),
              new Food(8,  "Lasagna", 20, 2, CookingApparatusType.Oven),
              new Food(9, "Burger", 15,1,CookingApparatusType.Oven),
              new Food(10, "Gyros", 15,1, null),
                new Food(11, "Kebab", 15, 1, null),
              new Food(12, "Unagi Maki", 20, 2, null),
              new Food(13, "Tobacco Chicken", 30, 2, CookingApparatusType.Oven)
            });
        }
    }
}
