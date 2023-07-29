using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.Core.Entities.Order_Aggregate
{
    public class OrderItem : BaseEntity
    {
        public OrderItem() { }

        public OrderItem(ProductOrderItem product, decimal cost, int quantity)
        {
            Product = product;
            Cost = cost;
            Quantity = quantity;
        }

        public ProductOrderItem Product { get; set; }
        public decimal Cost { get; set; }
        public int Quantity { get; set; }

    }
}
