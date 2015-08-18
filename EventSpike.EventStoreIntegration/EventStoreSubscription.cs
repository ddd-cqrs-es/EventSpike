namespace EventSpike.EventStoreIntegration
{
    public struct EventStoreSubscription
    {
        public EventStoreSubscription(string streamName, string groupName)
        {
            StreamName = streamName;
            GroupName = groupName;
        }

        public readonly string StreamName;
        public readonly string GroupName;
    }
}