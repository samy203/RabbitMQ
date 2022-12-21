using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Producer
{
    internal class RequestReplyClient : BaseProducer
    {
        private string replyTo;
        public RequestReplyClient()
        {
            MediumName = "requests";
        }

        public void Configure(Action<string> reportingAction)
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
            };

            var connection = factory.CreateConnection();

            Channel = connection.CreateModel();

            replyTo = Channel.QueueDeclare("replies", false,false,true).QueueName;

            Channel.QueueDeclare(MediumName, false,false,true);

            var consumer = new EventingBasicConsumer(Channel);

            consumer.Received += (sender, args) =>
            {
                var body = args.Body.ToArray();
                var decodedMsg = Encoding.UTF8.GetString(body);
                reportingAction.Invoke($"\n Reply recieved {decodedMsg}");
            };

            Channel.BasicConsume(replyTo, true, consumer);
        }

        public override void Publish(string message, string routingKey = "")
        {
            var encodedMsg = Encoding.UTF8.GetBytes(message);

            var properties = Channel.CreateBasicProperties();
            properties.ReplyTo = replyTo;
            properties.CorrelationId = Guid.NewGuid().ToString();

            Channel.BasicPublish("", MediumName, properties, encodedMsg);
        }
    }
}
