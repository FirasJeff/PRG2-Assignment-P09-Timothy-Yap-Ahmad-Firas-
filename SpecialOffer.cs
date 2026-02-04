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
        private string offerCode;
        private string offerDesc;
        private double discount;
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
        public SpecialOffer()
        {
            offerCode = OfferCode;
            offerDesc = OfferDesc;
            discount = Discount;
        }
        public ovveride string ToString()
        {
            return $"Offer Code: {offerCode}, Description: {offerDesc}, Discount: {discount}%";
        }
    }
}
