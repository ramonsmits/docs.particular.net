using System;
using Messages;
using NServiceBus.Saga;

public class TestSaga : Saga<TestSaga.MyData>, IAmStartedByMessages<ClientOrder>, IHandleTimeouts<ClientOrderTimeout>
{
    public void Handle(ClientOrder message)
    {
        Console.WriteLine("!!!!!!");
        Data.OrderId = message.OrderId;
        RequestTimeout<ClientOrderTimeout>(TimeSpan.FromSeconds(10));
    }

    public void Timeout(ClientOrderTimeout state)
    {
        Console.WriteLine("REQUESTED TIMEOUT HAS ARRIVED");
        MarkAsComplete();
    }

    protected override void ConfigureHowToFindSaga(SagaPropertyMapper<MyData> mapper)
    {
        mapper.ConfigureMapping<ClientOrder>(x => x.OrderId).ToSaga(x => x.OrderId);
    }

    public class MyData : ContainSagaData
    {
        public Guid OrderId { get; set; }
    }
}