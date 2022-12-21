using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Consumers.ConsumerTypes
{
    internal class RequestReplyServer : BaseConsumer
    {
        public RequestReplyServer(string id, Action<string> reportingAction)
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

            channel.QueueDeclare("requests", false,false,true);

            var consumer = new EventingBasicConsumer(channel);


            consumer.Received += (sender, args) =>
            {
                var body = args.Body.ToArray();
                var decodedMsg = Encoding.UTF8.GetString(body);

                reportingAction.Invoke($"\n {decodedMsg} Recieved | CorrelationID : {args.BasicProperties.CorrelationId}");

                var msg = $"This is a reply to CorrelationID : {args.BasicProperties.CorrelationId}";
                var encodedReply = Encoding.UTF8.GetBytes(msg);
                channel.BasicPublish("", args.BasicProperties.ReplyTo, null, encodedReply);
            };

            channel.BasicConsume("requests", true, consumer);
        }
    }
}
