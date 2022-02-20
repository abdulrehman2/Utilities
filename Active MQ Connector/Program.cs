using Apache.NMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connector
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                string text = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(text)) return;
                SendNewMessageQueue("Hello");
                ReadNextMessageQueue();
                Console.ReadLine();
            }
        }

        private static void SendNewMessageQueue(string text)
        {
            try
            {
                string queueName = "DLQ";

                Console.WriteLine($"Adding message to queue topic: {queueName}");


                //?wireFormat.maxInactivityDuration=0
                //string brokerUri = $"activemq:tcp://127.0.0.1:8080?wireFormat.maxInactivityDuration=0";  // Default port

                //string brokerUri = $"activemq:failover:(tcp://127.0.0.1:8080)?wireFormat.maxInactivityDuration=0";  // Default port



                //string brokerUri = $"activemq:failover://(discovery://127.0.0.1:6=5672)?transport.randomize=true&transport.startupMaxReconnectAttempts=1&transport.timeout=2000";  // Default port

                string brokerUri = "activemq:tcp://127.0.0.1:61616";






                var factory = new Apache.NMS.ActiveMQ.ConnectionFactory(brokerUri);

                using (IConnection connection = factory.CreateConnection())
                {
                    connection.Start();

                    using (ISession session = connection.CreateSession(AcknowledgementMode.AutoAcknowledge))
                    using (IDestination dest = session.GetQueue(queueName))
                    using (IMessageProducer producer = session.CreateProducer(dest))
                    {
                        producer.DeliveryMode = MsgDeliveryMode.NonPersistent;

                        producer.Send(session.CreateTextMessage(text));
                        Console.WriteLine($"Sent {text} messages");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }




        static void ReadNextMessageQueue()
        {
            string queueName = "DLQ";

            string brokerUri = "activemq:tcp://127.0.0.1:61616";
            NMSConnectionFactory factory = new NMSConnectionFactory(brokerUri);

            using (IConnection connection = factory.CreateConnection())
            {
                connection.Start();
                using (ISession session = connection.CreateSession(AcknowledgementMode.AutoAcknowledge))
                using (IDestination dest = session.GetQueue(queueName))
                using (IMessageConsumer consumer = session.CreateConsumer(dest))
                {
                    IMessage msg = consumer.Receive();
                    if (msg is ITextMessage)
                    {
                        ITextMessage txtMsg = msg as ITextMessage;
                        string body = txtMsg.Text;

                        Console.WriteLine($"Received message: {txtMsg.Text}");
                    }
                    else
                    {
                        Console.WriteLine("Unexpected message type: " + msg.GetType().Name);
                    }
                }
            }
        }
    }
}
