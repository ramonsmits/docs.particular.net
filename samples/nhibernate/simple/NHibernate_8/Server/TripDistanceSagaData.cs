using System;
using System.Collections.Generic;
using NServiceBus;
using NServiceBus.SagaPersisters.NHibernate;

public class TripDistanceSagaData : IContainSagaData
{
    public TripDistanceSagaData()
    {
        DistancesData = new List<TripDistanceData>();
        IsCreated = false;
    }

    public virtual Guid Id { get; set; }
    public virtual string OriginalMessageId { get; set; }
    public virtual string Originator { get; set; }
    public virtual long TripKey { get; set; } // This changed from decimal to int in support of NSB upgrade v5.x to 6.x; also, "unique" atribute removed per NSB Upgrade
    public virtual decimal SourceKey { get; set; }
    public virtual bool InvalidDistance { get; set; } //Indicates the distance should not be used
    public virtual IList<TripDistanceData> DistancesData { get; set; }
    public virtual decimal? StartLatitude { get; set; }
    public virtual decimal? StartLongitude { get; set; }
    public virtual decimal? EndLatitude { get; set; }
    public virtual decimal? EndLongitude { get; set; }
    public virtual bool IsCreated { get; set; }
    [RowVersion]
    public virtual long V { get; set; }
}

public class TripDistanceData
{
    public virtual Guid Id { get; set; }
    public virtual CalculationMethod CalculationMethod { get; set; }
    public virtual decimal TripDistance { get; set; }
    public virtual DateTime LastReceived { get; set; }
}
