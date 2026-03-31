using pr.net.Models.Incoming.Generic;

namespace pr.net.Services.Context;

public interface IAmbientContextService<TEventCreated> where TEventCreated : PullReviewCreatedEvent {

    Dictionary<long, TEventCreated> CreatedEvents { get; set; }

}