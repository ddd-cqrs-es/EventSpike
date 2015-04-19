using System;
using System.Threading.Tasks;
using Magnum.Reflection;
using MassTransit;
using NEventStore.Spike.Common;
using NEventStore.Spike.Common.ApprovalCommands;
using NEventStore.Spike.Common.Registries;
using StructureMap;

namespace NEventStore.Spike.BusDriverConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var endpointName = typeof (Program).ToEndpointName();

            var container = new Container(configure => configure.AddRegistry(new MassTransitRegistry(endpointName)));

            var bus = container.GetInstance<IServiceBus>();

            var task = Task.Run(() => DispatchCommands(bus));

            task.Wait();
        }

        public static void DispatchCommands(IServiceBus bus)
        {
            var approvalId = Guid.NewGuid();
            const string tenantId = "tenant-1";
            const string userId = "user-1";

            var initiateApprovalCommand = new InitiateApproval
            {
                Id = approvalId,
                CausationId = Guid.NewGuid(),
                Title = "I need dis",
                Description = "Pretty plz, with sugar on top",
                TenantId = tenantId,
                UserId = userId
            };

            var pendingCommands = new[]
            {
                initiateApprovalCommand
            };

            foreach (var command in pendingCommands)
            {
                Console.WriteLine("Press [enter] to send next command...");
                Console.ReadLine();
                
                bus.FastInvoke(x => x.Publish(null), command);
            }

            Console.WriteLine("Press [enter] to exit");
            Console.ReadLine();
        }
    }
}
