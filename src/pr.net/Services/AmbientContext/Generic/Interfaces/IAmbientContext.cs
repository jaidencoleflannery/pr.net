using pr.net.Models.Incoming.Generic;

namespace pr.net.Services.Context;

public interface IAmbientContext<TEventCreated> {

    public TEventCreated CreatedEvent { get; set; }

}