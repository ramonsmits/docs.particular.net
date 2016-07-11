using System.Reflection;
using NServiceBus;

public static class Conventions
{
    public static void SetConventions(this EndpointConfiguration instance)
    {
        var asm = Assembly.GetExecutingAssembly();

        var c = instance.Conventions();
        c.DefiningMessagesAs(t => t.Name.EndsWith("Message") && t.Assembly == asm);
        c.DefiningCommandsAs(t => t.Name.EndsWith("Command") && t.Assembly == asm);
        c.DefiningEventsAs(t => t.Name.EndsWith("Event") && t.Assembly == asm);
    }
}