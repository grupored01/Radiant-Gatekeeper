namespace OTE.eAdapter.DataTypes.Interchange
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;
    using System.Xml.Serialization;
     

    /// <summary>
    /// Interchange Shipment
    /// </summary>
    public partial class Shipment
    {
        /// <summary>
        /// Retrieves the Shipment Identifier
        /// </summary>
        /// <param name="pIdentifierType">Shipment Identifier Type</param>
        /// <returns>Shipment Identifier</returns>
        public string RetrieveShipmentIdentifier(ShipmentIdentifierType pIdentifierType)
        {
            if (this.ShipmentIdentifier != null && this.ShipmentIdentifier.Length > 0)
            {
                return this.ShipmentIdentifier.Where(si => si.ShipmentIdentifierType == pIdentifierType).Select(si => si.Value).FirstOrDefault();
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Retrieves the Shipment Number
        /// </summary>
        /// <returns>Shipment Number</returns>
        public string RetrieveShipmentNumber()
        {
            string shipmentNumber = this.ShipmentDetails.AgentReference;

            if (string.IsNullOrWhiteSpace(shipmentNumber))
            {
                shipmentNumber = this.RetrieveShipmentIdentifier(ShipmentIdentifierType.Housebill);
            }

            return shipmentNumber;
        }

        /// <summary>
        /// Retrieves the POD Date
        /// </summary>
        /// <returns>POD Date</returns>
        public DateTime? RetrievePODDate()
        {
            if (this.ShipmentDetails.Deliver.GoodsDelivered != default(DateTime))
            {
                return this.ShipmentDetails.Deliver.GoodsDelivered;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Retrieves the POD Signed By
        /// </summary>
        /// <returns>POD Signed By</returns>
        public string RetrievePODSignedBy()
        {
            if (this.ShipmentDetails.Deliver != null && this.ShipmentDetails.Deliver.DeliveryLegs != null)
            {
                return this.ShipmentDetails.Deliver.DeliveryLegs.Where(dl => dl.LegType == ContainerLegType.DLV).Select(dl => dl.GoodsRecBy).FirstOrDefault();
            }
            else
            {
                return default(string);
            }
        }

        /// <summary>
        /// Retrieves Package Custom Text1 field
        /// </summary>
        /// <returns>Package Custom Text1 field</returns>
        public string RetrievePackageCustomText1()
        {
            if (this.ShipmentDetails.Packages != null)
            {
                return this.ShipmentDetails.Packages.Where(pk => pk.Custom != null && !string.IsNullOrWhiteSpace(pk.Custom.Text1)).Select(pk => pk.Custom.Text1).FirstOrDefault();
            }
            else
            {
                return default(string);
            }
        }
    }
}