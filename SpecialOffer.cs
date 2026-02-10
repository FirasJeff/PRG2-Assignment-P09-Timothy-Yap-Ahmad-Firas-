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
    public class SpecialOffer
    {
        private string restaurant;
        private string offerCode;
        private string offerDesc;
        private double discount;

        public string Restaurant
        {
            get { return restaurant; }
            set { restaurant = value; }
        }

        public string OfferCode
        {
            get { return offerCode; }
            set { offerCode = value; }
        }
        public string OfferDesc
        {
            get { return offerDesc; }
            set { offerDesc = value; }
        }
        public double Discount
        {
            get { return discount; }
            set { discount = value; }
        }

        public SpecialOffer() { }

        public override string ToString()
        {
            return $"Restaurant: {restaurant}, Offer Code: {offerCode}, Description: {offerDesc}, Discount: {discount}%";
        }
    }
}
