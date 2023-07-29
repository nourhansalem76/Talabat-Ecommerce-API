using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Order_Aggregate;

namespace Talabat.Core.Specifications.Order_Spec
{
    public class OrderWithPaymentIntentSpec: BaseSpecification<Order>
    {
        public OrderWithPaymentIntentSpec(string paymentIntent): base(O => O.PaymentIntentId == paymentIntent)
        { }
    }
}
