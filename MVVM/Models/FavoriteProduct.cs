using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateSettingsLookForSpecialOffers.MVVM.Models
{
    internal class FavoriteProduct
    {
        public string Name { get; set; }
        public decimal PriceCapPerProduct { get; set; }
        public decimal PriceCapPerKg { get; set; }

        public FavoriteProduct(string name, decimal priceCapPerKg, decimal priceCapPerProduct = 0)
        {
            Name = name;
            PriceCapPerProduct = priceCapPerProduct;
            PriceCapPerKg = priceCapPerKg;
        }
    }
}
