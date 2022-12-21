using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Producer.ProducerTypes
{
    internal class TopicProducer : BaseProducer
    {
        public TopicProducer()
        {
            MediumName = "topic-exchange";
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

            Channel.ExchangeDeclare(MediumName , ExchangeType.Topic);
        }

        public override void Publish(string message, string routingKey)
        {
            var encodedMsg = Encoding.UTF8.GetBytes(message);

            Channel.BasicPublish(MediumName, routingKey, null, encodedMsg);
        }
    }
}
