using System;
using System.Collections.Generic;
using System.Text;

namespace CarServiceBookingSystem.Domain.Enums
{
    public enum WebhookProcessingStatus
    {
        Processed,
        AlreadyProcessed,
        Ignored,
        Invalid,
        Failed
    }
}
