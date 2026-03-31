using pr.net.Models.Incoming.Generic;

namespace pr.net.Services.Context;

public abstract class AmbientContextService<TEventCreated>() : IAmbientContextService<TEventCreated> where TEventCreated : PullReviewCreatedEvent {
    // ambient context allows us to push our payload into an object and inject is as needed instead of passing it through the pipeline.

    public Dictionary<long, TEventCreated> CreatedEvents { get; set; } = new();

}