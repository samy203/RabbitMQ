using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Producer.ProducerTypes
{
    public abstract class BaseProducer
    {
        public IModel Channel { get; protected set; }
        public string MediumName { get; protected set; }
        public abstract void Publish(string message, string routingKey = "");
    }

}
