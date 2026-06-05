using Asp.Versioning;
using CarServiceBookingSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CarServiceBookingSystem.API.Controllers;

[ApiController]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/stripe/webhooks")]
public class StripeWebhookController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public StripeWebhookController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpPost]
    public async Task<IActionResult> Handle(CancellationToken cancellationToken)
    {
        var json = await new StreamReader(HttpContext.Request.Body)
            .ReadToEndAsync(cancellationToken);

        var stripeSignature = Request.Headers["Stripe-Signature"].ToString();

        if (string.IsNullOrWhiteSpace(stripeSignature))
        {
            return BadRequest("Missing Stripe-Signature header.");
        }

        var response = await _paymentService.HandleStripeWebhookAsync(
            json,
            stripeSignature,
            cancellationToken);

        if (!response.Success)
        {
            return BadRequest(response.Message);
        }

        return Ok();
    }
}