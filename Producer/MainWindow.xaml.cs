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

namespace Producer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BaseProducer producer;

        public MainWindow()
        {
            InitializeComponent();
            //   InitializeQueueProducer();
            //  InitializePubsubProducer();
            // InitializeTopicProducer();
            //InitializeRequestReplyClient();
            //InitializeExchangeToExchangeProducer();
            // InitializeHeadersExchangeProducer();
            //InitializeAltExchangeProducer();
            InitializeDeadLetterExchangeProducer();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxItem typeItem = (ComboBoxItem)Topics.SelectedItem;
            var topic = typeItem?.Content?.ToString();

            producer.Publish(Message.Text, topic ?? "");
        }

        private void InitializeQueueProducer()
        {
            producer = new QueueProducer();
        }

        private void InitializePubsubProducer()
        {
            producer = new PubSubProducer();
        }

        private void InitializeTopicProducer()
        {
            producer = new TopicProducer();
        }

        private void InitializeRequestReplyClient()
        {
            var client = new RequestReplyClient();

            client.Configure((string msg) =>
            {
                Dispatcher.Invoke(() =>
                {
                    MessagesLog.AppendText(msg);
                });
            });

            producer = client;
        }

        private void InitializeExchangeToExchangeProducer()
        {
            producer = new ExchangeToExchangeProducer();
        }

        private void InitializeHeadersExchangeProducer()
        {
            producer = new HeadersExchangeProducer();
        }

        private void InitializeAltExchangeProducer()
        {
            producer = new AlternateExchangeProducer();
        }

        private void InitializeDeadLetterExchangeProducer()
        {
            producer = new DeadLetterExchangeProducer();
        }
    }
}
