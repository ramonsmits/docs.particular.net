using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus;

class Program
{
    private const string connectionString = @"Data Source=(localdb)\v11.0;Initial Catalog=Samples.SqlIntegration;Integrated Security=True";
    public static CountdownEvent batchFinished;

    private static void Main()
    {
        #region EndpointConfiguration

        BusConfiguration busConfiguration = new BusConfiguration();

        busConfiguration.UseTransport<SqlServerTransport>()
            .ConnectionString(connectionString);
        busConfiguration.EndpointName("Samples.SqlServer.NativeIntegration");
        busConfiguration.UseSerialization<JsonSerializer>();

        #endregion

        busConfiguration.UsePersistence<InMemoryPersistence>();
        busConfiguration.EnableInstallers();

        const int BatchCount = 1000;

        using (batchFinished = new CountdownEvent(BatchCount))
        {
            using (Bus.Create(busConfiguration).Start())
            {
                Console.WriteLine("Press CTRL+C to exit");

                while (true)
                {
                    Parallel.For(0, BatchCount, i =>
                    {
                        PlaceOrder();
                    });

                    batchFinished.Wait();
                    batchFinished.Reset(BatchCount);
                    Console.WriteLine("Refilling queue, CTRL+C to quit.");
                }
            }
        }
    }

    private static int _orderId;

    static void PlaceOrder()
    {
        #region MessagePayload

        string message = @"{
                               $type: 'PlaceOrder',
                               OrderId: '" + System.Threading.Interlocked.Increment(ref _orderId) + @"'
                            }";

        #endregion

        #region SendingUsingAdoNet

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            string insertSql = @"INSERT INTO [Samples.SqlServer.NativeIntegration] ([Id],[Recoverable],[Headers],[Body]) VALUES (@Id,@Recoverable,@Headers,@Body)";
            using (SqlCommand command = new SqlCommand(insertSql, connection))
            {
                command.CommandType = CommandType.Text;

                command.Parameters.Add("Id", SqlDbType.UniqueIdentifier).Value = Guid.NewGuid();
                command.Parameters.Add("Headers", SqlDbType.VarChar).Value = "";
                command.Parameters.Add("Body", SqlDbType.VarBinary).Value = Encoding.UTF8.GetBytes(message);
                command.Parameters.Add("Recoverable", SqlDbType.Bit).Value = true;

                command.ExecuteNonQuery();
            }
        }

        #endregion
    }
}
