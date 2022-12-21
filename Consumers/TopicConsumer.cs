using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Consumers
{
    public class TopicConsumer : BaseConsumer
    {
        private string topic;
        public TopicConsumer(string id, string topic, Action<string> reportingAction)
        {
            this.id = id;
            this.topic = topic;
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

            channel.ExchangeDeclare("topic-exchange", ExchangeType.Topic);

            var queueName = channel.QueueDeclare().QueueName;

            var consumer = new EventingBasicConsumer(channel);

            channel.QueueBind(queueName, "topic-exchange", topic);


            consumer.Received += (sender, args) =>
            {
                var body = args.Body.ToArray();
                var decodedMsg = Encoding.UTF8.GetString(body);
                reportingAction.Invoke($"\n Consumer {id} recieved {decodedMsg}");
            };

            channel.BasicConsume(queueName, true, consumer);
        }
    }
}
