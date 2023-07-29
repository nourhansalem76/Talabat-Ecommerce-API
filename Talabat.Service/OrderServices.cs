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

namespace Talabat.Service
{
    public class OrderServices : IOrderService
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentService _paymentService;

        //private readonly IGenericRepository<Product> _productRepo;
        //private readonly IGenericRepository<DeliveryMethod> _deliveryMethodRepo;
        //private readonly IGenericRepository<Order> _orderRepo;

        public OrderServices(IBasketRepository basketRepository, IUnitOfWork unitOfWork,
                             IPaymentService paymentService    )
        {
            _basketRepository = basketRepository;
            _unitOfWork = unitOfWork;
            _paymentService = paymentService;
        }
        public async Task<Order?> CreateOrderAsync(string buyerEmail, string basketId, int deliveryMethod, Address address)
        {
            var basket = await _basketRepository.GetCustomerBasketAsync(basketId);

            var OrderItems = new List<OrderItem>();
            
            if(basket?.Items.Count > 0)
            {
                foreach (var item in basket.Items)
                {
                    var productRepo = _unitOfWork.Repository<Product>();
                    if (productRepo is not null)
                    {
                        var product = await productRepo.GetByIdAsync(item.Id);
                        if (product is not null)
                        {
                            var ProductOrderedItem = new ProductOrderItem(product.Id, product.Name, product.PictureUrl);
                            var OrderItem = new OrderItem(ProductOrderedItem, item.Price, item.Quantity);
                            OrderItems.Add(OrderItem);
                        }
                        
                    }

                }
            }

            var subTotal = OrderItems.Sum(orderItem => orderItem.Cost * orderItem.Quantity);
            var deliveryMethodRepo = _unitOfWork.Repository<DeliveryMethod>();
            DeliveryMethod DeliveryMethod = new DeliveryMethod();
            if (deliveryMethodRepo is not null)
            {
                 DeliveryMethod = await deliveryMethodRepo.GetByIdAsync(deliveryMethod);
            }

            var spec = new OrderWithPaymentIntentSpec(basket.PaymentIntentId);
            var exisitingOrder = await _unitOfWork.Repository<Order>().GetEntityAsyncSpec(spec);
            if(exisitingOrder is not null)
            {
                 _unitOfWork.Repository<Order>().Delete(exisitingOrder);
                _paymentService.CreateOrUpdatePaymentIntent(basket.Id);
            }
            var order = new Order(buyerEmail, address, DeliveryMethod, OrderItems, subTotal, basket.PaymentIntentId);

            var orderRepo = _unitOfWork.Repository<Order>();

            if (orderRepo is not null)
            {
                await orderRepo.AddAsync(order);

                var result = await _unitOfWork.Complete();

                if (result > 0)
                    return order;

            }

            return null;

        }

        public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync()
        {
            var deliveryMethods = await _unitOfWork.Repository<DeliveryMethod>().GetAllAsync();
            return deliveryMethods;
        }

        public async Task<Order?> GetOrderByIdAsync(string buyerEmail, int orderId)
        {
            var spec = new OrderSpecifications(buyerEmail, orderId);
            var Order = await _unitOfWork.Repository<Order>().GetEntityAsyncSpec(spec);
          
            return Order;
        }

        public async Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string buyerEmail)
        {
            var spec = new OrderSpecifications(buyerEmail);
            var Orders = await _unitOfWork.Repository<Order>().GetAllAsyncSpec(spec);
            return Orders;
        }

        
    }
}
