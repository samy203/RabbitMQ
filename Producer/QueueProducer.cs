using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Producer
{
    internal class QueueProducer : BaseProducer
    {
        public QueueProducer()
        {
            MediumName = "letterbox";
            Configure();
        }

        public void Configure()
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
            };

            var connection = factory.CreateConnection();

            Channel = connection.CreateModel();

            Channel.QueueDeclare(MediumName, false, false, false, null);
        }

        public override void Publish(string message, string routingKey = "")
        {
            var encodedMsg = Encoding.UTF8.GetBytes(message);

            Channel.BasicPublish("", MediumName, null, encodedMsg);
        }
    }
}
