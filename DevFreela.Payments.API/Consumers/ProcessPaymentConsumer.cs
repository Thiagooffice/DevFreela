﻿using DevFreela.Payments.API.Models;
using DevFreela.Payments.API.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace DevFreela.API.Consumers
{
    public class ProcessPaymentConsumer : BackgroundService
    {
        private const string QEUE = "Payments";
        private const string PAYMENT_APPROVED_QUEUE = "PaymentsApproved";
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IServiceProvider _serviceProvider;
        public ProcessPaymentConsumer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            var factory = new ConnectionFactory
            {
                HostName = "localhost",
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(
                queue: QEUE,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            _channel.QueueDeclare(
               queue: PAYMENT_APPROVED_QUEUE,
               durable: false,
               exclusive: false,
               autoDelete: false,
               arguments: null);
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (sender, eventArgs) =>
            {
                var byteArray = eventArgs.Body.ToArray();
                var paymentInfoJson = Encoding.UTF8.GetString(byteArray);

                var paymentInfo = JsonSerializer.Deserialize<PaymentInfoInputModel>(paymentInfoJson);

                ProcessPayment(paymentInfo);

                var paymentApproved = new PaymentApprovedIntegrationEvent(paymentInfo.IdProject);
                var paymentApprovedJson = JsonSerializer.Serialize(paymentApproved);
                var paymentApprovedBytes = Encoding.UTF8.GetBytes(paymentApprovedJson);

                _channel.BasicPublish(
                    exchange: "",
                    routingKey: PAYMENT_APPROVED_QUEUE,
                    basicProperties: null,
                    body: paymentApprovedBytes
                    );

                _channel.BasicAck(eventArgs.DeliveryTag, false);
            };

            _channel.BasicConsume(QEUE, false, consumer);

            return Task.CompletedTask;
        }

        public void ProcessPayment(PaymentInfoInputModel paymentInfo)
        {
            using(var scop = _serviceProvider.CreateScope())
            {
                var paymentService = scop.ServiceProvider.GetRequiredService<IPaymentService>();

                paymentService.Process(paymentInfo);
            }
        }
    }
}
