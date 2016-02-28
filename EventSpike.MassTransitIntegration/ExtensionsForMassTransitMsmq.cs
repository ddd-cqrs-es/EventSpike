namespace EventSpike.MassTransitIntegration
{
    public static class ExtensionsForMassTransitMsmq
    {
        public static string AsEndpointUri(this string endpointName)
        {
            return string.Format("msmq://localhost/{0}", endpointName);
        }
    }
}