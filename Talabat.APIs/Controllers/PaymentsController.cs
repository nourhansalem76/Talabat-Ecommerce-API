using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Talabat.APIs.Dtos;
using Talabat.APIs.Errors;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Core.Services;

namespace Talabat.APIs.Controllers
{
    
    public class PaymentsController : ApiBaseController
    {
        private readonly IPaymentService _paymentService;
        //private readonly ILogger _logger;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
            //_logger = logger;
        }
        [Authorize]
        [ProducesResponseType(typeof(CustomerBasketDto),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [HttpPost("{basketId}")]
        public async Task<ActionResult<CustomerBasketDto>> CreateOrUpdatePaymentIntent(string basketId)
        {
            var basket = await _paymentService.CreateOrUpdatePaymentIntent(basketId);
            if(basket is null) { return BadRequest(new ApiResponse(400, "There is a problem with your basket")); }
            return Ok(basket);
        }



        const string endpointSecret = "whsec_313d05d15cb78570ccad2ce16e89e918c3ab92bd5c060bc2bcc1850cbf4d6498";

        [HttpPost("webhook")]
        public async Task<IActionResult> Stripewebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
           
                var stripeEvent = EventUtility.ConstructEvent(json,
                    Request.Headers["Stripe-Signature"], endpointSecret);
                var paymentIntent= (PaymentIntent) stripeEvent.Data.Object;
                Order order;
                // Handle the event
                switch (stripeEvent.Type)
                {
                    case Events.PaymentIntentSucceeded:
                        order = await _paymentService.UpdatePaymentIntentStatus(paymentIntent.Id, true);
                        //_logger.LogInformation("Payment is succeeded", paymentIntent.Id);
                        break;

                    case Events.PaymentIntentPaymentFailed:
                        order = await _paymentService.UpdatePaymentIntentStatus(paymentIntent.Id, false);
                        //_logger.LogInformation("Payment is failed", paymentIntent.Id);
                        break;

                    default:
                        break;
                }
               
               
                return Ok();
            
          
        }
    }
}
