using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class CartItems
    {
        public int ProductId { get; set; }
        public string ProductName {  get; set; }
        public decimal Price { get; set; }

        public int Quntity { get; set; }

        public required string PictureUrl { get; set; }

        public required string Brand{ get; set; }
        public required string Type { get; set; }
    }
}
