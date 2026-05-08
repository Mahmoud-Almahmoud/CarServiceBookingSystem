using Asp.Versioning;
using CarServiceBookingSystem.Domain.Enums;
using CarServiceBookingSystem.Infrastructure.Payments;
using CarServiceBookingSystem.Infrastructure.Persistence;
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
    private readonly ApplicationDbContext _context;
    private readonly StripeSettings _stripeSettings;

    public StripeWebhookController(
        ApplicationDbContext context,
        IOptions<StripeSettings> stripeOptions)
    {
        _context = context;
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

        if (stripeEvent.Type == "payment_intent.succeeded")
        {
            var paymentIntent = stripeEvent.Data.Object as PaymentIntent;

            if (paymentIntent != null)
            {
                await MarkPaymentAsync(
                    paymentIntent.Id,
                    PaymentStatus.Succeeded);
            }
        }

        if (stripeEvent.Type == "payment_intent.payment_failed")
        {
            var paymentIntent = stripeEvent.Data.Object as PaymentIntent;

            if (paymentIntent != null)
            {
                await MarkPaymentAsync(
                    paymentIntent.Id,
                    PaymentStatus.Failed);
            }
        }

        return Ok();
    }

    private async Task MarkPaymentAsync(
        string paymentIntentId,
        PaymentStatus status)
    {
        var payment = await _context.Payments
            .Include(x => x.Booking)
            .FirstOrDefaultAsync(x => x.PaymentIntentId == paymentIntentId);

        if (payment == null)
            return;

        payment.Status = status;

        if (status == PaymentStatus.Succeeded)
        {
            payment.Booking.Status = BookingStatus.Confirmed;
        }

        await _context.SaveChangesAsync();
    }
}