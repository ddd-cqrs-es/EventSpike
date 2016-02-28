using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EventSpike.Approval.Messages.Commands;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace EventSpike.EventStoreStreamHeadwaterConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var adminCredentials = new UserCredentials("admin", "changeit");

            var settings = ConnectionSettings.Create()
                .SetDefaultUserCredentials(adminCredentials)
                .UseConsoleLogger()
                .KeepReconnecting()
                .KeepRetrying();

            var uri = new UriBuilder("tcp", "127.0.0.1", 1113).Uri;

            var connection = EventStoreConnection.Create(settings, uri);

            connection.ConnectAsync().Wait();

            DispatchCommands(connection);
        }

        private static void DispatchCommands(IEventStoreConnection connection)
        {
            const string streamName = "approval_commands";
            const string tenantId = "tenant-1";
            const string userId = "user-1";

            var commands = Enumerable.Repeat<Func<object>>(() => new InitiateApproval
            {
                Id = Guid.NewGuid(),
                Title = "I need dis",
                Description = "Pretty plz, with sugar on top",
            }, int.MaxValue);
            
            foreach (var commandFactory in commands)
            {
                Console.WriteLine("Press [enter] to send next command...");

                if (string.Equals(Console.ReadLine(), "quit", StringComparison.OrdinalIgnoreCase)) break;

                var command = commandFactory();

                var metadata = new Dictionary<string, string>
                {
                    {Constants.CausationIdKey, Guid.NewGuid().ToString()},
                    {Constants.TenantIdKey, tenantId},
                    {Constants.UserIdKey, userId}
                };

                var settings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };

                metadata.ChangeKey(Constants.CausationIdKey, "$correlationId");

                var commandJson = JsonConvert.SerializeObject(command, settings);
                var metadataJson = JsonConvert.SerializeObject(metadata, settings);

                var typeName = command.GetType().Name.ToCamelCase();

                var eventData = new EventData(Guid.NewGuid(), typeName, true, Encoding.UTF8.GetBytes(commandJson), Encoding.UTF8.GetBytes(metadataJson));

                connection.AppendToStreamAsync(streamName, ExpectedVersion.Any, eventData);
            }
        }
    }
}
