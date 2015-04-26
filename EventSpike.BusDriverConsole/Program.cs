using System;
using System.Linq;
using System.Threading.Tasks;
using EventSpike.Common;
using EventSpike.Common.ApprovalCommands;
using EventSpike.Common.MassTransit;
using EventSpike.Common.Registries;
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

            var commands = Enumerable.Repeat<Func<object>>(() => new InitiateApproval
            {
                Id = Guid.NewGuid(),
                Title = "I need dis",
                Description = "Pretty plz, with sugar on top",
            }, Int32.MaxValue);

            foreach (var commandFactory in commands)
            {
                Console.WriteLine("Press [enter] to send next command...");
                Console.ReadLine();

                bus.FastInvoke(x => x.Publish(null, default(Action<IPublishContext>)), commandFactory(), new Action<IPublishContext>(context =>
                {
                    context.SetHeader(Constants.CausationIdKey, Guid.NewGuid().ToString());
                    context.SetHeader(Constants.TenantIdKey, tenantId);
                    context.SetHeader(Constants.UserIdKey, userId);
                }));
            }

            Console.WriteLine("Press [enter] to exit");
            Console.ReadLine();
        }
    }
}