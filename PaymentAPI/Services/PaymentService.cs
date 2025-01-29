using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using PaymentAPI.Data;
using PaymentAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.Text.Json;

namespace PaymentAPI.Services
{
    public class PaymentService
    {
        private readonly ApplicationDbContext _context;
        private readonly CosmosDbService _cosmosDbService;
        private readonly string _serviceBusConnectionString;
        private readonly string _topicName;
        private readonly ServiceBusClient _serviceBusClient;
        private readonly ServiceBusSender _serviceBusSender;
        private readonly ServiceBusReceiver _serviceBusReceiver;

        public PaymentService(ApplicationDbContext context, CosmosDbService cosmosDbService, IConfiguration configuration)
        {
            _context = context;
            _cosmosDbService = cosmosDbService;
            _serviceBusConnectionString = configuration.GetValue<string>("AzureServiceBus:ConnectionString");
            _topicName = configuration.GetValue<string>("AzureServiceBus:TopicName");
            _serviceBusClient = new ServiceBusClient(_serviceBusConnectionString);
            _serviceBusSender = _serviceBusClient.CreateSender(_topicName);
            _serviceBusReceiver = _serviceBusClient.CreateReceiver(configuration["AzureServiceBus:TopicName"], configuration["AzureServiceBus:SubscriptionName"]);
        }

        public async Task<(bool IsSuccess, string Message)> ProcessPaymentAsync(PaymentRequest request)
        {
            try
            {
                await SendMessageToServiceBus(request);
                Console.WriteLine("Message sent to Service Bus");
                await ReceiveMessageFromServiceBus();
                Console.WriteLine("Message received from Service Bus");

                return (true, "Payment processed, containers updated, and confirmation sent.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing payment: {ex.Message}");
                return (false, "An error occurred while processing the payment.");
            }
        }

        private async Task SendMessageToServiceBus(PaymentRequest request)
        {
            try
            {
                var sender = _serviceBusClient.CreateSender(_topicName);

                var messageBody = new
                {
                    Containers = request.Containers.Select(c => new { c.ContainerId , c.Fees } ).ToList(),
                    request.CardNumber,
                    request.CVV,
                    request.Expiry,
                    request.TotalFees,
                    request.Username
                };

                string messageContent = JsonSerializer.Serialize(messageBody);

                var serviceBusMessage = new ServiceBusMessage(messageContent);

                await sender.SendMessageAsync(serviceBusMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending payment confirmation to Service Bus: {ex.Message}");
            }
        }

        private async Task ReceiveMessageFromServiceBus()
        {
            try
            {
                var receivedMessage = await _serviceBusReceiver.ReceiveMessageAsync();
                Console.WriteLine($"Received message from Service Bus: {receivedMessage.Body}");

                string messageBody = receivedMessage.Body.ToString();
                PaymentRequest containerMessage = JsonSerializer.Deserialize<PaymentRequest>(messageBody);
                if (containerMessage != null)
                {
                    await UpdatePaymentInDatabase(containerMessage);
                    await _serviceBusReceiver.CompleteMessageAsync(receivedMessage);
                }
                else
                {
                    Console.WriteLine("Failed to deserialize the message body.");
                }

                // await _serviceBusReceiver.CloseAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error receiving payment confirmation from Service Bus: {ex.Message}");
            }
        }

        private async Task UpdatePaymentInDatabase(PaymentRequest containerMessage)
        {
            try
            {
                foreach (var container in containerMessage.Containers)
                {
                    var payment = new Payment
                    {
                        TransactionId = Guid.NewGuid().ToString(),
                        CardNumber = containerMessage.CardNumber,
                        TotalFees = containerMessage.TotalFees,
                        PaymentDate = DateTime.UtcNow,
                        Status = "Successful",
                        Username = containerMessage.Username,
                        ContainerId = container.ContainerId,
                        ContainerFee = container.Fees
                    };
                    _context.Payments.Add(payment);
                    await _context.SaveChangesAsync();
                }

                var updateTasks = new List<Task>();

                foreach (var container in containerMessage.Containers)
                {
                    var updateTask = _cosmosDbService.UpdateContainerInCosmosDb(container.ContainerId);
                    updateTasks.Add(updateTask);
                }

                await Task.WhenAll(updateTasks);

                Console.WriteLine("All containers updated successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating container in database: {ex.Message}");
            }
        }
    }
}
