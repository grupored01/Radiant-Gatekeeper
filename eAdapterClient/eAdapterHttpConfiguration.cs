using System;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Text;
using CargoWise.eHub.Common;

namespace WcfRadiant
{
    internal class eAdapterHttpConfiguration : IServiceConfiguration
    {
        private string ServiceAddress { get; set; }
        public eAdapterHttpConfiguration(string serviceAddress)
        {
            ServiceAddress = serviceAddress;
        }

        public System.ServiceModel.EndpointAddress EndpointAddress
        {
            get { return new EndpointAddress(new Uri(ServiceAddress)); }
        }

        public System.ServiceModel.Channels.Binding Binding
        {
            get
            {
                var binding = new BasicHttpBinding();
                binding.CloseTimeout = new TimeSpan(0, 1, 0);
                binding.OpenTimeout = new TimeSpan(0, 1, 0);
                binding.ReceiveTimeout = new TimeSpan(0, 10, 0);
                binding.SendTimeout = new TimeSpan(0, 1, 0);
                binding.AllowCookies = false;
                binding.BypassProxyOnLocal = false;
                binding.HostNameComparisonMode = HostNameComparisonMode.StrongWildcard;
                binding.MaxBufferSize = 65536;
                binding.MaxBufferPoolSize = 524288;
                binding.MaxReceivedMessageSize = 65536;
                binding.MessageEncoding = WSMessageEncoding.Text;
                binding.TextEncoding = Encoding.UTF8;
                binding.TransferMode = TransferMode.Buffered;
                binding.UseDefaultWebProxy = true;

                binding.ReaderQuotas.MaxDepth = 32;
                binding.ReaderQuotas.MaxStringContentLength = 8192;
                binding.ReaderQuotas.MaxArrayLength = 16384;
                binding.ReaderQuotas.MaxBytesPerRead = 4096;
                binding.ReaderQuotas.MaxNameTableCharCount = 16384;

                binding.Security.Mode = BasicHttpSecurityMode.None;
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
                binding.Security.Transport.ProxyCredentialType = HttpProxyCredentialType.None;
                binding.Security.Transport.Realm = "";
                binding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;
                binding.Security.Message.AlgorithmSuite = SecurityAlgorithmSuite.Default;

                return binding;
            }
        }
    }
}
