namespace OTE.eAdapter.DataTypes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;

    /// <summary>
    /// eAdapter Message Header
    /// </summary>
    public class eAdapterMessageHeader
    {
        private MessageType messageType = MessageType.UniversalShipment;
        private MessageTypeCode messageTypeCode = MessageTypeCode.XUS;
        private WrapperType messageWrapperType = WrapperType.Universal;
        private string reference = string.Empty;
        private string senderID = string.Empty;
        private string recipientID = string.Empty;

        /// <summary>
        /// Message Type
        /// </summary>
        public MessageType MessageType
        {
            get
            {
                return this.messageType;
            }
        }

        /// <summary>
        /// Message Type Code
        /// </summary>
        public MessageTypeCode MessageTypeCode
        {
            get
            {
                return this.messageTypeCode;
            }
        }

        /// <summary>
        /// Wrapper Type
        /// </summary>
        public WrapperType MessageWrapperType
        {
            get
            {
                return this.messageWrapperType;
            }
        }

        /// <summary>
        /// Reference
        /// </summary>
        public string Reference
        {
            get
            {
                return this.reference;
            }
        }
        
        /// <summary>
        /// Sender ID
        /// </summary>
        public string SenderID
        {
            get
            {
                return this.senderID;
            }
        }
        
        /// <summary>
        /// Recipient ID
        /// </summary>
        public string RecipientID
        {
            get
            {
                return this.recipientID;
            }
        }

        /// <summary>
        /// Creates a new instance of eAdapterMessageHeader
        /// </summary>
        private eAdapterMessageHeader()
        {
        }

        /// <summary>
        /// Creates a new instance of eAdapterMessageHeader
        /// </summary>
        /// <param name="pMessage">eAdapter Message</param>
        public eAdapterMessageHeader(string pMessage)
        {
            try
            {
                XmlDocument xml = new XmlDocument();
                XmlNamespaceManager namespaces = new XmlNamespaceManager(xml.NameTable);

                this.messageWrapperType = eAdapterDataTypesFactory.RetrieveWrapperType(pMessage);

                xml.LoadXml(pMessage);

                XmlNodeList nodes = null;
                XmlNode node = null;

                switch (this.messageWrapperType)
                {
                    case WrapperType.Universal:
                        namespaces.AddNamespace("ns", eAdapterDataTypesFactory.CARGOWISE_UNIVERSAL_NAMESPACE);

                        nodes = xml.SelectNodes("//ns:UniversalInterchange/ns:Header", namespaces);

                        this.senderID = nodes[0]["SenderID"].InnerText;
                        this.recipientID = nodes[0]["RecipientID"].InnerText;

                        nodes = xml.SelectNodes("//ns:UniversalInterchange/ns:Body", namespaces);

                        node = nodes[0].FirstChild;

                        switch (node.Name.ToLowerInvariant())
                        {
                            case "universalevent":
                                this.messageType = MessageType.UniversalEvent;
                                this.messageTypeCode = DataTypes.MessageTypeCode.XUE;

                                nodes = xml.SelectNodes("//ns:UniversalInterchange/ns:Body/ns:UniversalEvent/ns:Event/ns:DataContext/ns:DataSourceCollection/ns:DataSource", namespaces);

                                List<string> typesEvents = nodes.Cast<XmlNode>().Select(n => n.FirstChild.FirstChild.Value).ToList();

                                if (!typesEvents.Contains("ForwardingShipment") && typesEvents.Contains("ForwardingConsol"))
                                {
                                    this.reference = (from n in nodes.Cast<XmlNode>() where n.FirstChild.FirstChild.Value == "ForwardingConsol" select n.FirstChild.NextSibling.FirstChild.Value).First();
                                }
                                else if (typesEvents.Contains("ForwardingShipment"))
                                {
                                    this.reference = (from n in nodes.Cast<XmlNode>() where n.FirstChild.FirstChild.Value == "ForwardingShipment" select n.FirstChild.NextSibling.FirstChild.Value).First();
                                }
                                else
                                {
                                    throw new eAdapterFormatException(string.Format("Unexpected Forwarding type: {0}", typesEvents.First()));
                                }
                                break;
                            case "universalshipment":
                                nodes = xml.SelectNodes("//ns:UniversalInterchange/ns:Body/ns:UniversalShipment/ns:Shipment/ns:DataContext/ns:DataSourceCollection/ns:DataSource", namespaces);

                                List<string> typesShipments = nodes.Cast<XmlNode>().Select(n => n.FirstChild.FirstChild.Value).ToList();

                                if (!typesShipments.Contains("ForwardingShipment") && typesShipments.Contains("ForwardingConsol"))
                                {
                                    this.messageType = MessageType.UniversalConsolidation;
                                    this.messageTypeCode = DataTypes.MessageTypeCode.XUC;
                                    this.reference = (from n in nodes.Cast<XmlNode>() where n.FirstChild.FirstChild.Value == "ForwardingConsol" select n.FirstChild.NextSibling.FirstChild.Value).First();
                                }
                                else if (typesShipments.Contains("ForwardingShipment"))
                                {
                                    this.messageType = MessageType.UniversalShipment;
                                    this.messageTypeCode = DataTypes.MessageTypeCode.XUS;
                                    this.reference = (from n in nodes.Cast<XmlNode>() where n.FirstChild.FirstChild.Value == "ForwardingShipment" select n.FirstChild.NextSibling.FirstChild.Value).First();
                                }
                                else
                                {
                                    throw new eAdapterFormatException(string.Format("Unexpected Forwarding type: {0}", typesShipments.First()));
                                }
                                break;
                            case "universaltransactionbatch":
                                this.messageType = MessageType.UniversalAccountingBatch;
                                this.messageTypeCode = DataTypes.MessageTypeCode.UAB;
                                break;
                            case "universaltransaction":
                                this.messageType = MessageType.UniversalTransactionInfo;
                                this.messageTypeCode = DataTypes.MessageTypeCode.XUT;

                                nodes = xml.SelectNodes("//ns:UniversalInterchange/ns:Body/ns:UniversalTransaction/ns:TransactionInfo/ns:DataContext/ns:DataSourceCollection/ns:DataSource", namespaces);

                                this.reference = (from n in nodes.Cast<XmlNode>() where n.FirstChild.FirstChild.Value == "AccountingInvoice" select n.FirstChild.NextSibling.FirstChild.Value).First();
                                break;
                            default:
                                throw new eAdapterFormatException(string.Format("Unexpected Universal type: {0}", node.Name));
                        }
                        break;
                    case WrapperType.Interchange:
                        this.senderID = string.Empty;

                        // There is no recipientId on the interchange messages, so all the processors that would need them should be hard coded
                        this.recipientID = "AIS";

                        namespaces.AddNamespace("ns", eAdapterDataTypesFactory.CARGOWISE_INTERCHANGE_NAMESPACE);
                        nodes = xml.SelectNodes("//ns:XmlInterchange/ns:Payload", namespaces);

                        node = nodes[0].FirstChild;

                        switch (node.Name.ToLowerInvariant())
                        {
                            case "shipments":
                                this.messageType = MessageType.InterchangeShipment;
                                this.messageTypeCode = DataTypes.MessageTypeCode.XML;

                                nodes = xml.SelectNodes("//ns:XmlInterchange/ns:Payload/ns:Shipments/ns:Shipment/ns:ShipmentDetails/ns:AgentReference/text()", namespaces);

                                if (nodes.Count > 0)
                                {
                                    this.reference = nodes[0].Value;
                                }
                                else
                                {
                                    this.reference = xml.SelectNodes("//ns:XmlInterchange/ns:Payload/ns:Shipments/ns:Shipment/ns:ShipmentIdentifier[@ShipmentIdentifierType='Housebill']/text()", namespaces)[0].Value;
                                }
                                break;
                            default:
                                throw new eAdapterFormatException(string.Format("Unexpected Interchange type: {0}", node.Name));
                        }
                        break;
                    default:
                        throw new eAdapterFormatException(string.Format("Unexpected Wrapper type: {0}", pMessage.Substring(0, 15)));
                }
            }
            catch (eAdapterFormatException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new eAdapterHeaderException("Unable to parse the header message", ex);
            }
        }

        public override string ToString()
        {
            return string.Format("Sender: {0}, Recipient: {1}, Type: {2}", this.senderID, this.recipientID, this.messageType);
        }
    }
}
