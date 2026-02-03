using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirasTimothy_PRG2Assignment
{
    class Customer
    {
        public string EmailAddress { get; set; }
        public string CustomerName { get; set; }
        public List<Order> orders { get; set; }
        public Customer() { }
        public Customer(string emailAddress, string customerName, Order customerorder)
        {
            EmailAddress = emailAddress;
            CustomerName = customerName;
            orders = new List<Order>();
        }
        public void AddOrder(Order addedorder)
        {
            orders.Add(addedorder);
        }
        public void DisplayAllOrders()
        {
            Console.WriteLine("All your orders:");
            Console.WriteLine("================\n");
            Console.WriteLine($"{"Order ID",-15} {"Ordered Date",-12} {"Total",-6} {"Order Status",8} {"Delivery arrival",-12} {"Delivery Address",20} {"PaymentMethod",8}");
            foreach(Order order in orders)
            {
                Console.WriteLine(order.ToString());
            }
        }
    }
}
