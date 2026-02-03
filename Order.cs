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

namespace S10268547H_PRG2Assignment
{
    class FoodItem
    {
        public string ItemName { get; set; }
        public string ItemDesc { get; set; }
        public double ItemPrice { get; set; }
        public string Customise { get; set; }

        public FoodItem() { }
        public FoodItem(string itemName, string itemDesc, double itemPrice, string customise)
        {
            ItemName = itemName;
            ItemDesc = itemDesc;
            ItemPrice = itemPrice;
            Customise = customise;
        }
        public override string ToString()
        {
            return $"Item Name: {ItemName} Item Description: {ItemDesc} Item Price: {ItemPrice} Customise: {Customise}";
        }
    }

    class OrderedFoodItem
    {
        public int QtyOrdered { get; set; }
        public double SubTotal { get; set; }
        public FoodItem FoodItem { get; set; }
        public OrderedFoodItem() { }
        public OrderedFoodItem(int qtyOrdered, double subTotal, FoodItem fooditem)
        {
            QtyOrdered = qtyOrdered;
            SubTotal = subTotal;
            FoodItem = fooditem;
        }
        public double CalculateSubTotal()
        {
            SubTotal = Convert.ToDouble(QtyOrdered) * FoodItem.ItemPrice;
            return SubTotal;
        }

    }

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



    }

}
