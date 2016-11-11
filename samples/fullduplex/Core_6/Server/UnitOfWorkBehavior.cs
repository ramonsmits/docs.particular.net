using System;
using System.Threading.Tasks;
using System.Transactions;
using NHibernate;
using NServiceBus.Pipeline;

class QuerySession
{
    public ISession Session { get; }

    public QuerySession(ISessionFactory factory)
    {
        Session = factory.OpenSession();
        Session.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);
    }
}

class UnitOfWorkBehavior : Behavior<IIncomingLogicalMessageContext>
{
    public override async Task Invoke(IIncomingLogicalMessageContext context, Func<Task> next)
    {
        QuerySession querySession;

        using (var outer = new TransactionScope(TransactionScopeOption.Suppress)) //, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted }, TransactionScopeAsyncFlowOption.Enabled))
        {
            querySession = context.Builder.Build<QuerySession>();
        }

        try
        {
            await next().ConfigureAwait(false);
        }
        finally
        {
            querySession.Session?.Dispose();
        }
    }
}
