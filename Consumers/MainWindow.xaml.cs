using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Consumers
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //InitalizeQueueConsumers();
            //InitalizePubsubConsumers();
            // InitalizeTopicConsumers();
            //InitalizeRequestReplyServer();
            // InitalizeExchangeToExchangeConsumer();
            //InitalizeHeadersExchangeConsumer();
            //InitalizeAlternateExchangeConsumer();
            InitalizeDeadLetterExchangeConsumer();
        }

        private void InitalizeQueueConsumers()
        {

            for (int i = 0; i < 3; i++)
            {
                var consumer = new QueueConsumer($"{i}", (string msg) =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        MessagesLog.AppendText(msg);
                    });
                });
            }
        }

        private void InitalizePubsubConsumers()
        {

            for (int i = 0; i < 3; i++)
            {
                var consumer = new PubsubConsumer($"{i}", "pubsub", (string msg) =>
                 {
                     Dispatcher.Invoke(() =>
                     {
                         MessagesLog.AppendText(msg);
                     });
                 });
            }
        }

        private void InitalizeTopicConsumers()
        {
            var userConsumer = new TopicConsumer($"User-Service", "users.#", (string msg) =>
            {
                Dispatcher.Invoke(() =>
                {
                    MessagesLog.AppendText(msg);
                });
            });

            var paymentConsumer = new TopicConsumer($"Payment-Service", "payments.#", (string msg) =>
            {
                Dispatcher.Invoke(() =>
                {
                    MessagesLog.AppendText(msg);
                });
            });

            var AnalyticsConsumer = new TopicConsumer($"AnalyticsService", "#", (string msg) =>
            {
                Dispatcher.Invoke(() =>
                {
                    MessagesLog.AppendText(msg);
                });
            });

        }

        private void InitalizeRequestReplyServer()
        {
            var server = new RequestReplyServer($"Server",(string msg) =>
            {
                Dispatcher.Invoke(() =>
                {
                    MessagesLog.AppendText(msg);
                });
            });
        }

        private void InitalizeExchangeToExchangeConsumer()
        {
            var server = new ExchangeToExchangeConsumer($"ExchangeToExchangeComsumer", (string msg) =>
            {
                Dispatcher.Invoke(() =>
                {
                    MessagesLog.AppendText(msg);
                });
            });
        }

        private void InitalizeHeadersExchangeConsumer()
        {
            var server = new HeadersExchangeConsumer($"PaymentService", (string msg) =>
            {
                Dispatcher.Invoke(() =>
                {
                    MessagesLog.AppendText(msg);
                });
            });
        }

        private void InitalizeAlternateExchangeConsumer()
        {
            var server = new AlternateExchangeConsumer($"PaymentService", (string msg) =>
            {
                Dispatcher.Invoke(() =>
                {
                    MessagesLog.AppendText(msg);
                });
            });
        }

        private void InitalizeDeadLetterExchangeConsumer()
        {
            var server = new DeadLetterExchangeConsumer($"Alerting Service", (string msg) =>
            {
                Dispatcher.Invoke(() =>
                {
                    MessagesLog.AppendText(msg);
                });
            });
        }
    }
}
