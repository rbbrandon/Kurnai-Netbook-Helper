using System;
using System.Drawing;

namespace KurnaiNetbookHelper
{
    class LWTItem
    {
        public string ItemCode { get; set; }
        public string Description { get; set; }
        public string Price { get {return String.Format("${0:0.00}", PriceFloat);} }
        public float PriceFloat { get; set; }
        public string GLCode { get; set; }
        public uint Quantity { get; set; }
        public Image Image { get; set; }

        public LWTItem(string itemCode, string description, float price, uint quantity, Image image)
        {
            ItemCode = itemCode;
            Description = description;
            PriceFloat = price;
            Quantity = quantity;
            Image = image;
            GLCode = "86402";
        }
    }
}
