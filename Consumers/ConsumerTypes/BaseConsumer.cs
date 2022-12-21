using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Consumers.ConsumerTypes
{
    public abstract class BaseConsumer
    {
        protected string id;
        protected IModel channel;
        protected IConnection connection;
        protected Action<string> reportingAction;
    }
}
