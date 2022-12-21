using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Consumers
{
    public class ExchangeToExchangeConsumer : BaseConsumer
    {
        public ExchangeToExchangeConsumer(string id, Action<string> reportingAction)
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

            channel.ExchangeDeclare("final-exchange", ExchangeType.Fanout);

            channel.QueueDeclare("letterbox",false,false,false,null);

            channel.QueueBind("letterbox", "final-exchange", "0");

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (sender, args) =>
            {
                var body = args.Body.ToArray();
                var decodedMsg = Encoding.UTF8.GetString(body);

                reportingAction.Invoke($"\n{decodedMsg} is Recieved from final-exchange");
            };

            channel.BasicConsume("letterbox", true, consumer);
        }
    }
}
