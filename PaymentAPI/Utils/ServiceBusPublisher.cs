using Azure.Messaging.ServiceBus;
using System.Text.Json;

namespace PaymentAPI.Utils
{
    public class ServiceBusPublisher
    {
        private readonly string _serviceBusConnectionString;
        private readonly string _queueName;

        public ServiceBusPublisher(string serviceBusConnectionString, string queueName)
        {
            _serviceBusConnectionString = serviceBusConnectionString;
            _queueName = queueName;
        }

        public async Task PublishAsync(object message)
        {
            await using var client = new ServiceBusClient(_serviceBusConnectionString);
            var sender = client.CreateSender(_queueName);
            var messageBody = JsonSerializer.Serialize(message);
            var serviceBusMessage = new ServiceBusMessage(messageBody);

            await sender.SendMessageAsync(serviceBusMessage);
        }
    }
}
