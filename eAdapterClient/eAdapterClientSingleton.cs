namespace WcfRadiant
{
    using NativeService = OTE.eAdapter.NativeDataService;
    using NativeServiceTest = OTE.eAdapter.NativeDataServiceTest;

    /// <summary>
    /// Singleton eAdapter Services
    /// </summary>
    public static class eAdapterClientSingleton
    {
        private static eAdapterClient client = null;
        private static NativeService.EnterpriseNativeDataServiceClient native = null;

        private static eAdapterClient clientTest = null;
        private static NativeServiceTest.EnterpriseNativeDataServiceClient nativeTest = null;

        /// <summary>
        /// eAdapter Client
        /// </summary>
        public static eAdapterClient ClientTest
        {
            get
            {
                if (clientTest == null)
                {
                    clientTest = new eAdapterClient("https://www01-ote.ote.local/Train/Services/eAdapterStreamedService.svc", "OTXOTETST", "test", "test");
                }

                return clientTest;
            }
        }

        /// <summary>
        /// eAdapter Client - Test
        /// </summary>
        public static eAdapterClient Client
        {
            get
            {
                if (client == null)
                {
                    client = new eAdapterClient("https://www01-ote.ote.local/OnTrax/Services/eAdapterStreamedService.svc", "OTXOTEPHX", "test", "test");
                }

                return client;
            }
        }

        /// <summary>
        /// Native Client
        /// </summary>
        public static NativeService.EnterpriseNativeDataServiceClient Native
        {
            get
            {
                if (native == null)
                {
                    native = new NativeService.EnterpriseNativeDataServiceClient();
                }

                return native;
            }
        }

        /// <summary>
        /// Native Client - Test
        /// </summary>
        public static NativeServiceTest.EnterpriseNativeDataServiceClient NativeTest
        {
            get
            {
                if (nativeTest == null)
                {
                    nativeTest = new NativeServiceTest.EnterpriseNativeDataServiceClient();
                }

                return nativeTest;
            }
        }
    }
}
