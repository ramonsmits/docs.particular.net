using log4net.Config;
using NServiceBus;
using NServiceBus.Logging;

public static class InitLogging
{
    public static void Init()
    {
        XmlConfigurator.Configure();
        //var layout = new PatternLayout
        //{
        //    ConversionPattern = "%d [%t] %-5p %c [%x] - %m%n"
        //};
        //layout.ActivateOptions(); // <-- This is added
        //var eventAppender = new EventLogAppender
        //{
        //    Threshold = Level.Info,
        //    Layout = layout,
        //    //LogName = "QuantumChoice",
        //    //ApplicationName = specifier.ToString().Remove(specifier.ToString().LastIndexOf('.'))
        //    ApplicationName = "PubSub",
        //};
        //// Note that ActivateOptions is required in NSB 5 and above
        //eventAppender.ActivateOptions();
        //BasicConfigurator.Configure(eventAppender);
        LogManager.Use<Log4NetFactory>();
    }
}
