using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Consumers.ConsumerTypes
{
    public class AlternateExchangeConsumer : BaseConsumer
    {
        public AlternateExchangeConsumer(string id, Action<string> reportingAction)
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

            channel.ExchangeDeclare("alt-exchange", ExchangeType.Fanout);

            channel.ExchangeDeclare("main-exchange", ExchangeType.Direct, arguments: new Dictionary<string, object>
            {
                {"alternate-exchange" , "alt-exchange" }
            });

            channel.QueueDeclare("main-exchange-queue",false,false,false,null);
            channel.QueueDeclare("alt-exchange-queue",false,false,false,null);

            
            
            //alt consumer 
            channel.QueueBind("alt-exchange-queue", "alt-exchange", "");
            var altConsumer = new EventingBasicConsumer(channel);

            altConsumer.Received += (sender, args) =>
            {
                var body = args.Body.ToArray();
                var decodedMsg = Encoding.UTF8.GetString(body);

                reportingAction.Invoke($"\n{decodedMsg} is Recieved from Alternate Exchange");
            };
            channel.BasicConsume("alt-exchange-queue", true, altConsumer);


            
            //main consumer
            channel.QueueBind("main-exchange-queue", "main-exchange", "");
            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (sender, args) =>
            {
                var body = args.Body.ToArray();
                var decodedMsg = Encoding.UTF8.GetString(body);

                reportingAction.Invoke($"\n{decodedMsg} is Recieved from Main Exchange");
            };

            channel.BasicConsume("main-exchange-queue", true, consumer);
        }
    }
}
