using Azure.Messaging.ServiceBus.Administration;

namespace CMH.Common.Extenstion
{
    public static class ServiceBusAdministrationClientExtension
    {
        public static async Task<List<string>> GetQueueNamesAsync(this ServiceBusAdministrationClient client,
            string queuePrefix, CancellationToken cancellationToken = default)
        {
            var queueNames = new List<string>();
            var queues = client.GetQueuesAsync(cancellationToken).GetAsyncEnumerator(cancellationToken);
            while (await queues.MoveNextAsync())
            {
                var queue = queues.Current;
                if (queue.Name.ToLower().StartsWith(queuePrefix) &&
                    queue.Status != EntityStatus.Disabled &&
                    queue.Status != EntityStatus.ReceiveDisabled)
                {
                    queueNames.Add(queue.Name);
                }
            }

            return queueNames;
        }
    }
}
