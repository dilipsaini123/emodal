using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentAPI.Data;
using PaymentAPI.Models;
using PaymentAPI.Services;
using Azure.Messaging.ServiceBus;

namespace PaymentAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        
        private readonly ApplicationDbContext _context;
        private readonly PaymentService _paymentService;

        public PaymentsController(ApplicationDbContext context, PaymentService paymentService)
        {
            _context = context;
            _paymentService = paymentService;
        }

        [HttpPost]
        // [Authorize]
        public async Task<IActionResult> ProcessPayment([FromBody] PaymentRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var paymentResult = await _paymentService.ProcessPaymentAsync(request);

            if (paymentResult.IsSuccess)
                return Ok(new { Message = "Payment successful!" });

            return StatusCode(500, "Payment failed. Please try again.");
        }
    }
}
