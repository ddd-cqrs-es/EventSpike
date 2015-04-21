using System;

namespace EventSpike.Common
{
    public class CommandEnvelope<TCommand>
    {
        public string TenantId { get; set; }
        public Guid CausationId { get; set; }
        public string UserId { get; set; }
        public TCommand Body { get; set; }
    }
}
