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
    public class Restaurant
    {
        private string restaurantId;
        private string restaurantName;
        private string restaurantEmail;
        private List<Order> orders;
        private List<Menu> menus;
        private List<SpecialOffer> specialOffers;
        private Queue<Order> restaurantqueue;
        public string RestaurantId
        {
            get { return restaurantId; }
            set { restaurantId = value; }
        }
        public string RestaurantName
        {
            get { return restaurantName; }
            set { restaurantName = value; }
        }
        public string RestaurantEmail
        {
            get { return restaurantEmail; }
            set { restaurantEmail = value; }
        }
        public Restaurant(string restaurantId, string restaurantName, string restaurantEmail)
        {
            RestaurantId = restaurantId;
            RestaurantName = restaurantName;
            RestaurantEmail = restaurantEmail;
            orders = new List<Order>();
            menus = new List<Menu>();
            specialOffers = new List<SpecialOffer>();
            restaurantqueue = new Queue<Order>();
        }
        public void DisplayOrders()
        {
            Console.WriteLine($"\n=== Orders for {restaurantName} ===");
            if (orders.Count == 0)
            { 
                Console.WriteLine("No orders received."); return; 
            }
            for (int i = 0; i < orders.Count; i++) { 
                Console.WriteLine($"{i + 1}. {orders[i]}"); 
            }
        }
        public void DisplaySpecialOffers()
        {
            Console.WriteLine($"\n=== Special Offers - {restaurantName} ===");
            if (specialOffers.Count == 0)
            {
                Console.WriteLine("No special offers available.");
                return;
            }
            for (int i = 0; i < specialOffers.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {specialOffers[i]}");
            }
        }
        public void DisplayMenu() 
        { 
            Console.WriteLine($"\n=== {restaurantName} Menu ==="); 
            if (menus.Count == 0)
            { 
                Console.WriteLine("No menus available."); 
               
            } 
            foreach (Menu menu in menus) 
            { 
                menu.DisplayFoodItems(); 
            } 
        }
        public void AddMenu(Menu menu)
        { 
            if (menu != null) 
            { 
                menus.Add(menu); 
            } 
        }
        public bool RemoveMenu(Menu menu) 
        { 
            if (menu != null) 
            { 
                return menus.Remove(menu); 
            } 
            return false; 
        }
        public void AddSpecialOffer(SpecialOffer offer) 
        { 
            if (offer != null) 
            { 
                specialOffers.Add(offer); 
            } 
        }
        public void AddOrder(Order order)
        { 
            if (order != null) 
            { 
                orders.Add(order); 
            } 
        }
        public List<Menu> GetMenus() 
        { 
            return menus; 
        }
        public List<SpecialOffer> GetSpecialOffers() 
        { 
            return specialOffers; 
        }
        public List<Order> GetOrders() 
        { 
            return orders; 
        }
        public void AddQueue(Order order)
        {
            restaurantqueue.Enqueue(order);
        }
        public Order RemoveQueue()
        {
            if (restaurantqueue.Count > 0)
            {
                return restaurantqueue.Dequeue();
            }
            return null;
        }
        public override string ToString() 
        { 
            return $"Restaurant ID: {restaurantId}, Name: {restaurantName}, " + $"Email: {restaurantEmail}, Menus: {menus.Count}, " + $"Orders: {orders.Count}, Special Offers: {specialOffers.Count}"; 
        }
    }
}

