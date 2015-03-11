using System;
using Messages;
using NServiceBus;

namespace Receiver
{
    using NHibernate;

    public class OrderSubmittedHandler : IHandleMessages<OrderSubmitted>
    {
        static readonly Random ChaosGenerator = new Random();

        public IBus Bus { get; set; }

        public void Handle(OrderSubmitted message)
        {
            Console.WriteLine("Order {0} worth {1} submitted", message.OrderId, message.Value);

            using (ISession session = Program.SessionFactory.OpenSession())
            using (ITransaction tx = session.BeginTransaction())
            {
                session.Save(new Order
                             {
                                 OrderId = message.OrderId,
                                 Value = message.Value
                             });
                tx.Commit();
            }

			Bus.Reply(new OrderAccepted
                      {
                          OrderId = message.OrderId,
                      });

            if (ChaosGenerator.Next(2) == 0)
            {
                throw new Exception("Boom!");
            }
        }
    }
}