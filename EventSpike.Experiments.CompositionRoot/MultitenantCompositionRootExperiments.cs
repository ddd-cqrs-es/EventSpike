using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using EventSpike.SqlIntegration;
using EventStore.ClientAPI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Paramol;
using Paramol.SqlClient;

namespace EventSpike.Experiments.CompositionRoot
{
    [TestClass]
    public class MultitenantCompositionRootExperiments
    {
        [TestMethod]
        public void Using_a_multitenant_root()
        {

        }

        // http://blog.ploeh.dk/2014/06/03/compile-time-lifetime-matching/

        // http://blog.ploeh.dk/2015/01/06/composition-root-reuse/
        // > A Composition Root is application-specific. It makes no sense to reuse it across code bases.
        // > A Composition Root is application-specific; it's what defines a single application.
        // > After having written nice, decoupled code throughout your code base, the Composition Root is where you finally couple everything, from data access to (user) interfaces.


        internal class ApplicationCompositionRoot
        {
            private readonly ConcurrentDictionary<string, HandlerCompositionRoot> _handlerRoots = new ConcurrentDictionary<string, HandlerCompositionRoot>();

            public ApplicationCompositionRoot()
            {
                var connection = EventStoreConnection
                    .Create(ConnectionSettings.Create().Build(), new IPEndPoint(IPAddress.Loopback, 1113));

                var ingressSubscription = connection.SubscribeToStreamFrom("ingress", StreamCheckpoint.StreamStart, false, (subscription, resolvedEvent) =>
                {
                    var headerJson = Encoding.UTF8.GetString(resolvedEvent.Event.Metadata);
                    var header = JObject.Parse(headerJson);
                    var tenantId = header["tenantId"].Value<string>();

                    var messageJson = Encoding.UTF8.GetString(resolvedEvent.Event.Data);
                    var messageTypeName = header["$type"].Value<string>();
                    var messageType = Type.GetType(messageTypeName, false, false);
                    var message = JsonConvert.DeserializeObject(messageJson, messageType);

                    var handlerRoot = _handlerRoots.GetOrAdd(tenantId, _ => new HandlerCompositionRoot(tenantId));

                    var messages = new[] { message };

                    do
                    {
                        messages = messages.Select(msg => handlerRoot.Handle(msg).ToArray()).SelectMany(_ => _).ToArray();
                    } while (messages.Any());
                });
            }
        }

        private class HandlerCompositionRoot : IDisposable
        {
            private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
            private readonly Dispatcher<object, IEnumerable<object>> _dispatcher;

            public HandlerCompositionRoot(string tenantId)
            {
                var connectionSettingsFactory = new ConventionTenantSqlConnectionSettingsFactory(tenantId);

                _dispatcher = new Dispatcher<object, IEnumerable<object>>();

                var connectionSettings = new
                {
                    Domain = connectionSettingsFactory.GetSettings(),
                    Projections = connectionSettingsFactory.GetSettings("Projections")
                };

                _dispatcher.Register<ItemPurchased>(Handlers.Handle);

                _dispatcher.Register<SqlNonQueryCommand>(command =>
                {
                    return Enumerable.Empty<object>();
                });
            }

            public IEnumerable<object> Handle<TMessage>(TMessage message)
            {
                return _dispatcher.Dispatch(message);
            }

            public void Dispose()
            {
                _compositeDisposable.Dispose();
            }
        }

        internal class ItemPurchased
        {
            public readonly string StockKeepingUnit;

            public ItemPurchased(string stockKeepingUnit)
            {
                StockKeepingUnit = stockKeepingUnit;
            }
        }

        internal class Handlers
        {
            public static IEnumerable<object> Handle(ItemPurchased purchased)
            {
                yield return TSql.NonQueryStatement(@"insert into ItemsPurchased (StockKeepingUnit) values (@StockKeepingUnit)", new
                {
                    purchased.StockKeepingUnit
                });
            }
        }
    }
}
