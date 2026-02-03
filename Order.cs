//========================================================== 
// Student Number : S10268547H
// Student Name : Timothy Yap 
// Partner Name : Firas
//========================================================== 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirasTimothy_PRG2Assignment
{
    class Order
    {
        public int OrderID { get; set; }
        public DateTime OrderDateTime { get; set; }
        public double OrderTotal { get; set; }
        public string OrderStatus { get; set; }
        public DateTime DeliveryDateTime { get; set; }
        public string DeliveryAddress { get; set; }
        public string OrderPaymentMethod { get; set; }
        public bool OrderPaid { get; set; }
        public List<OrderedFoodItem> orderedItems { get; set; }

        public Order() { }

        public Order(int orderid, DateTime orderdatetime, double ordertotal, string orderstatus, DateTime deliverydatetime, string deliveryaddress, string orderpaymentmethod, bool orderpaid)
        {
            OrderID = orderid;
            OrderDateTime = orderdatetime;
            OrderTotal = 0;
            OrderStatus = orderstatus;
            DeliveryDateTime = deliverydatetime;
            DeliveryAddress = deliveryaddress;
            OrderPaymentMethod = orderpaymentmethod;
            OrderPaid = orderpaid;
            orderedItems = new List<OrderedFoodItem>();
        }
        public double CalculateOrderTotal()
        {
            OrderTotal = 0;
            foreach (var item in orderedItems)
            {
                OrderTotal += item.CalculateSubTotal();
            }
            return OrderTotal;
        }
        public void AddOrderedFoodItem(OrderedFoodItem item)
        {
            orderedItems.Add(item);
            CalculateOrderTotal();
        }
        public bool RemoveOrderedFoodItem(OrderedFoodItem item)
        {
            if (orderedItems.Remove(item))
            {
                orderedItems.Remove(item);
                return true;
            }
            else
            {
                return false;
            }
        }
        public void DisplayOrderedFoodItems()
        {
            Console.WriteLine("Ordered food items \n==================");
            foreach (var item in orderedItems)
            {
                Console.WriteLine($"{item.FoodItem.ItemName, -10}: {item.QtyOrdered, 3} ${item.CalculateSubTotal(), -10}");
            }
        }

        public override string ToString()
        {
            return $"ID: {OrderID} order date and time: {OrderDateTime} Order total: ${OrderTotal} Order status: {OrderStatus} Delivery date and time: {DeliveryDateTime} Delivery Address: {DeliveryAddress} Payment method: {OrderPaymentMethod}";
        }
    }

}
