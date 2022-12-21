using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Consumers
{
    public class PubsubConsumer : BaseConsumer
    {
        private string exchangeName;
        public PubsubConsumer(string id,string exchangeName, Action<string> reportingAction)
        {
            this.id = id;
            this.exchangeName = exchangeName;
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

            channel.ExchangeDeclare(exchangeName, ExchangeType.Fanout);

            var queueName = channel.QueueDeclare().QueueName;

            var consumer = new EventingBasicConsumer(channel);

            channel.QueueBind(queueName, exchangeName, "");


            consumer.Received += (sender, args) =>
            {
                var body = args.Body.ToArray();
                var decodedMsg = Encoding.UTF8.GetString(body);
                reportingAction.Invoke($"\n{decodedMsg} is being processed by node id : {id}");
            };

            channel.BasicConsume(queueName, true, consumer);
        }
    }
}
