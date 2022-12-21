using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Consumers.ConsumerTypes
{
    public class DeadLetterExchangeConsumer : BaseConsumer
    {
        public DeadLetterExchangeConsumer(string id, Action<string> reportingAction)
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

            channel.ExchangeDeclare("dead-letter-exchange", ExchangeType.Fanout);

            channel.ExchangeDeclare("main-exchange", ExchangeType.Direct);

            channel.QueueDeclare("main-exchange-queue", false, false, false, new Dictionary<string, object>
            {
                { "x-dead-letter-exchange" , "dead-letter-exchange" },
                { "x-message-ttl" , 1000 },
            });

            channel.QueueBind("main-exchange-queue", "main-exchange", "test");
            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (sender, args) =>
            {
                var body = args.Body.ToArray();
                var decodedMsg = Encoding.UTF8.GetString(body);

                reportingAction.Invoke($"\n{decodedMsg} is Recieved from Main Exchange");
            };

            //comment out to simulate unconsuming for the duration so it goes to dead letter exchange
            //channel.BasicConsume("main-exchange-queue", true, consumer);



            //dead letter stuff 
            channel.QueueDeclare("dead-letter-queue", false, false, false, null);

            channel.QueueBind("dead-letter-queue", "dead-letter-exchange", "");
            var deadLetterConsumer = new EventingBasicConsumer(channel);

            deadLetterConsumer.Received += (sender, args) =>
            {
                var body = args.Body.ToArray();
                var decodedMsg = Encoding.UTF8.GetString(body);

                reportingAction.Invoke($"\n{decodedMsg} is Recieved into Dead Letter Exchange");
            };

            channel.BasicConsume("dead-letter-queue", true, deadLetterConsumer);
        }
    }
}
