using System;
using System.Collections;
using System.Collections.Concurrent;
using EventSpike.SqlIntegration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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

        internal class ApplicationCompositionRoot
        {
            private readonly ConcurrentDictionary<string, TenantCompositionRoot> _tenantRoots = new ConcurrentDictionary<string, TenantCompositionRoot>();

            public ApplicationCompositionRoot()
            {
                
            }
        }
        private class TenantCompositionRoot : IDisposable
        {
            private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

            public TenantCompositionRoot(string tenantId)
            {
                var connectionSettingsFactory = new ConventionTenantSqlConnectionSettingsFactory(tenantId);

                var dispatcher = new Dispatcher<object, IEnumerable>();

                var connectionSettings = new
                {
                    Domain = connectionSettingsFactory.GetSettings(),
                    Projections = connectionSettingsFactory.GetSettings("Projections")
                };

                dispatcher.Register<ItemPurchased>(Handlers.Handle);
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
            public static IEnumerable Handle(ItemPurchased purchased)
            {
                yield return TSql.NonQueryStatement(@"insert into ItemsPurchased (StockKeepingUnit) values (@StockKeepingUnit)", new
                {
                    purchased.StockKeepingUnit
                });
            }
        }
    }
}
