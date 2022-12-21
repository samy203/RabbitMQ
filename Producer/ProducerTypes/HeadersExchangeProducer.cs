using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Producer.ProducerTypes
{
    internal class HeadersExchangeProducer : BaseProducer
    {
        public HeadersExchangeProducer()
        {
            MediumName = "headers-exchange";
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

            Channel.ExchangeDeclare(MediumName, ExchangeType.Headers);
        }

        public override void Publish(string message, string routingKey = "")
        {
            var properties = Channel.CreateBasicProperties();

            properties.Headers = new Dictionary<string, object>{
                {"type", "payment"},
                {"region","eu" }
            };

            var encodedMsg = Encoding.UTF8.GetBytes(message);

            Channel.BasicPublish(MediumName, "", properties, encodedMsg);
        }
    }
}
