using pr.net.Models.Incoming.Generic;

namespace pr.net.Services.Context;

public interface IAmbientContextService<TEventCreated> where TEventCreated : PullReviewCreatedEvent {

    public TEventCreated CreatedEvent { get; set; }

}