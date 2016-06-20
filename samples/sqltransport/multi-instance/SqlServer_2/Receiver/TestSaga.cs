using System;
using Messages;
using NServiceBus.Saga;

public class TestSaga : Saga<TestSaga.MyData>, IAmStartedByMessages<ClientOrder>
{
    public class MyData : ContainSagaData
    {
        public Guid OrderId { get; set; }
    }

    protected override void ConfigureHowToFindSaga(SagaPropertyMapper<MyData> mapper)
    {
        mapper.ConfigureMapping<ClientOrder>(x => x.OrderId).ToSaga(x => x.OrderId);

    }

    public void Handle(ClientOrder message)
    {
        Console.WriteLine("!!!!!!");
        Data.OrderId = message.OrderId;
    }
}