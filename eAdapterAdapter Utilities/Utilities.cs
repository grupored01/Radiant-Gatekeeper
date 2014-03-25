using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OTE.eAdapter.DataTypes.Universal;
using OTE.eAdapter.DataTypes;
using OTE.eAdapter.DataTypes.Native;

namespace WcfRadiant
{
    public static class Utilities
    {
        /// <summary>
        /// Populates the Native Organization from the OrganizationAddress
        /// </summary>
        /// <param name="pOrganizationAddress">OrganizationAddress</param>
        public static void PopulateNativeOrganization(OrganizationAddress pOrganizationAddress)
        {
            // Use the native web service to retrieve the record
            string responseTest = eAdapterClientSingleton.Native.RetrieveData(eAdapterDataTypesFactory.CreateNativeOrganizationRequest(pOrganizationAddress.OrganizationCode));

            List<OrganizationData> nativeOrganization = eAdapterDataTypesFactory.UnwrapMessage<OrganizationData>(responseTest);

            if (nativeOrganization != null && nativeOrganization.Count > 0)
            {
                pOrganizationAddress.NativeOrganization = nativeOrganization.First().OrgHeader;
            }
        }

        /// <summary>
        /// Populates the Native Organization from the Universal Shipment
        /// </summary>
        /// <param name="pShipment">Universal Shipment</param>
        public static void PopulateNativeShipment(Shipment pShipment)
        {
            // Use the native web service to retrieve the record
            string responseTest = eAdapterClientSingleton.Native.RetrieveData(eAdapterDataTypesFactory.CreateNativeShipmentRequest(pShipment.RetrieveDataSourceValue<string>(DataSourceType.ForwardingShipment)));

            List<ShipmentData> nativeShipments = eAdapterDataTypesFactory.UnwrapMessage<ShipmentData>(responseTest);

            if (nativeShipments != null && nativeShipments.Count > 0)
            {
                pShipment.NativeJobShipment = nativeShipments.First().JobShipment;
            }
        }

        /// <summary>
        /// Populates the Native UNLOCO from the OrganizationAddress
        /// </summary>
        /// <param name="pOrganizationAddress">OrganizationAddress</param>
        public static void PopulateNativeUNLOCO(OrganizationAddress pOrganizationAddress)
        {
            if (pOrganizationAddress.Port != null && !string.IsNullOrWhiteSpace(pOrganizationAddress.Port.Code))
            {
                // Use the native web service to retrieve the record
                string responseTest = eAdapterClientSingleton.Native.RetrieveData(eAdapterDataTypesFactory.CreateNativeUNLOCORequest(pOrganizationAddress.Port.Code));

                List<UNLOCOData> nativeUNLOCO = eAdapterDataTypesFactory.UnwrapMessage<UNLOCOData>(responseTest);

                // Return if the UNLOCO is set on the organization address
                if (nativeUNLOCO != null && nativeUNLOCO.Count > 0)
                {
                    pOrganizationAddress.NativeUNLOCO = nativeUNLOCO.First().RefUNLOCO;

                    return;
                }
            }

            if (pOrganizationAddress.NativeOrganization != null && pOrganizationAddress.NativeOrganization.ClosestPort != null && !string.IsNullOrWhiteSpace(pOrganizationAddress.NativeOrganization.ClosestPort.Code))
            {
                // Use the native web service to retrieve the record
                string responseTest = eAdapterClientSingleton.Native.RetrieveData(eAdapterDataTypesFactory.CreateNativeUNLOCORequest(pOrganizationAddress.NativeOrganization.ClosestPort.Code));

                List<UNLOCOData> nativeUNLOCO = eAdapterDataTypesFactory.UnwrapMessage<UNLOCOData>(responseTest);

                if (nativeUNLOCO != null && nativeUNLOCO.Count > 0)
                {
                    pOrganizationAddress.NativeUNLOCO = nativeUNLOCO.First().RefUNLOCO;
                }
            }
        }
    }
}
