using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Order_Aggregate;

namespace Talabat.Core.Specifications.Order_Spec
{
    public class OrderSpecifications : BaseSpecification<Order>
    {
        public OrderSpecifications(string email): base(O => O.BuyerEmail == email ) 
        {
            Includes.Add(O => O.DeliveryMethod);
            Includes.Add(O => O.Items);

            AddOrderbyDesc(O => O.OrderDate);
        }

        public OrderSpecifications(string email, int orderid) : base(O => O.BuyerEmail == email && O.Id == orderid)
        {
            Includes.Add(O => O.DeliveryMethod);
            Includes.Add(O => O.Items);

        }
    }
}
