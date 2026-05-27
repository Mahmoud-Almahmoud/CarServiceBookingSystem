using Asp.Versioning;
using CarServiceBookingSystem.Application.Interfaces;
using CarServiceBookingSystem.Domain.Enums;
using CarServiceBookingSystem.Infrastructure.Payments;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.V2.Core;

namespace CarServiceBookingSystem.API.Controllers;

[ApiController]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/[controller]")]
public class StripeWebhookController : ControllerBase
{
    private readonly IPaymentService _service;
    private readonly StripeSettings _stripeSettings;

    public StripeWebhookController(
        IPaymentService service,
        IOptions<StripeSettings> stripeOptions)
    {
        _service = service;
        _stripeSettings = stripeOptions.Value;
    }

    [HttpPost]
    public async Task<IActionResult> Handle()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

        Stripe.Event stripeEvent;

        try
        {
            stripeEvent = EventUtility.ConstructEvent(
                json,
                Request.Headers["Stripe-Signature"],
                _stripeSettings.WebhookSecret
            );
        }
        catch (StripeException)
        {
            return BadRequest();
        }

        var paymentIntent = stripeEvent.Data.Object as PaymentIntent;

        var stripeWebhookDto = new StripeWebhookDto
        {
            EventId = stripeEvent.Id,
            EventType = stripeEvent.Type,
            PaymentIntentId = paymentIntent?.Id,
            Amount = paymentIntent?.Amount ?? 0,
            Currency = paymentIntent?.Currency
        };
        var result = await _service.HandleStripeWebhookAsync(stripeWebhookDto);
        if (!result.Success && result.Data == WebhookProcessingStatus.Invalid)
        {
            return BadRequest(result.Message);
        }

        return Ok();
    }
}