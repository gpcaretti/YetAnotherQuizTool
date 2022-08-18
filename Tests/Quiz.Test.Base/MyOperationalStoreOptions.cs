using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.Extensions.Options;

namespace Quiz.Test.Base {
    internal class MyOperationalStoreOptions : IOptions<OperationalStoreOptions> {
        public OperationalStoreOptions Value => new OperationalStoreOptions() {
            DeviceFlowCodes = new TableConfiguration("DeviceCodes"),
            EnableTokenCleanup = false,
            PersistedGrants = new TableConfiguration("PersistedGrants"),
            TokenCleanupBatchSize = 100,
            TokenCleanupInterval = 3600,
        };
    }
}