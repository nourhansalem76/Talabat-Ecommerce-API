using Microsoft.Extensions.Configuration;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Core.Repositries;
using Talabat.Core.Services;
using Talabat.Core.Specifications.Order_Spec;
using Product = Talabat.Core.Entities.Product;

namespace Talabat.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly IBasketRepository _basketRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PaymentService(IConfiguration configuration, IBasketRepository basketRepository,
                              IUnitOfWork unitOfWork) 
        {
            _configuration = configuration;
            _basketRepository = basketRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<CustomerBasket> CreateOrUpdatePaymentIntent(string basketId)
        {
            StripeConfiguration.ApiKey= _configuration["StripeSettings:SecretKey"];

            var basket = await _basketRepository.GetCustomerBasketAsync(basketId);
            if (basket is null) { return null; }
            var shippingPrice = 0m;
            if(basket.DeliveryMethodId.HasValue)
            {
                var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(basket.DeliveryMethodId.Value);
                basket.ShippingCost = deliveryMethod.Cost;
                shippingPrice = deliveryMethod.Cost;
            }

            if(basket?.Items?.Count> 0)
            {
         
                foreach (var item in basket.Items)
                {
                    var product = await _unitOfWork.Repository<Product>().GetByIdAsync(item.Id);
                    if(item.Price != product.Price)
                    {
                        item.Price = product.Price;
                    }
                }
            }

            var service = new PaymentIntentService();
            PaymentIntent paymentIntent;

            if(string.IsNullOrEmpty(basket.PaymentIntentId))
            {
                var options = new PaymentIntentCreateOptions()
                {
                    Amount= (long) basket.Items.Sum(item => item.Price * item.Quantity *100) + (long)shippingPrice*100,
                    Currency="usd",
                    PaymentMethodTypes = new List<string>() { "card"}
                };
              paymentIntent= await service.CreateAsync(options);
              
              basket.PaymentIntentId = paymentIntent.Id;
              basket.ClientSecret = paymentIntent.ClientSecret;
            }
            else
            {
                var options = new PaymentIntentUpdateOptions()
                {
                    Amount = (long)basket.Items.Sum(item => item.Price * item.Quantity * 100) + (long)shippingPrice * 100

                };

                await service.UpdateAsync(basket.PaymentIntentId,options);
            }

            await _basketRepository.UpdateCustomerBasketAsync(basket);

            return basket;


        }

        public async Task<Order> UpdatePaymentIntentStatus(string paymentIntentId, bool isSuccess)
        {
            var spec = new OrderWithPaymentIntentSpec(paymentIntentId);

            var order = await _unitOfWork.Repository<Order>().GetEntityAsyncSpec(spec);
            
            if (isSuccess)
                order.Status = OrderStatus.PaymentRecieved;
            else
                order.Status = OrderStatus.PaymentFailed;

            _unitOfWork.Repository<Order>().Update(order);
            await _unitOfWork.Complete();

            return order;
                
        }
    }
}
