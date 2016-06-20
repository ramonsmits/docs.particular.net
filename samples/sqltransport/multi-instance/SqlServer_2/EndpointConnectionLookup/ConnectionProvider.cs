using System;
using NServiceBus.Transports.SQLServer;

#region ConnectionProvider
public class ConnectionProvider
{
    public const string ReceiverConnectionString =       @"Data Source=.\SQLEXPRESS;Integrated Security=True;Initial Catalog=ReceiverCatalog";
    public const string SenderConnectionString =         @"Data Source=.\SQLEXPRESS;Integrated Security=True;Initial Catalog=SenderCatalog";
    public const string ServiceControlConnectionString = @"Data Source=.\SQLEXPRESS;Integrated Security=True;Initial Catalog=ServiceControlCatalog";

    public static ConnectionInfo GetConnection(string transportAddress)
    {
        string cs;

        if (transportAddress == "error" || transportAddress == "audit")
            cs = ServiceControlConnectionString;
        else
            cs = transportAddress.StartsWith("Samples.SqlServer.MultiInstanceSender")
                ? SenderConnectionString
                : ReceiverConnectionString;

        Console.WriteLine(" {0} = {1}", transportAddress, cs);
        return ConnectionInfo.Create().UseConnectionString(cs);
    }
}
#endregion
