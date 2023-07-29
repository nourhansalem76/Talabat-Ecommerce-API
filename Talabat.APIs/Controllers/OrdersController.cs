using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Talabat.APIs.Dtos;
using Talabat.APIs.Errors;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Core.Specifications.Order_Spec;
using Talabat.Service;

namespace Talabat.APIs.Controllers
{
    [Authorize]
    public class OrdersController : ApiBaseController
    {
        
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;

        
        public OrdersController(IOrderService orderService, IMapper mapper)
        {
            _orderService = orderService;
            _mapper = mapper;
        }
        
        [HttpPost]
        public async Task<ActionResult<OrdersDTO>> CreateOrder(OrderDto orderDto)
        {
            var buyerEmail = User.FindFirstValue(ClaimTypes.Email);
            var shippingAddress = _mapper.Map<AddressDto, Address>(orderDto.ShipToAddress);

            var order = await _orderService.CreateOrderAsync(buyerEmail, orderDto.BasketId, orderDto.DeliveryMethodId, shippingAddress);
            if(order is null) { return BadRequest(new ApiResponse(400)); }
            return Ok(_mapper.Map<Order,OrdersDTO>(order));

        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<OrdersDTO>>> GetOrdersForUser()
        {
            var buyerEmail = User.FindFirstValue(ClaimTypes.Email);
            var orders = await _orderService.GetOrdersForUserAsync(buyerEmail);
            if(orders is null) { return NotFound(new ApiResponse(404)); }
            return Ok(_mapper.Map<IReadOnlyList<Order>,IReadOnlyList<OrdersDTO>>(orders));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrdersDTO>> GetOrderForUserById(int id)
        {
            var buyerEmail = User.FindFirstValue(ClaimTypes.Email);
            var order = await _orderService.GetOrderByIdAsync(buyerEmail, id);
            if(order is null) { return NotFound(new ApiResponse(404)); }
            return Ok(_mapper.Map<Order, OrdersDTO>(order));
            
        }

        [HttpGet("deliveryMethods")]
        public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethods()
        {
            var DeliveryMethods = await _orderService.GetDeliveryMethodsAsync();
            if(DeliveryMethods is null) { return NotFound(new ApiResponse(404)); }
            return Ok(DeliveryMethods);
        }
    }
}
