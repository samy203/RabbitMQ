using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Consumers
{
    public class HeadersExchangeConsumer : BaseConsumer
    {
        public HeadersExchangeConsumer(string id, Action<string> reportingAction)
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

            channel.ExchangeDeclare("headers-exchange", ExchangeType.Headers);

            channel.QueueDeclare("letterbox", false, false, false, null);

            var bindingArguments = new Dictionary<string, object>{
                {"x-match", "all"},
                {"type", "payment"},
                {"region", "eu"}
        };

            channel.QueueBind("letterbox", "headers-exchange", "", bindingArguments);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (sender, args) =>
            {
                var body = args.Body.ToArray();
                var decodedMsg = Encoding.UTF8.GetString(body);

                reportingAction.Invoke($"\n{decodedMsg} is Recieved using Header Exchange");
            };

            channel.BasicConsume("letterbox", true, consumer);
        }
    }
}
