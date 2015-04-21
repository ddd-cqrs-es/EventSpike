using System;
using System.Linq;
using System.Threading.Tasks;
using EventSpike.Common;
using EventSpike.Common.ApprovalCommands;
using EventSpike.Common.MassTransit;
using Magnum.Reflection;
using MassTransit;
using StructureMap;

namespace EventSpike.BusDriverConsole
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var endpointName = typeof (Program).ToEndpointName();

            var container = new Container(configure =>
            {
                configure.AddRegistry<MassTransitRegistry>();

                configure
                    .For<string>()
                    .Add(endpointName)
                    .Named(MassTransitRegistry.InstanceNames.DataEndpointName);
            });

            var bus = container.GetInstance<IServiceBus>();

            var task = Task.Run(() => DispatchCommands(bus));

            task.Wait();
        }

        public static void DispatchCommands(IServiceBus bus)
        {
            const string tenantId = "tenant-1";
            const string userId = "user-1";

            var commands = Enumerable.Repeat<Func<object>>(() => new CommandEnvelope<InitiateApproval>
            {
                CausationId = Guid.NewGuid(),
                TenantId = tenantId,
                UserId = userId,
                Body = new InitiateApproval
                {
                    Id = Guid.NewGuid(),
                    Title = "I need dis",
                    Description = "Pretty plz, with sugar on top",
                }
            }, Int32.MaxValue);

            foreach (var commandFactory in commands)
            {
                Console.WriteLine("Press [enter] to send next command...");
                Console.ReadLine();

                bus.FastInvoke(x => x.Publish(null), commandFactory());
            }

            Console.WriteLine("Press [enter] to exit");
            Console.ReadLine();
        }
    }
}