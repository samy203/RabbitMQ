using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Consumers
{
    public class QueueConsumer : BaseConsumer
    {
        public QueueConsumer(string id, Action<string> reportingAction)
        {
            this.id = id;
            this.reportingAction = reportingAction;
            Configure();
        }


        private void Configure()
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
            };

            if (connection == null || !connection.IsOpen)
                connection = factory.CreateConnection();

            channel = connection.CreateModel();

            channel.QueueDeclare("letterbox", false, false, false, null);

            channel.BasicQos(0, 1, false);

            var consumer = new EventingBasicConsumer(channel);

            var random = new Random();

            consumer.Received += (sender, args) =>
            {
                var processingTime = random.Next(1, 6);
                var body = args.Body.ToArray();
                var decodedMsg = Encoding.UTF8.GetString(body);

                reportingAction.Invoke($"\n{decodedMsg} is being processed by node id : {id} | ETA : {processingTime} Seconds");

                Task.Delay(TimeSpan.FromSeconds(processingTime)).Wait();

                channel.BasicAck(args.DeliveryTag, false);
            };

            channel.BasicConsume("letterbox", false, consumer);
        }
    }
}
