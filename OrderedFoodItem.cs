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

}
