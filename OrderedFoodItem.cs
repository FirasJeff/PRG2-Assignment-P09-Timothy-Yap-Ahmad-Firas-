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
    public class OrderedFoodItem : FoodItem
    {
        public int QtyOrdered { get; set; }
        public double SubTotal { get; set; }
        public OrderedFoodItem() { }
        public OrderedFoodItem(string itemName, string itemDesc, double itemPrice, string customise, int qtyOrdered) : base(itemName, itemDesc, itemPrice, customise)
        {
            QtyOrdered = qtyOrdered;
        }

        public double CalculateSubTotal()
        {
            SubTotal = Convert.ToDouble(QtyOrdered) * ItemPrice;
            return SubTotal;
        }

        public override string ToString()
        {
            return base.ToString() + $", Quantity: {QtyOrdered}, Subtotal: ${SubTotal:F2}";
        }
    }

}
