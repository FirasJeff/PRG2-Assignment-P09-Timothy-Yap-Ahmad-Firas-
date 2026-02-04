//========================================================== 
// Student Number : S10273408F
// Student Name : Firas 
// Partner Name : Timothy
//========================================================== 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirasTimothy_PRG2Assignment
{
    public class Menu
    {
        private string menuId;
        private string menuName;
        private List<FoodItem> foodItems;
        public string MenuId
        {
            get { return menuId; }
            set { menuId = value; }
        }
        public string MenuName
        {
            get { return menuName; }
            set { menuName = value; }
        }
        public Menu(string menuId, string menuName)
        {
            MenuId = menuId;
            MenuName = menuName;
            foodItems = new List<FoodItem>();
        }
        public void AddFoodItem(FoodItem fooditem)
        {
            if (fooditem != null)
            {
                foodItems.Add(fooditem);
            }
        }
        public bool RemoveFoodItem(FoodItem fooditem)
        {
            if (fooditem != null)
            {
                return foodItems.Remove(fooditem);
            }
            else
            {
                return false;
            }
        }
        public void DisplayFoodItems()
        {
            Console.WriteLine($"\n=== {menuName} Menu ===");
            if (foodItems.Count == 0)
            {
                Console.WriteLine("No food items available."); return;
            }
            for (int i = 0; i < foodItems.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {foodItems[i]}");
            }
        }
        public List<FoodItem> GetFoodItems()
        {
            return foodItems;
        }
        public override string ToString()
        {
            return $"Menu ID: {menuId}, Menu Name: {menuName}, Items Count: {foodItems.Count}";
        }

    }
}
