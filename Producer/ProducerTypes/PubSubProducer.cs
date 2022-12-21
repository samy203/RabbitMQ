using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Producer.ProducerTypes
{
    internal class PubSubProducer : BaseProducer
    {
        public PubSubProducer()
        {
            MediumName = "pubsub";
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

            Channel.ExchangeDeclare(MediumName , ExchangeType.Fanout);
        }

        public override void Publish(string message,string routingKey)
        {
            var encodedMsg = Encoding.UTF8.GetBytes(message);

            Channel.BasicPublish(MediumName, "", null, encodedMsg);
        }
    }
}
