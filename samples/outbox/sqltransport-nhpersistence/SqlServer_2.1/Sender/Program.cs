using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using NServiceBus;

class Program
{
    static void Main()
    {
        InitLogging.Init();
        Console.Title = "Samples.SQLNHibernateOutbox.Sender";
        const string letters = "ABCDEFGHIJKLMNOPQRSTUVXYZ";
        var random = new Random();
        var busConfiguration = new BusConfiguration();
        busConfiguration.UseSerialization<JsonSerializer>();
        busConfiguration.EndpointName("Samples.SQLNHibernateOutbox.Sender");

        #region SenderConfiguration

        busConfiguration.UseTransport<SqlServerTransport>();
        busConfiguration.UsePersistence<NHibernatePersistence>();
        busConfiguration.EnableOutbox();
        //busConfiguration.Transactions().DisableDistributedTransactions();

        #endregion

        using (var bus = Bus.Create(busConfiguration).Start())
        {
            var r = new Random();

            Console.WriteLine("Press enter to publish a message");
            Console.WriteLine("Press any key to exit");
            while (!Console.KeyAvailable)
            {
                Thread.Sleep(r.Next(5000));
                Console.WriteLine("Publish...");

                var orderId = new string(Enumerable.Range(0, 4).Select(x => letters[random.Next(letters.Length)]).ToArray());
                bus.Publish(new OrderSubmitted
                {
                    OrderId = orderId,
                    Value = random.Next(100)
                });
            }
        }
    }
}
