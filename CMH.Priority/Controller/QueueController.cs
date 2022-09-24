using Azure.Messaging.ServiceBus.Administration;
using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Mvc;

using CMH.Common.Extenstion;
using CMH.Common.Message;
using CMH.Data.Repository;
using CMH.Common.Variable;

namespace CMH.Priority.Controller
{
    [ApiController]
    [Route("v1/[controller]")]
    public class QueueController : ControllerBase
    {
        private readonly IDataSourceRepository _dataSourceRepository;
        private readonly ServiceBusClient _serviceBusClient;
        private readonly ServiceBusAdministrationClient _serviceBusAdministrationClient;

        public QueueController(
            IDataSourceRepository dataSourceRepository,
            ServiceBusClient serviceBusClient,
            ServiceBusAdministrationClient serviceBusAdministrationClient)
        {
            _dataSourceRepository = dataSourceRepository;
            _serviceBusClient = serviceBusClient;
            _serviceBusAdministrationClient = serviceBusAdministrationClient;
        }

        [HttpGet]
        [Route("{queuePrefix}")]
        public async Task<List<string>> GetQueueNames(string queuePrefix)
        {
            return await _serviceBusAdministrationClient.GetQueueNamesAsync(queuePrefix);
        }

        [HttpPost]
        [Route("reset")]
        public async Task ResetQueues()
        {
            var queues = _serviceBusAdministrationClient.GetQueueNamesAsync("").Result;

            foreach (var queue in queues)
            {
                await _serviceBusAdministrationClient.DeleteQueueAsync(queue);
                await _serviceBusAdministrationClient.CreateQueueAsync(queue);
            }
        }

        [HttpPost]
        [Route("send/{nbrOfMessages}")]
        public async Task SendMessages(int nbrOfMessages, string? queueName, short? dataSourceId)
        {
            var maxMessageBatch = 500;
            var maxParallellWriteTasks = 10;

            var random = new Random();
            var priorityQueues = await _serviceBusAdministrationClient.GetQueueNamesAsync(Queue.PriorityQueuePrefix);
            var dataSources = _dataSourceRepository.GetAll();
            var tasks = new List<Task>();

            while (nbrOfMessages > 0)
            {
                var messages = new List<ServiceBusMessage>();
                var messageBatch = nbrOfMessages >= maxMessageBatch ? maxMessageBatch : nbrOfMessages;

                for (var i = 0; i < messageBatch; i++)
                {
                    var dataSource = dataSources.FirstOrDefault(_ => _.Id == dataSourceId) ?? dataSources[random.Next(dataSources.Count)];
                    messages.Add(MessageHelper.CreateMessage(dataSource.Id));
                }

                var priorityQueue = priorityQueues.FirstOrDefault(_ => _ == queueName) ?? priorityQueues[random.Next(priorityQueues.Count)];
                var sender = _serviceBusClient.CreateSender(priorityQueue);
                nbrOfMessages -= messages.Count;

                var task = sender.SendMessagesAsync(messages);
                tasks.Add(task);

                while(tasks.Count(_ => _.IsCompleted == false) >= maxParallellWriteTasks)
                {
                    Task.WaitAny(tasks.ToArray());
                }
            }

            Task.WaitAll(tasks.ToArray());
        }
    }
}