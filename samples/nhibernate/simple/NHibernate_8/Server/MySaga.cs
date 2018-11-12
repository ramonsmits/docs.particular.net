using System;
using System.Linq;
using System.Threading.Tasks;
using NServiceBus;

public class CalculatedTripDistanceSaga
    : Saga<TripDistanceSagaData>
    , IAmStartedByMessages<CalculatedTripDistance>
    , IHandleTimeouts<CalculatedTripDistanceSaga.CalculatedTripDistanceTimeout>
{
    readonly DateTime Now = DateTime.UtcNow;
    readonly double SAGA_TIMEOUT_PERIOD = 30D;

    protected override void ConfigureHowToFindSaga(SagaPropertyMapper<TripDistanceSagaData> mapper)
    {
        mapper.ConfigureMapping<CalculatedTripDistance>(msg => msg.TripKey).ToSaga(saga => saga.TripKey);
    }

    public async Task Handle(CalculatedTripDistance message, IMessageHandlerContext context)
    {
        if (Data.IsCreated == false)  // Access is limited to the first incoming Distance Calculation message in a series only
        {
            // First Distance Calculation message sets the Saga Timeout
            await RequestTimeout<CalculatedTripDistanceTimeout>(context, TimeSpan.FromMinutes(SAGA_TIMEOUT_PERIOD))
                .ConfigureAwait(continueOnCapturedContext: false);

            Data.SourceKey = message.SourceKey;
            Data.StartLatitude = message.StartLatitude;
            Data.StartLongitude = message.StartLongitude;
            Data.EndLatitude = message.EndLatitude;
            Data.EndLongitude = message.EndLongitude;
            Data.IsCreated = true;
        }

        // Append incoming Distance Calculation Data to this Trip Distance Saga Data
        AppendDistanceCalculationData(message);

        // Check if all three Distance Calculation Data message types have been aggregated: { Bing, Linear, Odometer }
        var bing = Data.DistancesData.FirstOrDefault(x => x.CalculationMethod == CalculationMethod.Bing);
        var linear = Data.DistancesData.FirstOrDefault(x => x.CalculationMethod == CalculationMethod.Linear);
        var odo = Data.DistancesData.FirstOrDefault(x => x.CalculationMethod == CalculationMethod.Odometer);

        if (bing != null && linear != null && odo != null)
        {
            // When all three Distance Calculation message types are aggregated, then Saga is complete
            await SendAndMarkComplete(context)
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        // NOTE:
        // When this method completes, NServiceBus will persist TripDistanceSagaData and DistancesData object models
        // to the [TripDistanceSagaData] [1:M] [DistancesData] ("Saga Infractructure" Tables), respectively
    }

    public async Task Timeout(CalculatedTripDistanceTimeout state, IMessageHandlerContext context)
    {
        // The Timeout Period has expired; send the Calculated Trip Distance Data regardless of how many Calculation Types are available for the Trip
        await SendAndMarkComplete(context)
            .ConfigureAwait(continueOnCapturedContext: false);
    }

    /*async*/
    Task SendAndMarkComplete(IMessageHandlerContext context)
    {
        //this select is purely do avoid a linq groupby clause not working with Oracle
        //var dataDistances = Data.DistancesData.ToList();

        //var distances = (from d in dataDistances
        //                 group d by d.CalculationMethod into r
        //                 select r.OrderByDescending(t => t.LastReceived)
        //                         .FirstOrDefault())
        //                .Select(x => new TripDistance
        //                {
        //                    CalculationMethod = x.CalculationMethod,
        //                    Distance = x.TripDistance
        //                })
        //                .ToList();

        //var command = new ReverseGeocodeTrip()
        //{
        //    TripDistances = distances,
        //    SourceKey = Data.SourceKey,
        //    TripKey = Data.TripKey,
        //    EndLatitude = Data.EndLatitude,
        //    EndLongitude = Data.EndLongitude,
        //    StartLatitude = Data.StartLatitude,
        //    StartLongitude = Data.StartLongitude
        //};

        //if (dataDistances.Any())
        //{
        //    await context.Send(message: command)
        //        .ConfigureAwait(continueOnCapturedContext: false);
        //}

        MarkAsComplete();
        return Task.CompletedTask;
    }

    void AppendDistanceCalculationData(CalculatedTripDistance message)
    {
        if (message.InvalidDistance)
        {
            return;
        }

        var distance = Data.DistancesData.FirstOrDefault(x => x.CalculationMethod == message.CalculationMethod);

        if (distance == null)
        {
            Data.DistancesData.Add(new TripDistanceData
            {
                CalculationMethod = message.CalculationMethod,
                TripDistance = message.TripDistance,
                LastReceived = Now
            });

            return;
        }

        distance.LastReceived = Now; // (Ramon) Using UTC instead of Local to not be influenced by DST
        distance.TripDistance = message.TripDistance;
    }
    
    public class CalculatedTripDistanceTimeout { }
}
