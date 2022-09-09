using Azure.Messaging.ServiceBus;

namespace CMH.Common.Extenstion
{
    public static class ServiceBusSenderExtension
    {
        public static async Task RescheduleMessageAsync(this ServiceBusSender sender, 
            ServiceBusReceivedMessage message, DateTimeOffset scheduledEnqueueTime,
            CancellationToken cancellationToken = default)
        {
            //Schedule new message
            var resendMessage = new ServiceBusMessage(message);
            var tries = resendMessage.ApplicationProperties.ContainsKey("Tries") ? (int)resendMessage.ApplicationProperties["Tries"] : 0;
            resendMessage.ApplicationProperties["Tries"] = tries + 1;
            await sender.ScheduleMessageAsync(resendMessage, scheduledEnqueueTime, cancellationToken);
        }
    }
}
