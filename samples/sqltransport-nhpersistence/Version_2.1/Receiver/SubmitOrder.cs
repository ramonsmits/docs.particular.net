using NServiceBus;

public class SubmitOrder : ICommand
{
    public string OrderId { get; set; }
    public decimal Value { get; set; }
}