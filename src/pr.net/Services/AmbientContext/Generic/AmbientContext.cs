using pr.net.Models.Incoming.Generic;

namespace pr.net.Services.Context;

public class AmbientContext<TEventCreated>(TEventCreated _createdEvent) : IAmbientContext<TEventCreated> where TEventCreated : PullReviewCreatedEvent {

    public TEventCreated CreatedEvent { 
        get => _createdEvent; 
        set => _createdEvent = value; 
    }

}