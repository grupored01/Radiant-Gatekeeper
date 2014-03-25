using System;
using System.ServiceModel;
using CargoWise.eHub.Common;

namespace WcfRadiant
{
    internal class eAdapterHttpsConfiguration : ServiceConfiguration
    {
        public string ServiceAddress { get; private set; }
        public eAdapterHttpsConfiguration(string serviceAddress)
        {
            ServiceAddress = serviceAddress;
        }

        public override System.ServiceModel.EndpointAddress EndpointAddress
        {
            get { return new EndpointAddress(new Uri(ServiceAddress)); }
        }
    }
}
